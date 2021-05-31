using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MECM;
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

    bool R_UsePos;
    bool R_UseRot;
    bool R_UseObjs;

    bool L_UsePos;
    bool L_UseRot;
    bool L_UseObjs;

    bool HMD_UsePos;
    bool HMD_UseRot;
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

        EditorGUILayout.BeginVertical();
        RightController = EditorGUILayout.ObjectField("Right Controller", RightController, typeof(GameObject), true) as GameObject;
        if(RightController != null)
        {    
            R_ExposeParameters.target = true;
            if(EditorGUILayout.BeginFadeGroup(R_ExposeParameters.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();
                R_UsePos = EditorGUILayout.ToggleLeft("Use Position",R_UsePos);
                R_UseRot = EditorGUILayout.ToggleLeft("Use Rotaion",R_UseRot);
                R_UseObjs = EditorGUILayout.ToggleLeft("Use Grabbed Objects",R_UseObjs);  
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup(); 
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        LeftController = EditorGUILayout.ObjectField("Left Controller", LeftController, typeof(GameObject), true) as GameObject;
        if(LeftController != null)
        {    
            L_ExposeParameters.target = true;
            if(EditorGUILayout.BeginFadeGroup(L_ExposeParameters.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();
                L_UsePos = EditorGUILayout.ToggleLeft("Use Position",L_UsePos);
                L_UseRot = EditorGUILayout.ToggleLeft("Use Rotaion",L_UseRot);
                L_UseObjs = EditorGUILayout.ToggleLeft("Use Grabbed Objects",L_UseObjs);  
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup(); 
        }
        EditorGUILayout.EndVertical();


        EditorGUILayout.BeginVertical();
        HMD = EditorGUILayout.ObjectField("Head Mounted Display", HMD, typeof(GameObject), true) as GameObject;
        if(HMD != null)
        {    
            HMD_ExposeParameters.target = true;
            if(EditorGUILayout.BeginFadeGroup(HMD_ExposeParameters.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical();
                HMD_UsePos = EditorGUILayout.ToggleLeft("Use Position",HMD_UsePos);
                HMD_UseRot = EditorGUILayout.ToggleLeft("Use Rotaion",HMD_UseRot);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup(); 
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Tracking Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        StartTrackingWhenSceneActive.target = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", StartTrackingWhenSceneActive.target);
        if (EditorGUILayout.BeginFadeGroup(StartTrackingWhenSceneActive.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call StopTracking method", MessageType.Info);
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
                UploadWhenCollectingDone = EditorGUILayout.ToggleLeft("Upload when tracking stops",UploadWhenCollectingDone);
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
            setupUploadSettings();
        }
    }


    void setupSystems()
    {
        Resources.Load("DataCollection.prefab");
        Resources.Load("IML System.prefab");
    }

    void setupControllerTrackers()
    {
        HMD.AddComponent<GrabbingPieceFeatureExtractor>();
        RightController.AddComponent<GrabbingPieceFeatureExtractor>();
        LeftController.AddComponent<GrabbingPieceFeatureExtractor>();

        TrackersInfoScriptableObject trackersInfo = ScriptableObject.CreateInstance<TrackersInfoScriptableObject>();



        //Spawn DataCollector & IMLComponent Gameobjects
        // add grabbing extractor to both scripts
        // set a custom attribute to force choosing which is left and which is right.
        // check the enabled fields of each controller and set em up in the data collection controller (to be sent to IML Graph)
        // Send the controllers to the IML Component

    }
    void setupUserID()
    {
        UserDetails userInfo = ScriptableObject.CreateInstance<UserDetails>();
        userInfo.UserID = userID;
        AssetDatabase.CreateAsset(userInfo,"Assets/MECM/Resources/UserDetailsObject.asset");
        AssetDatabase.SaveAssets();
    }
    void setupDataCollectionSettings()
    {
        //setup how datacollector can start collecting when scene Start
        // Setup custom game objects to start/stop Tracking
    }
    void setupUploadSettings()
    {
        UploadInfoScriptableObject uploadInfo = ScriptableObject.CreateInstance<UploadInfoScriptableObject>();
        uploadInfo.UploadOnTrackingDone = UploadWhenCollectingDone;
        uploadInfo.FirebaseID = FirebaseID;
        uploadInfo.UploadEnabled = UploadData.target;
        AssetDatabase.CreateAsset(uploadInfo,"Assets/MECM/Resources/UploadInfoObject.asset");
        AssetDatabase.SaveAssets();
    }

}
