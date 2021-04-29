﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Extensions;
using InteractML;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Handles read/write logic from firebase cloud storage
/// </summary>
public class FirebaseController : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Instance firebase authentication class
    /// </summary>
    FirebaseAuth FAuth;
    /// <summary>
    /// Instance firebase cloud storage class
    /// </summary>
    FirebaseStorage FStorage;
    /// <summary>
    /// Reference to a file in store
    /// </summary>
    StorageReference FStorageRef;
    /// <summary>
    /// Needed to check when a task is available
    /// </summary>
    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    /// <summary>
    /// Let other scripts know if Firebase is successfully initialized
    /// </summary>
    protected bool isFirebaseInitialized = false;
    /// <summary>
    /// Address of storage bucket
    /// </summary>
    public string StorageBucket = "gs://fir-test-b6418.appspot.com";
    /// <summary>
    /// Currently enabled logging verbosity.
    /// </summary>
    protected Firebase.LogLevel logLevel = Firebase.LogLevel.Info;

    /// <summary>
    /// Has the file been sent? Used to allow sending files only once
    /// </summary>
    bool fileSent = false;

    /// <summary>
    /// File with encrypted credentials
    /// </summary>
    public string CredentialsPath = "Common/AuthCredentials/FirebaseUserCredentials.json.aes";

    /// <summary>
    /// Path of file or folder to upload
    /// </summary>
    public string PathToUpload;

    public Image AuthStateDisplay;
    public bool authSuccess;

    #endregion

    #region Unity Messages

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_STANDALONE
        StartCoroutine(LoadCredentialsSignIn());
