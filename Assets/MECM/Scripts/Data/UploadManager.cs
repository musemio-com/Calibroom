using System.Collections;
using System.Collections.Generic;
using Dasync.Collections;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System;
using UnityEngine.Networking;

namespace MECM
{
public class UploadManager : MonoBehaviour
{
    public static UploadManager Instance;

    DashboardRefs uploadInfo;
    bool fileSent = false;
    string PathToUpload;
    public enum UploadRESTOptions { DotNetWebRequest, UnityWebRequest, WWW }

    private void Awake()
    {
        Instance = this;
    }
    private void Start() {
            uploadInfo = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
            Debug.Log(uploadInfo.uploadSettings.firebaseStorageID);
    }
    public void UploadToServer(string localFilePath, string serverFilePath, bool useTasks = true)
    {
            if (fileSent)
            {
                Debug.LogError("Already uploaded to server, it won't upload again with the same instance (to avoid uploading twice when not needed)");
                //return;
            }

            string date = DateTime.Now.ToLocalTime().ToString("s") + "/"; // prepared for path

            serverFilePath = Path.Combine(serverFilePath, date);

            var uploadCoroutine = UploadREST(localFilePath, serverFilePath, UploadRESTOptions.DotNetWebRequest, useTasks);
            StartCoroutine(uploadCoroutine);
            fileSent = true;
    }
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

                if (fileDetected && File.Exists(localFilePath))
                {
                    //Debug.Log("Attempting file upload...");
                    StartCoroutine(UploadFileREST(localFilePath, serverFilePath, options));

                }
                else if (directoryDetected && Directory.Exists(localFilePath))
                {
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
    private IEnumerator UploadFileREST(string fileToUpload, string serverFilePath, UploadRESTOptions options = UploadRESTOptions.DotNetWebRequest)
    {

            string fileName = Path.GetFileName(fileToUpload);
            string fileNameEscaped = System.Web.HttpUtility.UrlEncode(fileName);
            string serverFilePathEscaped = System.Web.HttpUtility.UrlEncode(serverFilePath);
            byte[] fileBinary = File.ReadAllBytes(fileToUpload);
            
            // HTTP
            //string firebaseProjectID = "fir-test-b6418.appspot.com"; // this'll be retrieved from a scriptable object
            string firebaseProjectID = uploadInfo.uploadSettings.firebaseStorageID;
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
    private async Task UploadFileRESTAsync(string fileToUpload, string serverFilePath)
    {
            // Runs the async task in a background thread pool (as per https://docs.microsoft.com/en-us/archive/msdn-magazine/2015/july/async-programming-brownfield-async-development)
            await UploadFileRESTDotNetWebRequestAsync(fileToUpload, serverFilePath);
    }
    private async Task UploadFileRESTDotNetWebRequestAsync(string fileToUpload, string serverFilePath, bool debug = true)
    {
            await Task.Run(async () =>
            {
            //Debug.Log("Attempting file upload...");
            string fileName = Path.GetFileName(fileToUpload);
            string fileNameEscaped = UnityWebRequest.EscapeURL(fileName);
            string serverFilePathEscaped = UnityWebRequest.EscapeURL(serverFilePath);
            byte[] fileBinary;

            // Async binary read 
            using (FileStream SourceStream = File.Open(fileToUpload, FileMode.Open))
                {
                    fileBinary = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(fileBinary, 0, (int)SourceStream.Length);
                    SourceStream.Close();
                }
            //Debug.Log("File read from disk, ready to upload...");

            // HTTP
            // string firebaseProjectID = "fir-test-b6418.appspot.com";
                string firebaseProjectID = uploadInfo.uploadSettings.firebaseStorageID;
                string urlFirebase = "https://firebasestorage.googleapis.com/v0/b/" +
                    firebaseProjectID + "/o/" + serverFilePathEscaped + fileNameEscaped;
                string contentType = "application/force-download";

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

}

}

