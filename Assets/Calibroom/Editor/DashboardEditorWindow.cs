using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MECM;
using InteractML;
using XNode;
using System.Linq;
using System;
using Unity.EditorCoroutines.Editor;
using UnityEditor.SceneManagement;

public class DashboardEditorWindow : EditorWindow
{
    static int userID;
    static GameObject LeftController;
    static GameObject RightController;
    static GameObject HMD;
    static AnimBool trackOnSceneStart;
    static AnimBool UploadData;
    Vector2 scrollPos;

    static bool OnCollectionStatus;
    static string OnCollectionStatusString = "OFF";
    static bool IsRightHandGrabbing;
    static string IsRightHandGrabbingString = "OFF";
    static bool IsLeftHandGrabbing;
    static string IsLeftHandGrabbingString = "OFF";
    private static Texture2D DataCollectionButtonTex;
    private static Texture2D RightHandButtonTex;
    private static Texture2D LeftHandButtonTex;

    DataCollectionController dataController;

    static string FirebaseID;
    GUIStyle SuggestionButtonsStyle;
    static GUIStyle CollectionButtonStyle;
    static GUIStyle RightHandButtonStyle;
    static GUIStyle LeftHandButtonStyle;

    static float taskCompletionTimeScale = 0.0f;
    static float VisuoSpatialScale = 0.0f;
    static float SpeedProcessingScale = 0.0f;

    static DashboardRefs _ref;

    [InitializeOnLoadMethod]
    static void Init()
    {
        UpdateEditorData._OnCollectingDelegate += RefreshCollectionStauts;
        UpdateEditorData._OnRightHandDelegate = RefreshRightHandStatus;
        UpdateEditorData._OnLeftHandDelegate = RefreshLeftHandStatus;
        UpdateEditorData._OnTaskTimeDelegate = RefreshCompletionTimeStatus;
        UpdateScore._OnUpdateScore = SetupScore;

    }

    [MenuItem("Calibroom/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow),false,"Calibroom Dashboard");
    }
    private void OnEnable()
    {
        DataCollectionButtonTex = Make1x1Texture(Color.red);
        RightHandButtonTex = Make1x1Texture(Color.red);
        LeftHandButtonTex = Make1x1Texture(Color.red);
        trackOnSceneStart = new AnimBool(false);
        trackOnSceneStart.valueChanged.AddListener(Repaint);
        UploadData = new AnimBool(false);
        UploadData.valueChanged.AddListener(Repaint);

        loadAllRefs();
    }
    private void OnDisable()
    {
        DataCollectionButtonTex = Make1x1Texture(Color.red);
        RightHandButtonTex = Make1x1Texture(Color.red);
        LeftHandButtonTex = Make1x1Texture(Color.red);
    }
    static Texture2D Make1x1Texture(Color _color)
    {
        Texture2D tex = new Texture2D(1, 1);
        for (int y = 0; y < tex.height; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color color = ((x & y) != 0 ? _color : _color);
                tex.SetPixel(x, y, color);
            }
        }
        tex.Apply();
        return tex;
    }
    void OnInspectorUpdate()
    {
        Repaint();
    }

