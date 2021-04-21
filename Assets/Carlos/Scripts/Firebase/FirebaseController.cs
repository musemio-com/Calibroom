using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Storage;
using Firebase.Auth;
using Firebase.Extensions;

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
    public string CredentialsPath = "/Common/AuthCredentials/FirebaseUserCredentials.json.aes";

    /// <summary>
    /// Path of file or folder to upload
    /// </summary>
    public string PathToUpload;

    #endregion

    #region Unity Messages

    // Start is called before the first frame update
    void Start()
    {
        // Create sign in credentials for firebase
        //FirebaseUserCredentials credentials = new FirebaseUserCredentials("email", "password");

        //// Convert to json, and store in disk
        //string credentialsJson = JsonUtility.ToJson(credentials);
        //File.WriteAllText(Application.dataPath + "/FirebaseUserCredentials.json", credentialsJson);

        //// Encrypt json file
        //encryptor.FileEncryptAsync(Application.dataPath + "/FirebaseUserCredentials.json", "patata");

        // Decrypt json file into file
        string decryptedCredentialsFilePath = "/Common/AuthCredentials/FirebaseUserCredentialsDecrypted.json";
        Task<bool> resultDecryption = AESEncryptor.FileDecryptAsync(Application.dataPath + CredentialsPath, Application.dataPath + decryptedCredentialsFilePath, "patata");
        while (resultDecryption.Result == false)
        {
            // it will exit this loop once result is true
        }
        // Read from file and delete
        string credentialsReadJson = File.ReadAllText(Application.dataPath + decryptedCredentialsFilePath);
        // Delete json file
        File.Delete(Application.dataPath + decryptedCredentialsFilePath);
        // Parse into Json
        UserCredentials credentialsRead = JsonUtility.FromJson<UserCredentials>(credentialsReadJson);

        // One-off call to check and fix dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
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

    // Update is called once per frame
    void Update()
    {
        if (FAuth != null && FAuth.CurrentUser != null && !fileSent)
        {
            UploadFile(FStorage, PathToUpload);
            fileSent = true;
        }
    }

    private void OnDestroy()
    {
        SignOutFirebase(FAuth);
    }

    #endregion


    #region Private Methods

    protected void InitializeFirebase()
    {
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
    }

    private void SignInFirebase(FirebaseAuth auth, string email, string password)
    {
        if (auth == null)
        {
            return;
        }

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
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
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);

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

    private void UploadFile(FirebaseStorage storage, string filePath)
    {
        if (storage == null)
        {
            Debug.LogError("Firebase Storage couldn't be initalized");
            return;
        }
        if (System.String.IsNullOrEmpty(filePath))
        {
            Debug.LogError("No file specified but uploadFile called!");
            return;
        }

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl(StorageBucket);

        string localFilePath = Application.dataPath + filePath;

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
            Debug.Log("Attempting file upload...");

            // Create a firebase reference to the file 
            StorageReference testFileRef = storageRef.Child("test/" + filePath);

            // Upload the file to the path "images/rivers.jpg"
            testFileRef.PutFileAsync(localFilePath)
                .ContinueWith((Task<StorageMetadata> task) => {
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
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);
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
                StorageReference testFileRef = storageRef.Child("test/" + file);

                // Upload the file to the path "images/rivers.jpg"
                testFileRef.PutFileAsync(file)
                    .ContinueWith((Task<StorageMetadata> task) => {
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

    #endregion


}
