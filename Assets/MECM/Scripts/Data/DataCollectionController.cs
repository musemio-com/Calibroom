﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace MECM
{
    /// <summary>
    /// Controls when data starts being collected and when it stops being collected
    /// </summary>
    //[ExecuteAlways]
    public class DataCollectionController : MonoBehaviour
    {

        #region Variables

        /// <summary>
        /// Is the class collecting data?
        /// </summary>
        public bool CollectingData;

        /// <summary>
        /// Events that fires the toggle data collection on/off (used outside of IMLGraph)
        /// </summary>
        //[SerializeField]
        //private bool m_ToggleCollectDataEvent;

        /// <summary>
        /// Toggles data collection on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleDataCollection;


        /// <summary>
        /// Events that fires the toggle run model on/off (used outside of IMLGraph)
        /// </summary>
        //[SerializeField]
        private bool m_ToggleTrainModelEvent;
        /// <summary>
        /// Toggles run model on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleTrainModel;


        /// <summary>
        /// Events that fires the toggle run model on/off (used outside of IMLGraph)
        /// </summary>
        //[SerializeField]
        private bool m_ToggleRunModelEvent;
        /// <summary>
        /// Toggles run model on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleRunModel;

        /// <summary>
        /// User ID that we are collecting data from
        /// </summary>
        [SendToIMLGraph,HideInInspector]
        public int UserIDInt;

        /// <summary>
        /// Contains the ID + Scene as a directory where to store data
        /// </summary>
        [SendToIMLGraph,HideInInspector]
        public string UserIDString;

    #region Right Hand Settings
    [SendToIMLGraph,HideInInspector]
    public bool _R_UsePosition;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UsePostionVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UsePositionAcceleration;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UseRotation;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UseRotationVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UseRotationAcceleration;

    [SendToIMLGraph,HideInInspector]
    public bool _R_UseGrabbedObject;
    #endregion

    #region Left Hand Settings
    [SendToIMLGraph,HideInInspector]
    public bool _L_UsePosition;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UsePostionVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UsePositionAcceleration;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UseRotation;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UseRotationVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UseRotationAcceleration;

    [SendToIMLGraph,HideInInspector]
    public bool _L_UseGrabbedObject;
    #endregion

    #region HMD Settings
    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UsePosition;

    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UsePostionVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UsePositionAcceleration;

    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UseRotation;

    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UseRotationVelocity;

    [SendToIMLGraph,HideInInspector]
    public bool _HMD_UseRotationAcceleration;
    #endregion

        /// <summary>
        /// Used to interface with the user details SO (get persistent userID)
        /// </summary>
        // private UserDetailsController m_UserDetailsCtrlr;
        private DashboardRefs dashboardRefs;

        /// <summary>
        /// Handles uploads to firebase server
        /// </summary>
        // private FirebaseController m_FirebaseController;
        private UploadManager _UploadManager;

        /// <summary>
        /// Extractor for grabbing info left hand
        /// </summary>
        public GrabbingPieceFeatureExtractor LeftHandGrabbingExtractor;
        /// <summary>
        /// Extractor for grabbing info right hand
        /// </summary>
        public GrabbingPieceFeatureExtractor RightHandGrabbingExtractor;

        /// <summary>
        /// Is the left hand grabbing a piece?
        /// </summary>
        [SendToIMLGraph]
        public bool LeftHandGrabbing;
        /// <summary>
        /// Is the right hand grabbing a piece?
        /// </summary>
        [SendToIMLGraph]
        public bool RightHandGrabbing;

        /// <summary>
        /// Upload data after collecting?
        /// </summary>
        //[SerializeField, Header("Upload Options")]
        private bool m_UploadData = false;
        [SerializeField]
        private bool m_UseTasksOnUpload = true;

        private string FirebaseProjectID;


        #endregion

        #region Unity Messages
        // Called before start
        private void Awake()
        {
            // Load UserID (used in graph for data storage and identification of training set)
            // m_UserDetailsCtrlr = FindObjectOfType<UserDetailsController>();
            dashboardRefs = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
            if (dashboardRefs != null)
            {
                UserIDInt = dashboardRefs.userID;
                UserIDString = UserIDInt.ToString() + "/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                m_UploadData = dashboardRefs.uploadSettings.enable;
                FirebaseProjectID = dashboardRefs.uploadSettings.firebaseStorageID;

                CollectingData = dashboardRefs.trackingSettings.TrackOnSceneStart;
                // UserIDInt = userDetails.UserID;
                // We store the path to the directory where to store data here (each teach the machine node reads this value in the IML graph)
                // UserIDString = UserIDInt.ToString() + "/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
            // Get reference to firebase controller
            // m_FirebaseController = FindObjectOfType<FirebaseController>();
            _UploadManager = FindObjectOfType<UploadManager>();

            // Get reference to grabbing piece feature extractors and assign them to correct hands
            var grabbingExtractors = FindObjectsOfType<GrabbingPieceFeatureExtractor>();
            if (grabbingExtractors != null && grabbingExtractors.Length > 0)
            {
                // Assign leftHand and rightHand extractors
                foreach (var grabbingExtractor in grabbingExtractors)
                {
                    if (grabbingExtractor.XRControllerType == GrabbingPieceFeatureExtractor.ControllerType.LeftHand)
                        LeftHandGrabbingExtractor = grabbingExtractor;
                    else if (grabbingExtractor.XRControllerType == GrabbingPieceFeatureExtractor.ControllerType.RightHand)
                        RightHandGrabbingExtractor = grabbingExtractor;
                }
            }
        }

        // Called once in first frame
        private void Start()
        {
            // Make sure to delete all training examples, but don't delete from disk to avoid data loss
            IMLEventDispatcher.DeleteAllTrainingExamplesInGraphCallback(deleteFromDisk: false);
        }

        // Update is called once per frame
        void Update()
        {
            // If we reach to this script with the toggle true, make sure to restart it
            //if (ToggleDataCollection)
            //{
            //    ToggleDataCollection = false;
            //}
            if (ToggleTrainModel)
            {
                ToggleTrainModel = false;
            }
            if (ToggleRunModel)
            {
                ToggleRunModel = false;
            }

            // If the event was fired, flag data collection toggle to true
            if (CollectingData)//if (m_ToggleCollectDataEvent || ToggleDataCollection)
            {
                ToggleDataCollection = true;
                //ToggleDataCollection = true;
                //m_ToggleCollectDataEvent = false;
                

                // Update whether we are starting or stopping collecting data
                //CollectingData = !CollectingData;
                // If we have stopped collecting data and we need to upload data...
                //if (!CollectingData && m_UploadData)
                //{
                //    Debug.Log("UPLOADING...");
                //    string userDataSetPath = IMLDataSerialization.GetTrainingExamplesDataPath() + "/" + UserIDString;
                //    // Upload files from our IDString directory to firebase server
                //    //m_FirebaseController.UploadAsync(userDataSetPath, UserIDString + "/", useTasks: m_UseTasksOnUpload);
                //    _UploadManager.UploadToServer(userDataSetPath, UserIDString + "/", useTasks: m_UseTasksOnUpload);
                //}
            }
            if (!CollectingData)
                ToggleDataCollection = false;
                

            if (!CollectingData && m_UploadData)
            {
                Debug.Log("UPLOADING...");
                string userDataSetPath = IMLDataSerialization.GetTrainingExamplesDataPath() + "/" + UserIDString;
                // Upload files from our IDString directory to firebase server
                //m_FirebaseController.UploadAsync(userDataSetPath, UserIDString + "/", useTasks: m_UseTasksOnUpload);
                _UploadManager.UploadToServer(userDataSetPath, UserIDString + "/", useTasks: m_UseTasksOnUpload);
            }
            if (m_ToggleTrainModelEvent || ToggleTrainModel)
            {
                ToggleTrainModel = true;
                m_ToggleTrainModelEvent = false;
                Debug.Log("Toggling Train Model!");
            }
            if (m_ToggleRunModelEvent || ToggleRunModel)
            {
                ToggleRunModel = true;
                m_ToggleRunModelEvent = false;
                Debug.Log("Toggling Run Model!");
            }

            // Pull grabbing feature extractors
            if (LeftHandGrabbingExtractor != null)
                LeftHandGrabbing = LeftHandGrabbingExtractor.GrabbingPiece;
            if (RightHandGrabbingExtractor != null)
                RightHandGrabbing = RightHandGrabbingExtractor.GrabbingPiece;
        }
        void OnDisable()
        {
            // Make sure to stop data collection if the data controller gets disabled
            if (CollectingData)
                StopCollectingData();//FireToggleCollectDataEvent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fires the collect data event (toggles on/off collecting data)
        /// </summary>
        /// <returns></returns>
        public void FireToggleCollectDataEvent()
        {
            //m_ToggleCollectDataEvent = true;
        }

        public void StartCollectingData()
        {
            CollectingData = true;
            Debug.Log("Starting data collection!");
        }
        public void StopCollectingData()
        {
            CollectingData = false;
            Debug.Log("Stopping data collection!");
        }
        /// <summary>
        /// Fires the Train model event (toggles on/off model inference)
        /// </summary>
        /// <returns></returns>
        public void FireToggleTrainModelEvent()
        {
            m_ToggleTrainModelEvent = true;
        }

        /// <summary>
        /// Fires the run model event (toggles on/off model inference)
        /// </summary>
        /// <returns></returns>
        public void FireToggleRunModelEvent()
        {
            m_ToggleRunModelEvent = true;
        }


        #endregion

    }

}