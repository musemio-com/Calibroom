using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MECM;
using InteractML;
using UnityEngine.SceneManagement;
public class CalibrationMethodManager : MonoBehaviour
{
    [Header("User to collect Data From")]
    public int UserID;
    [Header("Right Hand - Left Hand - HMD")]
    public TrackedObjects trackedObjects;
    public bool UploadToFirebase;
    [DrawIf("UploadToFirebase",true)]
    public FirebaseCredentialsObject FirebaseCredentials;
    public void InitializeMECM()
    {
        DataCollectionController DTC = FindObjectOfType<DataCollectionController>();
        IMLComponent ImlComponent = FindObjectOfType<IMLComponent>();

        FindObjectOfType<UserDetailsController>().UserData.UserID = UserID;
        DTC.UserIDInt = UserID;
        DTC.UserIDString = "" + FindObjectOfType<UserDetailsController>().LoadUserID() + "/" + SceneManager.GetActiveScene().name;
        DTC.RightHandGrabbingExtractor = trackedObjects.RightHand.AddComponent<GrabbingPieceFeatureExtractor>();
        DTC.LeftHandGrabbingExtractor = trackedObjects.LeftHand.AddComponent<GrabbingPieceFeatureExtractor>();
        ImlComponent.GameObjectsToUse.Clear();
        ImlComponent.GameObjectsToUse.Insert(0,trackedObjects.HMD);
        ImlComponent.GameObjectsToUse.Insert(1,trackedObjects.RightHand);
        ImlComponent.GameObjectsToUse.Insert(2,trackedObjects.LeftHand);

        //SetupFirebaseCredentials();
    }
    //void SetupFirebaseCredentials()
    //{
    //    FirebaseController fbController = FindObjectOfType<FirebaseController>();
    //    fbController.StorageBucket = FirebaseCredentials.StorageBucket;
    //    fbController.CredentialsPath = FirebaseCredentials.CredentialsPath;
    //    fbController.PathToUpload = FirebaseCredentials.PathToUpload;
    //}
}

[Serializable]
public class FirebaseCredentialsObject
{
    public string StorageBucket;
    public string CredentialsPath;
    public string PathToUpload;
}

[Serializable]
public class TrackedObjects
{
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject HMD;
}
