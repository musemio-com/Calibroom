using System;
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
    /// <summary>
    /// Instance firebase authentication class
    /// </summary>
    FirebaseAuth FAuth;
    /// <summary>
    /// Instance firebase cloud storage class
    /// </summary>
    FirebaseStorage FStorage;
    StorageReference FStorageRef;
    private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool isFirebaseInitialized = false;
    protected string MyStorageBucket = "gs://fir-test-b6418.appspot.com";
    // Currently enabled logging verbosity.
    protected Firebase.LogLevel logLevel = Firebase.LogLevel.Info;

    bool fileSent = false;


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
        Task<bool> resultDecryption = AESEncryptor.FileDecryptAsync(Application.dataPath + "/FirebaseUserCredentials.json.aes", Application.dataPath + "/FirebaseUserCredentialsDecrypted.json", "patata");
        while (resultDecryption.Result == false)
        {
            // it will exit this loop once result is true
        }
        // Read from file and delete
        string credentialsReadJson = File.ReadAllText(Application.dataPath + "/FirebaseUserCredentialsDecrypted.json");
        // Delete json file
        File.Delete(Application.dataPath + "/FirebaseUserCredentialsDecrypted.json");
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
            UploadFile(FStorage);
            fileSent = true;
        }
    }

    private void OnDestroy()
    {
        SignOutFirebase(FAuth);
    }


    protected void InitializeFirebase()
    {
        // Get ref to storage bucket address
        var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
        if (!String.IsNullOrEmpty(appBucket))
        {
            MyStorageBucket = String.Format("gs://{0}/", appBucket);
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

    private void UploadFile(FirebaseStorage storage)
    {
        if (storage == null)
        {
            return;
        }

        // Create a storage reference from our storage service
        StorageReference storageRef =
            storage.GetReferenceFromUrl(MyStorageBucket);


        // Upload a file on start
        // Locate file
        string localFilePath = Application.dataPath + "/testFile.json";
        if (File.Exists(localFilePath) && storageRef != null)
        {
            Debug.Log("Attempting file upload...");

            // Create a firebase reference to the file 
            StorageReference testFileRef = storageRef.Child("test/testFile.json");

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
    }

}