#endif
    }

    private void OnDestroy()
    {
#if UNITY_STANDALONE
        SignOutFirebase(FAuth);
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Triggers sign in event
    /// </summary>
    public void SignIn()
    {
        StartCoroutine(LoadCredentialsSignIn());
    }

    /// <summary>
    /// Uploads a file or directory to the firebase server
    /// </summary>
    /// <param name="localFilePath">path in disk</param>
    /// <param name="serverFilePath">path we want to upload to in the server</param>
    public void UploadAsync(string localFilePath, string serverFilePath)
    {
        if (fileSent)
        {
            Debug.LogError("Already uploaded to server, it won't upload again with the same instance (to avoid uploading twice when not needed)");
            return;
        }

        // Get timestamp
        //string date = DateTime.Today.ToString("f");
        string date = DateTime.Now.ToLocalTime().ToString("s");

        // Add timestampt to serverfilepath
        serverFilePath = serverFilePath + "/" + date ;

#if UNITY_ANDROID
        var uploadCoroutine = UploadREST(localFilePath, serverFilePath);
        StartCoroutine(uploadCoroutine);
        fileSent = true;
#elif UNITY_STANDALONE
        if (FAuth == null || FAuth.CurrentUser == null)
        {
            Debug.LogError("Upload to Firebase failed! The app is not authenticated.");
        }
        if (FAuth != null && FAuth.CurrentUser != null && !fileSent)
        {
            var uploadCoroutine = UploadAsyncPrivate(FStorage, localFilePath, serverFilePath);
            StartCoroutine(uploadCoroutine);            
            fileSent = true;
        }
#endif

    }

    #endregion

    #region Private Methods

    protected void InitializeFirebase()
    {
        //Debug.Log("Attempting to init Firebase...");

        // Get ref to storage bucket address
        var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
        if (!String.IsNullOrEmpty(appBucket))
        {
            StorageBucket = String.Format("gs://{0}/", appBucket);
        }

        // Get a reference to authentication 
        FAuth = FirebaseAuth.DefaultInstance;

        // Get a reference to the storage service, using the default Firebase App
        FStorage = FirebaseStorage.DefaultInstance;
        FStorage.LogLevel = logLevel;


        // Update flag so other scripts are aware that firebase is init
        isFirebaseInitialized = true;

        //Debug.Log("Firebase init!");

    }

    private void SignInFirebase(FirebaseAuth auth, string email, string password)
    {
        //Debug.Log("Attempting to Sign into Firebase...");

        if (auth == null)
        {
            Debug.LogError("Auth object is null! Signing In aborted!");
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            authSuccess = true;
        });

    }

    private void SignOutFirebase(FirebaseAuth auth)
    {
        if (auth == null)
        {
            return;
        }

        auth.SignOut();

    }

    /// <summary>
    /// Loads credentials and signs into firebase over a coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadCredentialsSignIn()
    {
        // Create sign in credentials for firebase
        //FirebaseUserCredentials credentials = new FirebaseUserCredentials("email", "password");

        //// Convert to json, and store in disk
        //string credentialsJson = JsonUtility.ToJson(credentials);
        //File.WriteAllText(Application.dataPath + "/FirebaseUserCredentials.json", credentialsJson);

        //// Encrypt json file
        //encryptor.FileEncryptAsync(Application.dataPath + "/FirebaseUserCredentials.json", "patata");

        UserCredentials credentialsRead = null;

#if UNITY_ANDROID
        // Use encrypted credentials from resources folder
        var encryptedCredentialsResources = Resources.Load<TextAsset>("FirebaseUserCredentials");
        if (encryptedCredentialsResources != null)
        {
            // We read the encrypted credentials! Proceed to 

            // Save the encrypted as a .json.aes file in android assets data path
            string encryptedCredentialsPath = Path.Combine(IMLDataSerialization.GetAssetsPath(), CredentialsPath);
            //Debug.Log($"Writing credentials to {encryptedCredentialsPath}");
            var dirName = Path.GetDirectoryName(encryptedCredentialsPath);
            var fileName = Path.GetFileName(encryptedCredentialsPath);
            // Create local credentials directory if it doesn't exist
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
            // Create crypto credentials file
            var fs = File.Create(encryptedCredentialsPath);
            // close filestream before writing
            fs.Close();
            // wait a frame
            yield return null;

            //Debug.Log($"Attempting to read into folder {dirName}, searching or file {fileName}");

            // wait a frame
            yield return null;
            // Write all bytes into file
            File.WriteAllBytes(encryptedCredentialsPath, encryptedCredentialsResources.bytes);
            // waiting between frames until the file is fully written
            while (!File.Exists(Path.Combine(IMLDataSerialization.GetAssetsPath(), CredentialsPath)))
            {
                //Debug.Log($"Waiting for {encryptedCredentialsPath} to be fully written before signing into firebase...");
                yield return null;
            }
        }
#endif


        // Do we have a credentials file?
        if (File.Exists(Path.Combine(IMLDataSerialization.GetAssetsPath(), CredentialsPath)))
        {
            // Decrypt json file into file
            string decryptedCredentialsFilePath = "Common/AuthCredentials/FirebaseUserCredentialsDecrypted.json";
            Task<bool> resultDecryption = AESEncryptor.FileDecryptAsync(Path.Combine(IMLDataSerialization.GetAssetsPath(), CredentialsPath), Path.Combine(IMLDataSerialization.GetAssetsPath(), decryptedCredentialsFilePath), "patata");
            while (resultDecryption.Result == false)
            {
                // wait a frame
                yield return null;
                // it will exit this loop once result is true
            }
            // Read from file and delete
            string credentialsReadJson = File.ReadAllText(Path.Combine(IMLDataSerialization.GetAssetsPath(), decryptedCredentialsFilePath));
            // Delete json file
            File.Delete(Path.Combine(IMLDataSerialization.GetAssetsPath(), decryptedCredentialsFilePath));
            // Parse into Json
            credentialsRead = JsonUtility.FromJson<UserCredentials>(credentialsReadJson);
        }
        // If there is not a credentials file present...
        else
        {
            Debug.LogError($"There are no server credentials under {CredentialsPath}");
        }



        // Did we load any credentials?
        if (credentialsRead != null)
        {
            // One-off call to check and fix dependencies
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    // Init firebase
                    InitializeFirebase();
                    // SignIn
                    SignInFirebase(FAuth, credentialsRead.email, credentialsRead.password);
                }
                else
                {
                    Debug.LogError(
                      "Could not resolve all Firebase dependencies: " + dependencyStatus);

                }
            });

        }
        else
        {
            Debug.LogError("Credentials couldn't be loaded!");
        }


    }

    /// <summary>
    /// Upload files using Firebase SDK (not supported on Quest)
    /// </summary>
    /// <param name="storage"></param>
    /// <param name="localFilePath"></param>
    /// <param name="serverPath"></param>
    /// <returns></returns>
    private IEnumerator UploadAsyncPrivate(FirebaseStorage storage, string localFilePath, string serverPath)
    {
        if (storage == null)
        {
            Debug.LogError("Firebase Storage couldn't be initalized");
            yield break;
        }
        if (System.String.IsNullOrEmpty(localFilePath))
        {
            Debug.LogError("No file specified but uploadFile called!");
            yield break;
        }

        // Wait for a few miliseconds, to allow the training examples to be fully written in disk
        yield return new WaitForSeconds(0.2f);

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl(StorageBucket);

        // Is it a file or a folder?
        bool fileDetected = false;
        bool directoryDetected = false;
        FileAttributes attr = File.GetAttributes(localFilePath);
        //detect whether its a directory or file
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            directoryDetected = true;
        else
            fileDetected = true;


        // Upload a file on start
        // Locate file
        if (fileDetected && File.Exists(localFilePath) && storageRef != null)
        {
            //Debug.Log("Attempting file upload...");

            // Create a firebase reference based on the path wanted by the user with serverPath
            StorageReference testFileRef = storageRef.Child(serverPath);

            // Upload the file to the path 
            testFileRef.PutFileAsync(localFilePath)
                .ContinueWith((Task<StorageMetadata> task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError(task.Exception.ToString());
                        // Uh-oh, an error occurred!
                    }
                    else
                    {
                        // Metadata contains file metadata such as size, content-type, and download URL.
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        //Debug.Log("Finished uploading...");
                        //Debug.Log("md5 hash = " + md5Hash);
                    }
                });


        }
        else if (directoryDetected && Directory.Exists(localFilePath) && storageRef != null)
        {
            Debug.Log("Attempting directory upload...");

            // First, find all the folders
            // Iterate to upload all files in folder, including subdirectories
            string[] files = Directory.GetFiles(localFilePath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".meta")
                {
                    // Skip meta files
                    continue;
                }
                // Create a firebase reference to the file 
                StorageReference testFileRef = storageRef.Child(serverPath + Path.GetFileName(file));

                // Upload the file to the path 
                testFileRef.PutFileAsync(file)
                    .ContinueWith((Task<StorageMetadata> task) =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log(task.Exception.ToString());
                            // Uh-oh, an error occurred!
                        }
                        else
                        {
                            // Metadata contains file metadata such as size, content-type, and download URL.
                            StorageMetadata metadata = task.Result;
                            string md5Hash = metadata.Md5Hash;
                            Debug.Log("Finished uploading: " + file);
                            Debug.Log("md5 hash = " + md5Hash);
                        }
                    });

            }



        }
    }

    /// <summary>
    /// Upload files using REST API (all platforms, slower)
    /// </summary>
    /// <param name="localFilePath"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private IEnumerator UploadREST(string localFilePath, string serverFilePath)
    {
        if (string.IsNullOrEmpty(localFilePath))
        {
            Debug.LogError("No file specified but uploadFile called!");
            yield break;
        }

        // Wait for a few miliseconds, to allow the training examples to be fully written in disk
        yield return new WaitForSeconds(0.2f);

        // Is it a file or a folder?
        bool fileDetected = false;
        bool directoryDetected = false;
        FileAttributes attr = File.GetAttributes(localFilePath);
        //detect whether its a directory or file
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            directoryDetected = true;
        else
            fileDetected = true;


        // Upload a file on start
        // Locate file
        if (fileDetected && File.Exists(localFilePath))
        {
            Debug.Log("Attempting file upload...");
            StartCoroutine(UploadFileREST(localFilePath, serverFilePath));

        }
        else if (directoryDetected && Directory.Exists(localFilePath))
        {
            //Debug.Log("Attempting directory upload...");

            // First, find all the folders
            // Iterate to upload all files in folder, including subdirectories
            string[] files = Directory.GetFiles(localFilePath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetExtension(file) == ".meta")
                {
                    // Skip meta files
                    continue;
                }
                StartCoroutine(UploadFileREST(file, serverFilePath));

            }

        }


        #endregion


    }

    /// <summary>
    /// Uploads a single file using REST API (all platforms, slower)
    /// </summary>
    /// <param name="fileToUpload"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private IEnumerator UploadFileREST(string fileToUpload, string serverFilePath)
    {


        string fileName = Path.GetFileName(fileToUpload);
        string fileNameEscaped = System.Web.HttpUtility.UrlEncode(fileName);
        string serverFilePathEscaped = System.Web.HttpUtility.UrlEncode(serverFilePath);
        byte[] fileBinary = File.ReadAllBytes(fileToUpload);

        // HTTP
        string firebaseProjectID = "fir-test-b6418.appspot.com";
        string urlFirebase = "https://firebasestorage.googleapis.com/v0/b/" +
            firebaseProjectID + "/o/" + serverFilePathEscaped + fileNameEscaped;

        // UNITYWEBREQUEST 
        // Post file to server
        // Define a filled in upload handler
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(fileBinary);
        uploadHandler.contentType = "application/force-download";
        // Empty downloadHandler since we are only posting
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        // Prepare post request with data
        UnityWebRequest webRequest = new UnityWebRequest(urlFirebase, "POST", downloadHandlerBuffer, uploadHandler);
        //UnityWebRequest webRequest = UnityWebRequest.Post(urlFirebase, fileText);
        //webRequest.SetRequestHeader("Content-type", "text/plain");
        // Send
        yield return webRequest.SendWebRequest();

        // WWW
        //Dictionary<string, string> wwwHeaders = new Dictionary<string, string>();
        //wwwHeaders.Add("Content-type", "text/plain");
        //WWW www = new WWW(urlFirebase, fileBinary, wwwHeaders);
        //yield return www;

        // wait a frame
        yield return null;

        // UNITYWEBREQUEST
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
        }

        //// WWW
        //if (www.error == null)
        //{
        //    Debug.Log("Upload of test file complete!");

        //}
        //else
        //{
        //    Debug.LogError(www.error);
        //}


        // wait a frame
        yield return null;

    }
    /// <summary>
    /// Uploads a single file using REST API (all platforms, slower)
    /// </summary>
    /// <param name="fileToUpload"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private IEnumerator UploadFileREST(byte[] fileToUpload, string fileName, string serverFilePath)
    {

        //string fileName = "testfile.json";
        //string fileToUpload = Path.Combine(IMLDataSerialization.GetAssetsPath(), fileName);
        //string fileText = File.ReadAllText(fileToUpload);

        string fileNameEscaped = Uri.EscapeUriString(fileName);
        string serverFilePathEscaped = Uri.EscapeUriString(serverFilePath);
        byte[] fileBinary = fileToUpload;

        // HTTP
        string firebaseProjectID = "fir-test-b6418.appspot.com";
        string urlFirebase = "https://firebasestorage.googleapis.com/v0/b/" +
            firebaseProjectID + "/o/" + serverFilePathEscaped + fileNameEscaped;

        // UNITYWEBREQUEST 
        // Post file to server
        // Define a filled in upload handler
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(fileBinary);
        uploadHandler.contentType = "application/force-download";
        // Empty downloadHandler since we are only posting
        DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        // Prepare post request with data
        UnityWebRequest webRequest = new UnityWebRequest(urlFirebase, "POST", downloadHandlerBuffer, uploadHandler);
        //UnityWebRequest webRequest = UnityWebRequest.Post(urlFirebase, fileText);
        //webRequest.SetRequestHeader("Content-type", "text/plain");
        // Send
        yield return webRequest.SendWebRequest();

        //// WWW
        //Dictionary<string, string> wwwHeaders = new Dictionary<string, string>();
        //wwwHeaders.Add("Content-type", "application/force-download");
        //WWW www = new WWW(urlFirebase, fileBinary, wwwHeaders);
        //yield return www;

        //// wait a frame
        //yield return null;

        // UNITYWEBREQUEST
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(webRequest.error);
        }

        //// WWW
        //if (www.error == null)
        //{
        //    Debug.Log("Upload of test file complete!");

        //}
        //else
        //{
        //    Debug.LogError(www.error);
        //}


        // wait a frame
        yield return null;

    }
}