    static void loadAllRefs()
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
        trackOnSceneStart = new AnimBool(_ref.TrackOnSceneActive);
        UploadData = new AnimBool(_ref.uploadSettings.enable);
        FirebaseID = _ref.uploadSettings.firebaseStorageID;
    }

    private void OnGUI()
    {
        var HeadersStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState() { background = Texture2D.whiteTexture }
        };
        SuggestionButtonsStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
        };
        CollectionButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            normal = new GUIStyleState() { background = DataCollectionButtonTex }
        };
        RightHandButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            normal = new GUIStyleState() { background = RightHandButtonTex }
        };
        LeftHandButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            normal = new GUIStyleState() { background = LeftHandButtonTex }
        };


        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.LabelField("Settings", HeadersStyle);

        EditorGUILayout.Space();

        userID = EditorGUILayout.IntField("User ID", userID);
        EditorGUILayout.Space();

        GUILayout.Label("Components To Track",EditorStyles.boldLabel);
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
            EditorGUILayout.HelpBox("Call ToggleCollectingData To Stop Collecting Data", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        GUILayout.Label("Upload to Firebase Cloud Storage", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        UploadData.target = EditorGUILayout.ToggleLeft("Upload data when Data Collection Finishes", UploadData.target);
        if (EditorGUILayout.BeginFadeGroup(UploadData.faded))
        {
            EditorGUI.indentLevel++;
            FirebaseID = EditorGUILayout.TextField("Firebase Project ID", FirebaseID);
            EditorGUILayout.HelpBox("Using REST API, Slow but supported on all platforms", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        var SetupButtonStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 15,
            fontStyle = FontStyle.Bold,
        };
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUI.indentLevel++;
        if (GUILayout.Button("Setup",SetupButtonStyle,GUILayout.Height(30),GUILayout.Width(120)) && (RightController != null && LeftController != null && HMD != null))
        {
            DashboardSetup();        
        }
        EditorGUI.indentLevel--;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (RightController == null)
        {
            EditorGUILayout.HelpBox("Right Controller is missing",MessageType.Warning);
        }

        if (LeftController == null)
        {
            EditorGUILayout.HelpBox("Left Controller is missing",MessageType.Warning);
        }
        if (HMD == null)
        {
            EditorGUILayout.HelpBox("HMD Controller is missing",MessageType.Warning);
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Status Overview", HeadersStyle);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Data Collection");
        GUILayout.Button(OnCollectionStatusString, CollectionButtonStyle, GUILayout.Height(20), GUILayout.Width(40));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Is RightHand Grabbing");
        GUILayout.Button(IsRightHandGrabbingString, RightHandButtonStyle, GUILayout.Height(20), GUILayout.Width(40));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Is LeftHand Grabbing");
        GUILayout.Button(IsLeftHandGrabbingString, LeftHandButtonStyle, GUILayout.Height(20), GUILayout.Width(40));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scoring", HeadersStyle);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        taskCompletionTimeScale = EditorGUILayout.Slider("Task completion time(sec)", taskCompletionTimeScale, 1f, 1000f);
        VisuoSpatialScale = EditorGUILayout.Slider("Visuo-Spatial", VisuoSpatialScale, 0f, 100f);
        SpeedProcessingScale = EditorGUILayout.Slider("Speed Processing", SpeedProcessingScale, 0f, 100f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Mode Suggestions", HeadersStyle);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        EditorGUI.indentLevel++;
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Easier", SuggestionButtonsStyle, GUILayout.Height(30),GUILayout.Width(80)))
        {
            
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Harder", SuggestionButtonsStyle, GUILayout.Height(30), GUILayout.Width(80)))
        {
            
        }
        EditorGUI.indentLevel--;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }
    public static void RefreshCompletionTimeStatus(float _t)
    {
        taskCompletionTimeScale = Mathf.Round(_t * 100) / 100;
    }
    public static void SetupScore(float _score1,float _score2, float _score3)
    {
        float visuoSpacialScore = (_score1 / 7132f) * 10f;
        float SpeedProcessingScore = ((_score2 + _score3) / (2812f + 9410f)) * 10f;
        //Debug.Log("VISUO SPACIAL : " + visuoSpacialScore);
        //Debug.Log("SPEED PROCESSING : " + SpeedProcessingScore);
        VisuoSpatialScale = (float)Math.Round(visuoSpacialScore * 10f) / 10f;
        SpeedProcessingScale = Mathf.Round(SpeedProcessingScore * 10) / 10;
    }
    public static void RefreshCollectionStauts(bool _state)
    {
        OnCollectionStatus = !OnCollectionStatus;
        if (OnCollectionStatus)
        {
            DataCollectionButtonTex = Make1x1Texture(Color.green);
            OnCollectionStatusString = "ON";
        }
        else
        {
            DataCollectionButtonTex = Make1x1Texture(Color.red);
            OnCollectionStatusString = "OFF";
        }
    }
    public static void RefreshRightHandStatus(bool state)
    {
        IsRightHandGrabbing = state;
        if (IsRightHandGrabbing)
        {
            RightHandButtonTex = Make1x1Texture(Color.green);
            IsRightHandGrabbingString = "ON";
        }
        else
        {
            RightHandButtonTex = Make1x1Texture(Color.red);
            IsRightHandGrabbingString = "OFF";
        }    
    }
    public static void RefreshLeftHandStatus(bool state)
    {
        IsLeftHandGrabbing = state;
        if (IsLeftHandGrabbing)
        {
            LeftHandButtonTex = Make1x1Texture(Color.green);
            IsLeftHandGrabbingString = "ON";
        }    
        else
        {
            LeftHandButtonTex = Make1x1Texture(Color.red);
            IsLeftHandGrabbingString = "OFF";
        }    
    }

    void DashboardSetup()
    {
        taskCompletionTimeScale = 0f;
        EditorCoroutine coroutine = EditorCoroutineUtility.StartCoroutine(IMLDataSetupCouroutine(),this);
        if(!RightController.GetComponent<GrabbingPieceFeatureExtractor>())
            RightController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.RightHand;
        if(!LeftController.GetComponent<GrabbingPieceFeatureExtractor>())
            LeftController.AddComponent<GrabbingPieceFeatureExtractor>().m_XRControllerType = GrabbingPieceFeatureExtractor.ControllerType.LeftHand;
        if (UploadData.target)
        {
            GameObject _uploadManager = Instantiate(Resources.Load<GameObject>("Prefabs/UploadManager"));
            _uploadManager.name = "UploadManager";
        }
        SaveDashboardData();
    }
    void SaveDashboardData()
    {
        _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
        if(_ref == null)
        {
            _ref = CreateInstance<DashboardRefs>();
            AssetDatabase.CreateAsset(_ref, "Assets/Calibroom/Resources/ScriptableObjects/DashboardRefs.asset");
            EditorApplication.delayCall += AssetDatabase.SaveAssets;
        }
        _ref.userID = userID;
        PlayerPrefs.SetInt("UserID", userID);

        if(RightController != null)
            _ref.rightHandController.ControllerNameRef = RightController.name;
        if(LeftController != null)
            _ref.leftHandController.ControllerNameRef = LeftController.name;
        if(HMD != null)
            _ref.headMountedDisplay.ControllerNameRef = HMD.name;

        _ref.TrackOnSceneActive = trackOnSceneStart.target;
        _ref.uploadSettings.enable = UploadData.target;
        _ref.uploadSettings.firebaseStorageID = FirebaseID;
    }
    IEnumerator IMLDataSetupCouroutine()
    {
        if (FindObjectOfType<DataCollectionController>())
            DestroyImmediate(FindObjectOfType<DataCollectionController>().gameObject);
        if (FindObjectOfType<IMLComponent>())
            DestroyImmediate(FindObjectOfType<IMLComponent>().gameObject);
        GameObject dataControllerObj = Resources.Load<GameObject>("Prefabs/DataCollection");
        GameObject imlObj = Resources.Load<GameObject>("Prefabs/IML System");
        GameObject neuralNetworkObj = Resources.Load<GameObject>("Prefabs/IML System_NeuralNetwork");


        GameObject IMLSystem = Instantiate(imlObj);
        GameObject IMLNeuralNetwork = Instantiate(neuralNetworkObj);
        GameObject DataController = Instantiate(dataControllerObj);
        DataController.name = "DataCollection";
        IMLSystem.name = "IML System";
        IMLNeuralNetwork.name = "IML NeuralNetwork";
        dataController = DataController.GetComponent<DataCollectionController>();

        IMLGraph IMLGraph = IMLSystem.GetComponent<IMLComponent>().graph;
        IMLGraph neuralNetworkGraph = IMLNeuralNetwork.GetComponent<IMLComponent>().graph;

        //if (IMLGraph.SceneComponent == null || IMLGraph.SceneComponent != IMLSystem.GetComponent<IMLComponent>())
        //    IMLGraph.SceneComponent = IMLSystem.GetComponent<IMLComponent>();

        //if (neuralNetworkGraph.SceneComponent == null || neuralNetworkGraph.SceneComponent != IMLNeuralNetwork.GetComponent<IMLComponent>())
        //    neuralNetworkGraph.SceneComponent = IMLNeuralNetwork.GetComponent<IMLComponent>();
        if (IMLGraph.SceneComponent == null)
            IMLGraph.SceneComponent = IMLSystem.GetComponent<IMLComponent>();

        yield return new EditorWaitForSeconds(3f);

        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(IMLGraph));
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();
        IMLSystem.GetComponent<IMLComponent>().ComponentsWithIMLData.Add(new IMLMonoBehaviourContainer(dataController));
        //ScriptNode goNode = new ScriptNode();
        //goNode = IMLSystem.GetComponent<IMLComponent>().ScriptNodesList[0];


        ScriptNode goNode = (ScriptNode)IMLGraph.AddNode(typeof(ScriptNode));

        if (goNode != null)
        {
            goNode.SetScript(dataController);
            goNode.graph = IMLGraph;
            IMLSystem.GetComponent<IMLComponent>().ScriptNodesList = new List<ScriptNode>();
            if (!IMLSystem.GetComponent<IMLComponent>().ScriptNodesList.Contains(goNode))
                IMLSystem.GetComponent<IMLComponent>().ScriptNodesList.Add(goNode);
            IMLSystem.GetComponent<IMLComponent>().MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
            if (!IMLSystem.GetComponent<IMLComponent>().MonoBehavioursPerScriptNode.Contains(goNode))
                IMLSystem.GetComponent<IMLComponent>().MonoBehavioursPerScriptNode.Add(dataController, goNode);
            IMLSystem.GetComponent<IMLComponent>().FetchDataFromMonobehavioursSubscribed();

            goNode.position.x = 424;
            goNode.position.y = 104;

            var ToggleDataCollectionOutPort = goNode.GetOutputPort("ToggleDataCollection");
            var userIdOutPort = goNode.GetPort("UserIDInt");
            var leftGrabOutPort = goNode.GetPort("LeftHandGrabbing");
            var rightGrabOutPort = goNode.GetPort("RightHandGrabbing");
            var UserIDStringOurPort = goNode.GetPort("UserIDString");

            var ml_GrabDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "50740dc4-667d-43d3-8c8b-7812d605ecc9");
            var GrabToggleRectPort = ml_GrabDataNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortOne = ml_GrabDataNode.GetPort("SubFolderDataPathStringPort");
            var ml_PosDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "b2accf43-eb94-4f24-be68-bffa39e2ea08");
            var PosToggleRecPort = ml_PosDataNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortTwo = ml_PosDataNode.GetPort("SubFolderDataPathStringPort");
            var ml_PosVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "59188512-88bb-4f3b-a661-e904a1d7bfb1");
            var PostVelToggleRecPort = ml_PosVelNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortThree = ml_PosVelNode.GetPort("SubFolderDataPathStringPort");
            var ml_PosAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "d1b74749-7cfd-4c68-85e2-6c4cbf758ad1");
            var PosAccToggleRecPort = ml_PosAccNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortFour = ml_PosAccNode.GetPort("SubFolderDataPathStringPort");
            var ml_RotDataNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "194dccce-2f98-419b-807f-d01c74767790");
            var RotToggleRecPort = ml_RotDataNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortFive = ml_RotDataNode.GetPort("SubFolderDataPathStringPort");
            var ml_RotVelNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "57469dcc-ac0f-4a14-8c85-29c2019db115");
            var RotVelToggleRecPort = ml_RotVelNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortSix = ml_RotVelNode.GetPort("SubFolderDataPathStringPort");
            var ml_RotAccNode = IMLGraph.nodes.First(x => (x as IMLNode).id == "efe83634-706e-4f18-b909-9982df9c2cc1");
            var RotAccToggleRecPort = ml_RotAccNode.GetPort("ToggleRecordingInputBoolPort");
            var userIDStringPathPortSeven = ml_RotAccNode.GetPort("SubFolderDataPathStringPort");

            ToggleDataCollectionOutPort.Connect(PosToggleRecPort);
            ToggleDataCollectionOutPort.Connect(PostVelToggleRecPort);
            ToggleDataCollectionOutPort.Connect(PosAccToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotVelToggleRecPort);
            ToggleDataCollectionOutPort.Connect(RotAccToggleRecPort);
            ToggleDataCollectionOutPort.Connect(GrabToggleRectPort);

            UserIDStringOurPort.Connect(userIDStringPathPortOne);
            UserIDStringOurPort.Connect(userIDStringPathPortTwo);
            UserIDStringOurPort.Connect(userIDStringPathPortThree);
            UserIDStringOurPort.Connect(userIDStringPathPortFour);
            UserIDStringOurPort.Connect(userIDStringPathPortFive);
            UserIDStringOurPort.Connect(userIDStringPathPortSix);
            UserIDStringOurPort.Connect(userIDStringPathPortSeven);


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

        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse = new List<GameObject>();

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
        IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(_CamNode);
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(HMD);


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
        IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(_LeftHandNode);
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(LeftController);

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
        IMLSystem.GetComponent<IMLComponent>().AddGameObjectNode(_RightHandNode);
        IMLSystem.GetComponent<IMLComponent>().GameObjectsToUse.Add(RightController);
    }
}
