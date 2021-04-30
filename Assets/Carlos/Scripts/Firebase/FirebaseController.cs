using System;
using System.Collections.Generic;
using Dasync.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Extensions;
using InteractML;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Http;
using System.Threading;

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

    public bool authSuccess;

    /// <summary>
    /// Specifies the implementation to use when doing a REST upload
    /// </summary>
    public enum UploadRESTOptions { DotNetWebRequest, UnityWebRequest, WWW }

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
    public void UploadAsync(string localFilePath, string serverFilePath, bool useTasks = true)
    {
        if (fileSent)
        {
            Debug.LogError("Already uploaded to server, it won't upload again with the same instance (to avoid uploading twice when not needed)");
            //return;
        }

        // Get timestamp
        //string date = DateTime.Today.ToString("f");
        string date = DateTime.Now.ToLocalTime().ToString("s") + "/"; // prepared for path

        // Add timestampt to serverfilepath
        serverFilePath = Path.Combine(serverFilePath, date);

#if UNITY_ANDROID
        var uploadCoroutine = UploadREST(localFilePath, serverFilePath, UploadRESTOptions.DotNetWebRequest, useTasks);
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
    private IEnumerator UploadREST(string localFilePath, string serverFilePath, UploadRESTOptions options = UploadRESTOptions.DotNetWebRequest, bool useTasks = true)
    {
        if (string.IsNullOrEmpty(localFilePath))
        {
            Debug.LogError("No file specified but uploadFile called!");
            yield break;
        }

        // Wait for a few miliseconds, to allow the training examples to be fully written in disk
        yield return new WaitForSeconds(0.2f);

        if (useTasks)
        {
            Task.Run(async () => await UploadRESTAsync(localFilePath, serverFilePath, isDirectory: true));

            //Task.Factory.StartNew(() => UploadRESTAsync(localFilePath, serverFilePath, isDirectory: true), TaskCreationOptions.LongRunning);

            //Thread t = new Thread(async () => 
            //{
            //    Thread.CurrentThread.IsBackground = true;
            //    await UploadRESTAsync(localFilePath, serverFilePath, isDirectory: true);
            //});
            //t.Start();
            //Debug.Log("Thread started!");


            // wait a frame
            yield return null;
        }
        else
        {
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
                //Debug.Log("Attempting file upload...");
                StartCoroutine(UploadFileREST(localFilePath, serverFilePath, options));

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
                    StartCoroutine(UploadFileREST(file, serverFilePath, options));

                }
            }
        }

    }

    /// <summary>
    /// Upload files using REST API (all platforms, slower)
    /// </summary>
    /// <param name="localFilePath"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private async Task UploadRESTAsync(string localFilePath, string serverFilePath, bool isDirectory = false)
    {
        await Task.Run(async () => 
        {
            if (isDirectory && Directory.Exists(localFilePath))
            {
                //Debug.Log("Attempting directory upload...");
                await UploadFolderRESTAsync(localFilePath, serverFilePath);
            }
            else
            {
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
                    //Debug.Log("Attempting file upload...");
                    await UploadFileRESTAsync(localFilePath, serverFilePath);
                }
                else if (directoryDetected && Directory.Exists(localFilePath))
                {
                    //Debug.Log("Attempting directory upload...");

                    await UploadFolderRESTAsync(localFilePath, serverFilePath);
                }


            }
        });

    }

    /// <summary>
    /// Uploads a folder using REST API
    /// </summary>
    /// <param name="localFilePath"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private async Task UploadFolderRESTAsync(string localFilePath, string serverFilePath)
    {
        await Task.Run(async () => 
        {
            // First, find all the folders
            // Iterate to upload all files in folder, including subdirectories
            string[] files = Directory.GetFiles(localFilePath, "*", SearchOption.AllDirectories);
            await files.ParallelForEachAsync(async file =>
            {
                if (Path.GetExtension(file) != ".meta") 
                {
                    string folderName = new DirectoryInfo(@Path.GetDirectoryName(file)).Name + "/";
                    string auxServerFilePath = Path.Combine(serverFilePath, folderName);
                    await UploadFileRESTAsync(file, auxServerFilePath);
                }
            },
            maxDegreeOfParallelism: 5);

        });

    }

    /// <summary>
    /// Uploads a single file using REST API (all platforms, slower)
    /// </summary>
    /// <param name="fileToUpload"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private IEnumerator UploadFileREST(string fileToUpload, string serverFilePath, UploadRESTOptions options = UploadRESTOptions.DotNetWebRequest)
    {

        string fileName = Path.GetFileName(fileToUpload);
        string fileNameEscaped = System.Web.HttpUtility.UrlEncode(fileName);
        string serverFilePathEscaped = System.Web.HttpUtility.UrlEncode(serverFilePath);
        byte[] fileBinary = File.ReadAllBytes(fileToUpload);

        // HTTP
        string firebaseProjectID = "fir-test-b6418.appspot.com";
        string urlFirebase = "https://firebasestorage.googleapis.com/v0/b/" +
            firebaseProjectID + "/o/" + serverFilePathEscaped + fileNameEscaped;
        string contentType = "application/force-download";

        // wait a frame
        yield return null;

        // choose implementation
        switch (options)
        {
            case UploadRESTOptions.DotNetWebRequest:
                // C# WEBREQUEST
                WebRequest request = WebRequest.Create(urlFirebase);
                request.Method = "POST";
                request.ContentLength = fileBinary.Length;
                request.ContentType = contentType;
                request.Proxy = null; // this is known to speed up requests
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(fileBinary, 0, fileBinary.Length);
                dataStream.Close();
                // wait a frame
                yield return null;
                // Send request to server
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // wait a frame
                yield return null;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.LogError($"Upload failed! {response.StatusDescription}");
                }
                else
                    Debug.Log($"Upload succeeded! {response.StatusDescription}");
                // releases resources of response
                response.Close();


                break;


            case UploadRESTOptions.UnityWebRequest:
                // UNITYWEBREQUEST 
                // Post file to server
                // Define a filled in upload handler
                UploadHandlerRaw uploadHandler = new UploadHandlerRaw(fileBinary);
                uploadHandler.contentType = contentType;
                // Empty downloadHandler since we are only posting
                DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
                // Prepare post request with data
                UnityWebRequest webRequest = new UnityWebRequest(urlFirebase, "POST", downloadHandlerBuffer, uploadHandler);
                //UnityWebRequest webRequest = UnityWebRequest.Post(urlFirebase, fileText);
                //webRequest.SetRequestHeader("Content-type", "text/plain");
                // Send
                yield return webRequest.SendWebRequest();
                // errors?
                if (webRequest.result != UnityWebRequest.Result.Success)
                    Debug.LogError(webRequest.error);
                break;

            case UploadRESTOptions.WWW:
                // WWW
                Dictionary<string, string> wwwHeaders = new Dictionary<string, string>();
                wwwHeaders.Add("Content-type", contentType);
                WWW www = new WWW(urlFirebase, fileBinary, wwwHeaders);
                yield return www;
                // Any errors?
                if (www.error != null)
                    Debug.LogError(www.error);
                // wait a frame
                yield return null;
                break;
            default:
                break;
        }

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

    /// <summary>
    /// Runs the async task in a background thread pool (as per https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/july/async-programming-brownfield-async-development)
    /// </summary>
    /// <param name="fileToUpload"></param>
    /// <param name="serverFilePath"></param>
    private async Task UploadFileRESTAsync(string fileToUpload, string serverFilePath)
    {
        // Runs the async task in a background thread pool (as per https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/july/async-programming-brownfield-async-development)
        await UploadFileRESTDotNetWebRequestAsync(fileToUpload, serverFilePath);        

    }

    /// <summary>
    /// Uploads a single file using REST API (all platforms, slower)
    /// </summary>
    /// <param name="fileToUpload"></param>
    /// <param name="serverFilePath"></param>
    /// <returns></returns>
    private async Task UploadFileRESTDotNetWebRequestAsync(string fileToUpload, string serverFilePath, bool debug = true)
    {
        await Task.Run(async () =>
        {
            //Debug.Log("Attempting file upload...");
            string fileName = Path.GetFileName(fileToUpload);
            //string fileNameEscaped = System.Web.HttpUtility.UrlEncode(fileName); // THIS CAUSES THE SUDDENT STOP OF THE TASK ON OCULUS QUEST!! NEVER USE!!
            //string serverFilePathEscaped = System.Web.HttpUtility.UrlEncode(serverFilePath);
            string fileNameEscaped = UnityWebRequest.EscapeURL(fileName);
            string serverFilePathEscaped = UnityWebRequest.EscapeURL(serverFilePath);
            //Debug.Log("Name and url escaped for server upload");
            //byte[] fileBinary = File.ReadAllBytes(fileToUpload);
            byte[] fileBinary;

            //Debug.Log("Reading file...");
            // Async binary read 
            using (FileStream SourceStream = File.Open(fileToUpload, FileMode.Open))
            {
                fileBinary = new byte[SourceStream.Length];
                await SourceStream.ReadAsync(fileBinary, 0, (int)SourceStream.Length);
                SourceStream.Close();            
            }
            //Debug.Log("File read from disk, ready to upload...");

            // HTTP
            string firebaseProjectID = "fir-test-b6418.appspot.com";
            string urlFirebase = "https://firebasestorage.googleapis.com/v0/b/" +
                firebaseProjectID + "/o/" + serverFilePathEscaped + fileNameEscaped;
            string contentType = "application/force-download";

            // C# HTTPCLIENT
            //string jsonResponse;
            //using (var client = new HttpClient())
            //{
            //    HttpResponseMessage response = await client.PostAsync(urlFirebase, new ByteArrayContent(fileBinary));
            //    response.EnsureSuccessStatusCode();
            //    jsonResponse = await response.Content.ReadAsStringAsync(); // async sending as per https://stackoverflow.com/questions/65329127/unity-c-await-freeze-sync-process-which-should-be-executed-before-await-in-as
            //    Debug.Log(jsonResponse);
            //}

            // C# WEBREQUEST async ---> I never got it to work without freezing the main thread
            WebRequest request = WebRequest.CreateHttp(urlFirebase);
            request.Method = "POST";
            request.ContentLength = fileBinary.Length;
            request.ContentType = contentType;
            request.Proxy = null; // this is known to speed up requests
            Stream dataStream = await request.GetRequestStreamAsync();
            await dataStream.WriteAsync(fileBinary, 0, fileBinary.Length);
            dataStream.Close();
            //Debug.Log("Uploading...");
            // Send request to server
            // We need to use task factory since "getresponseasync" is actually synchronous (as per https://stackoverflow.com/questions/65329127/unity-c-await-freeze-sync-process-which-should-be-executed-before-await-in-as)
            await Task.Factory.FromAsync(request.BeginGetResponse,
                request.EndGetResponse, null).ContinueWith(task =>
                {
                    var response = (HttpWebResponse)task.Result;

                    if (debug)
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            Debug.LogError($"Upload failed! {response.StatusDescription}");
                        //else
                        //    Debug.Log($"Upload successful! {response.StatusDescription}");


                    }

                    // releases resources of response
                    response.Close();

                });

        });

    }
    
#endregion
}
