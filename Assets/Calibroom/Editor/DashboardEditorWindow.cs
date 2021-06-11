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

    DataCollectionController dataController;



    string OverallScore = "Overal Score";
    string VisuoSpatial = "Visuo-Spatial";
    string SpeedProcessing = "Speed Processing";
    string CycleTime = "Cycle Time";



    string FirebaseID;
    float taskCompletionTime;
    bool UploadWhenCollectingDone;

    static DashboardRefs _ref;

    [MenuItem("Calibroom/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow),false,"Calibroom Dashboard");
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
        if(GameObject.Find(_ref.rightHandController.ControllerNameRef))
            RightController = GameObject.Find(_ref.rightHandController.ControllerNameRef);
        if (GameObject.Find(_ref.leftHandController.ControllerNameRef))
            LeftController = GameObject.Find(_ref.leftHandController.ControllerNameRef);
        if (GameObject.Find(_ref.headMountedDisplay.ControllerNameRef))
            HMD = GameObject.Find(_ref.headMountedDisplay.ControllerNameRef);

        trackOnSceneStart= new AnimBool(_ref.trackingSettings.TrackOnSceneStart);
        TriggerTrackWithObjs = new AnimBool(_ref.trackingSettings.TrackTriggerManually.enable);

        if (GameObject.Find(_ref.trackingSettings.TrackTriggerManually.StartTrackingObjectName))
            StartTrackObject = GameObject.Find(_ref.trackingSettings.TrackTriggerManually.StartTrackingObjectName);
        if (GameObject.Find(_ref.trackingSettings.TrackTriggerManually.StopTrackingObjectName))
            StopTrackObject = GameObject.Find(_ref.trackingSettings.TrackTriggerManually.StopTrackingObjectName);

        UploadData = new AnimBool(_ref.uploadSettings.enable);
        FirebaseID = _ref.uploadSettings.firebaseStorageID;


    }
    private void OnDisable()
    {
        SaveRefs();
        //save editor refs in editor prefs    
    }

    private void OnGUI()
    {
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
        EditorGUI.BeginDisabledGroup(TriggerTrackWithObjs.target);
        trackOnSceneStart.target = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", trackOnSceneStart.target);
        if (EditorGUILayout.BeginFadeGroup(trackOnSceneStart.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call FireToggleCollectDataEvent method in DataCollectionController", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUI.EndDisabledGroup();
        EditorGUI.BeginDisabledGroup(trackOnSceneStart.target);
        TriggerTrackWithObjs.target = EditorGUILayout.ToggleLeft("Use GameObjects to trigger manually", TriggerTrackWithObjs.target);
        if (EditorGUILayout.BeginFadeGroup(TriggerTrackWithObjs.faded))
        {
            EditorGUI.indentLevel++;
            StartTrackObject = EditorGUILayout.ObjectField("Start Collecting", StartTrackObject, typeof(GameObject), true) as GameObject;
            StopTrackObject = EditorGUILayout.ObjectField("Stop Collecting", StopTrackObject, typeof(GameObject), true) as GameObject;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUI.EndDisabledGroup();

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
        //GUILayout.Label("Task Completion Time", EditorStyles.boldLabel);
        //EditorGUI.indentLevel++;
        //taskCompletionTime = EditorGUILayout.FloatField("time", taskCompletionTime);
        //EditorGUI.indentLevel--;
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
        GUILayout.Label("Task Completion Time : 0", EditorStyles.boldLabel);
        //EditorGUI.indentLevel++;
        //taskCompletionTime = EditorGUILayout.FloatField("time", taskCompletionTime);
        //EditorGUI.indentLevel--;
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

        EditorCoroutine coroutine = EditorCoroutineUtility.StartCoroutine(SetupPorts(),this);
        if(!RightController.GetComponent<GrabbingPieceFeatureExtractor>())
            RightController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.RightHand;
        if(!LeftController.GetComponent<GrabbingPieceFeatureExtractor>())
            LeftController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.LeftHand;


        if (TriggerTrackWithObjs.target)
        {
            Debug.Log("Setting up controllers");
            StopTrackObject.GetComponent<XRGrabInteractable>().onSelectEnter.AddListener(ToggleCollectOnGrab);
            StartTrackObject.GetComponent<XRGrabInteractable>().onSelectEnter.AddListener(ToggleCollectOnGrab);
        }
        if (UploadData.target)
        {
            GameObject _uploadManager = Instantiate(Resources.Load<GameObject>("Prefabs/UploadManager"));
            _uploadManager.name = "UploadManager";
        }
        SaveRefs();
    }
    void ToggleCollectOnGrab(XRBaseInteractor interactor)
    {
        dataController.GetComponent<DataCollectionController>().ToggleCollectingData();
    }

    void SaveRefs()
    {
        _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
        if(_ref == null)
        {
            _ref = ScriptableObject.CreateInstance<DashboardRefs>();
            AssetDatabase.CreateAsset(_ref,"Assets/MECM/Resources/ScriptableObjects/DashboardRefs.asset");
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }
        _ref.userID = userID;
        PlayerPrefs.SetInt("UserID", userID);

        _ref.rightHandController.ControllerNameRef = RightController.name;
        _ref.leftHandController.ControllerNameRef = LeftController.name;
        _ref.headMountedDisplay.ControllerNameRef = HMD.name;

        _ref.trackingSettings.TrackOnSceneStart = trackOnSceneStart.target;
        _ref.trackingSettings.TrackTriggerManually.enable = TriggerTrackWithObjs.target;
        if (TriggerTrackWithObjs.target)
        {
            _ref.trackingSettings.TrackTriggerManually.StartTrackingObjectName = StartTrackObject.name;
            _ref.trackingSettings.TrackTriggerManually.StopTrackingObjectName = StopTrackObject.name;
        }
        _ref.uploadSettings.enable = UploadData.target;
        _ref.uploadSettings.firebaseStorageID = FirebaseID;
    }
    IEnumerator SetupPorts()
    {
        if (FindObjectOfType<DataCollectionController>())
            DestroyImmediate(FindObjectOfType<DataCollectionController>().gameObject);
        if (FindObjectOfType<IMLComponent>())
            DestroyImmediate(FindObjectOfType<IMLComponent>().gameObject);

        GameObject DataController = Instantiate(Resources.Load<GameObject>("Prefabs/DataCollection"));
        GameObject IMLSystem = Instantiate(Resources.Load<GameObject>("Prefabs/IML System"));
        DataController.name = "DataCollection";
        IMLSystem.name = "IML System";
        dataController = DataController.GetComponent<DataCollectionController>();

        

        
        yield return new EditorWaitForSeconds(2f);
        
        IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();
        IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData.Add(new IMLMonoBehaviourContainer(dataController));
        var IMLGraph = IMLSystem.GetComponent<IMLComponent>().graph;
        ScriptNode goNode = (ScriptNode)(IMLGraph as IMLGraph).AddNode(typeof(ScriptNode));
        if (goNode != null)
        {
            goNode.SetScript(dataController);
            goNode.graph = IMLGraph;
            IMLSystem.GetComponent<IMLComponent>().m_ScriptNodesList = new List<ScriptNode>();
            if (!IMLSystem.GetComponent<IMLComponent>().m_ScriptNodesList.Contains(goNode))
                IMLSystem.GetComponent<IMLComponent>().m_ScriptNodesList.Add(goNode);
            IMLSystem.GetComponent<IMLComponent>().m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
            if (!IMLSystem.GetComponent<IMLComponent>().m_MonoBehavioursPerScriptNode.Contains(goNode))
                IMLSystem.GetComponent<IMLComponent>().m_MonoBehavioursPerScriptNode.Add(dataController,goNode);
            IMLSystem.GetComponent<IMLComponent>().FetchDataFromMonobehavioursSubscribed();



            goNode.position.x = 424;
            goNode.position.y = 104;
            var ToggleDataCollectionOutPort = goNode.GetOutputPort("ToggleDataCollection");
            var userIdOutPort = goNode.GetPort("UserIDInt");
            var leftGrabOutPort = goNode.GetPort("LeftHandGrabbing");
            var rightGrabOutPort = goNode.GetPort("RightHandGrabbing");

            var ml_GrabDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "50740dc4-667d-43d3-8c8b-7812d605ecc9");
            var GrabToggleRectPort = ml_GrabDataNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_PosDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "b2accf43-eb94-4f24-be68-bffa39e2ea08");
            var PosToggleRecPort = ml_PosDataNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_PosVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "59188512-88bb-4f3b-a661-e904a1d7bfb1");
            var PostVelToggleRecPort = ml_PosVelNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_PosAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "d1b74749-7cfd-4c68-85e2-6c4cbf758ad1");
            var PosAccToggleRecPort = ml_PosAccNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_RotDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "194dccce-2f98-419b-807f-d01c74767790");
            var RotToggleRecPort = ml_RotDataNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_RotVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "57469dcc-ac0f-4a14-8c85-29c2019db115");
            var RotVelToggleRecPort = ml_RotVelNode.GetPort("ToggleRecordingInputBoolPort");
            var ml_RotAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "efe83634-706e-4f18-b909-9982df9c2cc1");
            var RotAccToggleRecPort = ml_RotAccNode.GetPort("ToggleRecordingInputBoolPort");

            ToggleDataCollectionOutPort.Connect(PosToggleRecPort);
            ToggleDataCollectionOutPort.Connect(PostVelToggleRecPort);
            ToggleDataCollectionOutPort.Connect(PosAccToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotVelToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotAccToggleRecPort);
            ToggleDataCollectionOutPort.Connect(GrabToggleRectPort);


            var IntGrabNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "6b9b45a4-413e-48e7-bdd2-418ad5778fe6");
            var IntGrabPort = IntGrabNode.GetPort("m_In");
            var IntPosNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "0b2698e5-5434-45db-9863-545c915054b2");
            var IntPosPort = IntPosNode.GetPort("m_In");
            var IntPosVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "a7a28c2d-6f60-4ba3-92f6-69a3d276eb2d");
            var IntPosVelPort = IntPosVelNode.GetPort("m_In");
            var IntPosAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "10b38843-f787-4e90-b7f9-9eb2370e4885");
            var IntPosAccPort = IntPosAccNode.GetPort("m_In");
            var IntRotNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "5df11976-f51b-4241-a5e2-3b7352bec47b");
            var IntRotPort = IntRotNode.GetPort("m_In");
            var IntRotVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "cee2b392-7610-4adf-b50c-ef5d365337d9");
            var IntRotVelPort = IntRotVelNode.GetPort("m_In");
            var IntRotAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "38780414-ad7f-43f4-a907-1b9d9375149c");
            var IntRotAccPort = IntRotAccNode.GetPort("m_In");

            userIdOutPort.Connect(IntGrabPort);
            userIdOutPort.Connect(IntPosPort);
            userIdOutPort.Connect(IntPosVelPort);
            userIdOutPort.Connect(IntPosAccPort);
            userIdOutPort.Connect(IntRotPort);
            userIdOutPort.Connect(IntRotVelPort);
            userIdOutPort.Connect(IntRotAccPort);

            var LeftBoolGrabNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "222ec0ae-d030-44a7-ad11-3f34cd4351e9");
            var LeftBoolGrabPort = LeftBoolGrabNode.GetPort("m_In");
            var RightBoolGrabNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "e05e82b3-4801-4ec2-8e1d-ab01343b09d5");
            var RightBoolGrabPort = RightBoolGrabNode.GetPort("m_In");

            leftGrabOutPort.Connect(LeftBoolGrabPort);
            rightGrabOutPort.Connect(RightBoolGrabPort);

        }

        GameObjectNode _CamNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(HMD);
        _CamNode.position.x = -456;
        _CamNode.position.y = -1208;
        var _CamOutPort = _CamNode.GetPort("GameObjectDataOut");
        var mainCameraPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "82e72311-ad4c-4813-a73c-f7a92fd550de");
        var mainCameraRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "7159019a-48c4-4529-8008-868e19386616");
        var _CamRotationPort = mainCameraRotationNode.GetPort("GameObjectDataIn");
        var _CamPositionPort = mainCameraPositionNode.GetPort("GameObjectDataIn");
        _CamOutPort.Connect(_CamPositionPort);
        _CamOutPort.Connect(_CamRotationPort);


        GameObjectNode _LeftHandNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(LeftController);
        _LeftHandNode.position.x = -456;
        _LeftHandNode.position.y = -856;
        var LeftOutPort = _LeftHandNode.GetPort("GameObjectDataOut");
        var leftHandPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "4a97ee43-597d-4582-ba65-6134853ecfd2");
        var leftHandRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "8173f688-5ccc-4771-8d2d-bd64163bfb39");
        var leftPositionPort = leftHandPositionNode.GetPort("GameObjectDataIn");
        var leftRotationPort = leftHandRotationNode.GetPort("GameObjectDataIn");
        LeftOutPort.Connect(leftPositionPort);
        LeftOutPort.Connect(leftRotationPort);


        GameObjectNode _RightHandNode = IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(RightController);
        _RightHandNode.position.x = -456;
        _RightHandNode.position.y = -504;
        var RightOutPort = _RightHandNode.GetPort("GameObjectDataOut");
        var rightHandPositionNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "07dbcfee-f04b-446e-bea8-59534484fe0f");
        var rightHandRotationNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "bbeda9f7-a97c-4686-84f8-40201c246d3f");
        var rightPositionPort = rightHandPositionNode.GetPort("GameObjectDataIn");
        var rightRotationPort = rightHandRotationNode.GetPort("GameObjectDataIn");
        RightOutPort.Connect(rightPositionPort);
        RightOutPort.Connect(rightRotationPort);
    }
}
