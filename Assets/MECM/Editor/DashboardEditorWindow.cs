using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MECM;
using UnityEngine.XR.Interaction.Toolkit;
using InteractML;
public class DashboardEditorWindow : EditorWindow
{
    int userID;
    GameObject LeftController;
    GameObject RightController;
    GameObject HMD;
    GameObject customTrackingTriggerObject;
    AnimBool StartTrackingWhenSceneActive;
    GameObject StartCollectingObject;
    AnimBool customTrackingTrigger;
    AnimBool SpawnStartEndObjects;
    AnimBool UploadData;
    AnimBool R_ExposeParameters;
    AnimBool L_ExposeParameters;
    AnimBool HMD_ExposeParameters;



    string FirebaseID;
    float taskCompletionTime;
    bool UploadWhenCollectingDone;

    [MenuItem("MECM/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow),false,"Calibration Method");
    }
    private void OnEnable()
    {
        R_ExposeParameters = new AnimBool(false);
        R_ExposeParameters.valueChanged.AddListener(Repaint);

        L_ExposeParameters = new AnimBool(false);
        L_ExposeParameters.valueChanged.AddListener(Repaint);

        HMD_ExposeParameters = new AnimBool(false);
        HMD_ExposeParameters.valueChanged.AddListener(Repaint);

        StartTrackingWhenSceneActive = new AnimBool(false);
        StartTrackingWhenSceneActive.valueChanged.AddListener(Repaint);

        UploadData = new AnimBool(false);
        UploadData.valueChanged.AddListener(Repaint);
        
        SpawnStartEndObjects = new AnimBool(false);
        SpawnStartEndObjects.valueChanged.AddListener(Repaint);

    }
    private void OnGUI()
    {
        GUILayout.Label("Calibration Dashboard", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("User ID", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        userID = EditorGUILayout.IntField("User ID", userID);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Controllers To Track",EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        RightController = EditorGUILayout.ObjectField("Right Controller", RightController, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Settings"))
        {
            RightHandSettingsEditorWindow.ShowWindow();
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        LeftController = EditorGUILayout.ObjectField("Left Controller", LeftController, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Settings"))
        {
            LefttHandSettingsEditorWindow.ShowWindow();
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        HMD = EditorGUILayout.ObjectField("Head Mounted Display", HMD, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Settings"))
        {
            HMDSettingsEditorWindow.ShowWindow();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Tracking Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        StartTrackingWhenSceneActive.target = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", StartTrackingWhenSceneActive.target);
        if (EditorGUILayout.BeginFadeGroup(StartTrackingWhenSceneActive.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call FireToggleCollectDataEvent method in DataCollectionController", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        SpawnStartEndObjects.target = EditorGUILayout.ToggleLeft("Use GameObjects to trigger manually", SpawnStartEndObjects.target);
        if (EditorGUILayout.BeginFadeGroup(SpawnStartEndObjects.faded))
        {
            EditorGUI.indentLevel++;
            StartCollectingObject = EditorGUILayout.ObjectField("Start Collecting", StartCollectingObject, typeof(GameObject), true) as GameObject;
            EditorGUILayout.HelpBox("Grab the same object to stop collecting data", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        GUILayout.Label("Upload Data to Server", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        UploadData.target = EditorGUILayout.ToggleLeft("Upload to Firebase Cloud Storage", UploadData.target);
        if (EditorGUILayout.BeginFadeGroup(UploadData.faded))
        {
            EditorGUI.indentLevel++;
                FirebaseID = EditorGUILayout.TextField("Firebase Project ID", FirebaseID);
                // UploadWhenCollectingDone = EditorGUILayout.ToggleLeft("Upload when tracking stops",UploadWhenCollectingDone);
                EditorGUILayout.HelpBox("Using REST API, Slow but supported on all platforms", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUI.indentLevel--;
        GUILayout.Label("Task Completion Time", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        taskCompletionTime = EditorGUILayout.FloatField("time", taskCompletionTime);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        if (GUILayout.Button("Setup"))
        {
            setupUserID();
            setupSystems();
            setupControllers();
            setupDataCollectionSettings();
            setupUploadSettings();
        }
    }


    void setupSystems()
    {
        GameObject DataController = Instantiate(Resources.Load<GameObject>("DataCollection"));;
        GameObject IMLSystem = Instantiate(Resources.Load<GameObject>("IML System"));
        DataController.name = "DataCollection";
        IMLSystem.name = "IML System";
        IMLSystem.GetComponent<IMLComponent>().enabled = true;
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(RightController);
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(LeftController);
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(HMD);

        IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData[0].GameComponent = DataController.GetComponent<DataCollectionController>();

        
        
    }

    void setupControllers()
    {
        RightController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.RightHand;
        LeftController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.LeftHand;
    }
    void setupUserID()
    {
        UserDetails userInfo = Resources.Load<UserDetails>("UserDetailsObject");
        if(userInfo == null)
        {
           userInfo = ScriptableObject.CreateInstance<UserDetails>();
           AssetDatabase.CreateAsset(userInfo,"Assets/MECM/Resources/UserDetailsObject.asset");
           AssetDatabase.SaveAssets();
        }
        userInfo.UserID = userID;
    }
    void setupDataCollectionSettings()
    {
        if(StartTrackingWhenSceneActive.target)
        {
            TrackersInfoScriptableObject trackersInfo = Resources.Load<TrackersInfoScriptableObject>("TrackersInfoObject");
            if(trackersInfo == null)
            {
               trackersInfo = ScriptableObject.CreateInstance<TrackersInfoScriptableObject>();
               AssetDatabase.CreateAsset(trackersInfo,"Assets/MECM/Resources/TrackersInfoObject.asset");
               AssetDatabase.SaveAssets();
            }
            trackersInfo.StartTrackingOnSceneStart = StartTrackingWhenSceneActive.target;
        }

        if(SpawnStartEndObjects.target && StartCollectingObject != null)
        {
            DataCollectionController dataCollectionController = FindObjectOfType<DataCollectionController>();
            // StartCollectingObject.GetComponent<XRGrabInteractable>().onSelectEnter.AddListener

        }
    }
    void setupUploadSettings()
    {
        UploadInfoScriptableObject uploadInfo = Resources.Load<UploadInfoScriptableObject>("UploadInfoObject");
        if(uploadInfo == null)
        {
           uploadInfo = ScriptableObject.CreateInstance<UploadInfoScriptableObject>();
           AssetDatabase.CreateAsset(uploadInfo,"Assets/MECM/Resources/UploadInfoObject.asset");
           AssetDatabase.SaveAssets();
        }
        uploadInfo.UploadEnabled = UploadData.target;
        uploadInfo.FirebaseID = FirebaseID; 

        if(UploadData.target)
        {
            GameObject _uploadManager = Instantiate(Resources.Load<GameObject>("UploadManager"));
            _uploadManager.name = "UploadManager";
        }
    }

}
