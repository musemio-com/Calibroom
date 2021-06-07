using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MECM;
using UnityEngine.XR.Interaction.Toolkit;
using InteractML;
using XNode;
using System.Linq;
using Unity.EditorCoroutines.Editor;

public class DashboardEditorWindow : EditorWindow
{
    int userID;
    GameObject LeftController;
    GameObject RightController;
    GameObject HMD;
    AnimBool trackOnSceneStart;
    AnimBool TriggerTrackWithObjs;
    GameObject StartTrackObject;
    GameObject StopTrackObject;

    AnimBool UploadData;



    string OverallScore = "Overal Score";
    string VisuoSpatial = "Visuo-Spatial";
    string SpeedProcessing = "Speed Processing";
    string CycleTime = "Cycle Time";



    string FirebaseID;
    float taskCompletionTime;
    bool UploadWhenCollectingDone;

    static DashboardRefs _ref;

    [MenuItem("MECM/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow),false,"Calibration Method");
    }
    private void OnEnable()
    {   
        trackOnSceneStart = new AnimBool(false);
        trackOnSceneStart.valueChanged.AddListener(Repaint);

        UploadData = new AnimBool(false);
        UploadData.valueChanged.AddListener(Repaint);
        
        TriggerTrackWithObjs = new AnimBool(false);
        TriggerTrackWithObjs.valueChanged.AddListener(Repaint);

        loadAllRefs();
    }

    void loadAllRefs()
    {
        _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
        if(_ref == null)
           return;
        userID = _ref.userID;
        RightController = _ref.rightHandController.ControllerRef;
        LeftController = _ref.leftHandController.ControllerRef;
        HMD = _ref.headMountedDisplay.ControllerRef;
        trackOnSceneStart= new AnimBool(_ref.trackingSettings.TrackOnSceneStart);
        TriggerTrackWithObjs = new AnimBool(_ref.trackingSettings.TrackTriggerManually.enable);
        StartTrackObject = _ref.trackingSettings.TrackTriggerManually.StartTracking;
        StopTrackObject = _ref.trackingSettings.TrackTriggerManually.StopTracking;
        UploadData = new AnimBool(_ref.uploadSettings.enable);
        FirebaseID = _ref.uploadSettings.firebaseStorageID;


    }
    private void OnDisable()
    {
        //save editor refs in editor prefs    
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
        HMD = EditorGUILayout.ObjectField("Head Mounted Display", HMD, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Settings"))
        {
            HMDSettingsEditorWindow.ShowWindow();
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
        RightController = EditorGUILayout.ObjectField("Right Controller", RightController, typeof(GameObject), true) as GameObject;
        if(GUILayout.Button("Settings"))
        {
            RightHandSettingsEditorWindow.ShowWindow();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Tracking Settings", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        trackOnSceneStart.target = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", trackOnSceneStart.target);
        if (EditorGUILayout.BeginFadeGroup(trackOnSceneStart.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call FireToggleCollectDataEvent method in DataCollectionController", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        TriggerTrackWithObjs.target = EditorGUILayout.ToggleLeft("Use GameObjects to trigger manually", TriggerTrackWithObjs.target);
        if (EditorGUILayout.BeginFadeGroup(TriggerTrackWithObjs.faded))
        {
            EditorGUI.indentLevel++;
            StartTrackObject = EditorGUILayout.ObjectField("Start Collecting", StartTrackObject, typeof(GameObject), true) as GameObject;
            StopTrackObject = EditorGUILayout.ObjectField("Stop Collecting", StopTrackObject, typeof(GameObject), true) as GameObject;
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
        if (GUILayout.Button("Setup") && (RightController != null && LeftController != null && HMD != null))
        {
            Setup();        
        }
        if(RightController == null)
        {
            EditorGUILayout.HelpBox("Right Controller is missing",MessageType.Warning);
        }
        if(LeftController == null)
        {
            EditorGUILayout.HelpBox("Left Controller is missing",MessageType.Warning);
        }
        if(HMD == null)
        {
            EditorGUILayout.HelpBox("HMD Controller is missing",MessageType.Warning);
        }
        EditorGUILayout.Space();
        GUILayout.Label("OUTPUT", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical();
        if(GUILayout.Button(OverallScore))
        {
            OverallScore = "Overall Score: " + Random.Range(0,1000);
        }
        if(GUILayout.Button(VisuoSpatial))
        {
            VisuoSpatial = "Visuo-Spatial: " + Random.Range(0,1000);
        }
        if(GUILayout.Button(SpeedProcessing))
        {
            SpeedProcessing = "Speed Processing: " + Random.Range(0,1000);
        }
        if(GUILayout.Button(CycleTime))
        {
            CycleTime = "Cycle Time: " + Random.Range(0,1000);
        }
        EditorGUILayout.EndVertical();

        EditorGUI.indentLevel--;
    }
    void Setup()
    {
        if(FindObjectOfType<DataCollectionController>())
            DestroyImmediate(FindObjectOfType<DataCollectionController>().gameObject);
        if(FindObjectOfType<IMLComponent>())
            DestroyImmediate(FindObjectOfType<IMLComponent>().gameObject);


        EditorCoroutine edCo = EditorCoroutineUtility.StartCoroutine(SetupPorts(),this);

        RightController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.RightHand;
        LeftController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.LeftHand;

        if(TriggerTrackWithObjs.target)
        {
            // StartTrackObject.GetComponent<XRGrabInteractable>().onSelectEnter.AddListener(x=> DataController.GetComponent<DataCollectionController>().FireToggleCollectDataEvent());
            // StopTrackObject.GetComponent<XRGrabInteractable>().onSelectEnter.AddListener(x=> DataController.GetComponent<DataCollectionController>().FireToggleCollectDataEvent());
        }
        if(UploadData.target)
        {
            GameObject _uploadManager = Instantiate(Resources.Load<GameObject>("Prefabs/UploadManager"));
            _uploadManager.name = "UploadManager";
        }
        //SaveRefs();
    }
    void SaveRefs()
    {
        _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRef");
        if(_ref == null)
        {
            _ref = ScriptableObject.CreateInstance<DashboardRefs>();
            AssetDatabase.CreateAsset(_ref,"Assets/MECM/Resources/ScriptableObjects/DashboardRefs.asset");
            AssetDatabase.SaveAssets();
        }
        _ref.userID = userID;


        _ref.rightHandController.ControllerRef = RightController;
        _ref.leftHandController.ControllerRef = LeftController;
        _ref.headMountedDisplay.ControllerRef = HMD;

        _ref.trackingSettings.TrackOnSceneStart = trackOnSceneStart.target;
        _ref.trackingSettings.TrackTriggerManually.enable = TriggerTrackWithObjs.target;
        _ref.trackingSettings.TrackTriggerManually.StartTracking = StartTrackObject;
        _ref.trackingSettings.TrackTriggerManually.StopTracking = StopTrackObject;

        _ref.uploadSettings.enable = UploadData.target;
        _ref.uploadSettings.firebaseStorageID = FirebaseID;
    }
    IEnumerator SetupPorts()
    {
        GameObject DataController = Instantiate(Resources.Load<GameObject>("Prefabs/DataCollection"));;
        GameObject IMLSystem = Instantiate(Resources.Load<GameObject>("Prefabs/IML System"));
        DataController.name = "DataCollection";
        IMLSystem.name = "IML System";

        // IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData[0].GameComponent = DataController.GetComponent<DataCollectionController>();

        yield return new WaitForSeconds(2f);

        var IMLGraph = IMLSystem.GetComponent<IMLComponent>().graph;


        ScriptNode dataNode = new ScriptNode();

        GameObjectNode _CamNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(HMD);
        _CamNode.position.x = -504;
        _CamNode.position.y = -1224;
        var _CamOutPort = _CamNode.GetPort("GameObjectDataOut");
        var mainCameraPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "82e72311-ad4c-4813-a73c-f7a92fd550de");
        var mainCameraRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "7159019a-48c4-4529-8008-868e19386616");
        var _CamRotationPort = mainCameraRotationNode.GetPort("GameObjectDataIn");
        var _CamPositionPort = mainCameraPositionNode.GetPort("GameObjectDataIn");
        _CamOutPort.Connect(_CamPositionPort);
        _CamOutPort.Connect(_CamRotationPort);


        GameObjectNode _LeftHandNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(LeftController);
        _LeftHandNode.position.x = -504;
        _LeftHandNode.position.y = -1650;
        var LeftOutPort = _LeftHandNode.GetPort("GameObjectDataOut");
        var leftHandPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "4a97ee43-597d-4582-ba65-6134853ecfd2");
        var leftHandRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "8173f688-5ccc-4771-8d2d-bd64163bfb39");
        var leftPositionPort = leftHandPositionNode.GetPort("GameObjectDataIn");
        var leftRotationPort = leftHandRotationNode.GetPort("GameObjectDataIn");
        LeftOutPort.Connect(leftPositionPort);
        LeftOutPort.Connect(leftRotationPort);


        GameObjectNode _RightHandNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(RightController);
        _RightHandNode.position.x = -504;
        _RightHandNode.position.y = -1850;
        var RightOutPort = _RightHandNode.GetPort("GameObjectDataOut");
        var rightHandPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "07dbcfee-f04b-446e-bea8-59534484fe0f");
        var rightHandRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "bbeda9f7-a97c-4686-84f8-40201c246d3f");
        var rightPositionPort = rightHandPositionNode.GetPort("GameObjectDataIn");
        var rightRotationPort = rightHandRotationNode.GetPort("GameObjectDataIn");
        RightOutPort.Connect(rightPositionPort);
        RightOutPort.Connect(rightRotationPort);


        // GameObjectNode _LNode = new GameObjectNode();
        // _LNode.SetGameObject(LeftController);
        // IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(_LNode);
        // var _LOutPort = _LNode.GetPort("GameObjectDataOut");
        // var LPositionNode = IMLGraph.nodes.First(x=>(x as IMLNode).id == "4a97ee43-597d-4582-ba65-6134853ecfd2");
        // var _LPositionPort = LPositionNode.GetPort("GameObjectDataIn");
        // _LOutPort.Connect(_LPositionPort);

        // GameObjectNode _RNode = new GameObjectNode();
        // _RNode.SetGameObject(RightController);
        // IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(_RNode);
        // var _ROutPort = _RNode.GetPort("GameObjectDataOut");
        // var RPositionNode = IMLGraph.nodes.First(x=>(x as IMLNode).id == "07dbcfee-f04b-446e-bea8-59534484fe0f");
        // var _RPositionPort = RPositionNode.GetPort("GameObjectDataIn");
        // _ROutPort.Connect(_RPositionPort);
    }
}

//(graph as IMLGraph).nodes.First(x => (x as GameObjectNode).GameObjectDataOut.Equals(ourGameObject));
