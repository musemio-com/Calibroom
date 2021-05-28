using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
public class DashboardEditorWindow : EditorWindow
{
    GameObject LeftController;
    GameObject RightController;
    GameObject HMD;
    GameObject customTrackingTriggerObject;
    string userID;
    AnimBool StartTrackingWhenSceneActive;
    Color StartObjectColor;
    Color EndObjectColor;
    AnimBool customTrackingTrigger;
    AnimBool SpawnStartEndObjects;
    AnimBool UploadData;
    AnimBool firebaseEnabled;
    AnimBool RestEnabled;
    string firebaseStorage;
    string CredentialsPath;
    string PathToUpload;
    float taskCompletionTime;

    [MenuItem("MECM/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow),false,"Calibration Method Dashboard");
    }
    private void OnEnable()
    {
        StartTrackingWhenSceneActive = new AnimBool(false);
        StartTrackingWhenSceneActive.valueChanged.AddListener(Repaint);
        UploadData = new AnimBool(false);
        UploadData.valueChanged.AddListener(Repaint);
        customTrackingTrigger = new AnimBool(false);
        customTrackingTrigger.valueChanged.AddListener(Repaint);
        SpawnStartEndObjects = new AnimBool(false);
        SpawnStartEndObjects.valueChanged.AddListener(Repaint);
        firebaseEnabled = new AnimBool(false);
        firebaseEnabled.valueChanged.AddListener(Repaint);
        RestEnabled = new AnimBool(false);
        RestEnabled.valueChanged.AddListener(Repaint);

    }
    private void OnGUI()
    {
        GUILayout.Label("Calibration Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        GUILayout.Label("User ID", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        userID = EditorGUILayout.TextField("User ID", userID);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Controllers To Track",EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        RightController = EditorGUILayout.ObjectField("Right Controller", RightController, typeof(GameObject), false) as GameObject;
        if (GUILayout.Button(".", GUILayout.Height(20), GUILayout.Width(25)))
        {
            
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        LeftController = EditorGUILayout.ObjectField("Left Controller", LeftController, typeof(GameObject), false) as GameObject;
        if (GUILayout.Button(".", GUILayout.Height(20), GUILayout.Width(25)))
        {

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        HMD = EditorGUILayout.ObjectField("Head Mounted Display", HMD, typeof(GameObject), false) as GameObject;
        if (GUILayout.Button(".", GUILayout.Height(20), GUILayout.Width(25)))
        {

        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        GUILayout.Label("Tracking", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        StartTrackingWhenSceneActive.target = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", StartTrackingWhenSceneActive.target);
        if (EditorGUILayout.BeginFadeGroup(StartTrackingWhenSceneActive.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call StopTracking method", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        SpawnStartEndObjects.target = EditorGUILayout.ToggleLeft("Spawn Start Stop Tracking Objects", SpawnStartEndObjects.target);
        if (EditorGUILayout.BeginFadeGroup(SpawnStartEndObjects.faded))
        {
            EditorGUI.indentLevel++;
            StartObjectColor = EditorGUILayout.ColorField("Start Object Color", StartObjectColor);
            EndObjectColor = EditorGUILayout.ColorField("Stop Object Color", EndObjectColor);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        customTrackingTrigger.target = EditorGUILayout.ToggleLeft("Custom Trigger Object", customTrackingTrigger.target);
        if (EditorGUILayout.BeginFadeGroup(customTrackingTrigger.faded))
        {
            EditorGUI.indentLevel++;
            customTrackingTriggerObject = EditorGUILayout.ObjectField("Custom Object", customTrackingTriggerObject, typeof(GameObject), false) as GameObject;
            EditorGUILayout.HelpBox("To stop Collecting Data, Call StopTracking method", MessageType.Info);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        GUILayout.Label("Upload Data to Firebase", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        UploadData.target = EditorGUILayout.ToggleLeft("Upload Data", UploadData.target);
        if (EditorGUILayout.BeginFadeGroup(UploadData.faded))
        {
            EditorGUI.indentLevel++;
            firebaseEnabled.target = EditorGUILayout.ToggleLeft("Using Firebase API", firebaseEnabled.target);
            if (EditorGUILayout.BeginFadeGroup(firebaseEnabled.faded))
            {
                EditorGUI.indentLevel++;
                firebaseStorage = EditorGUILayout.TextField("Storage Bucket", firebaseStorage);
                CredentialsPath = EditorGUILayout.TextField("Credentials Path", CredentialsPath);
                PathToUpload = EditorGUILayout.TextField("Path To Upload", PathToUpload);
                EditorGUILayout.HelpBox("Not supported on Oculus Quest", MessageType.Info);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            RestEnabled.target = EditorGUILayout.ToggleLeft("Using REST API", RestEnabled.target);
            if (EditorGUILayout.BeginFadeGroup(RestEnabled.faded))
            {
                EditorGUI.indentLevel++;
                firebaseStorage = EditorGUILayout.TextField("Storage Bucket", firebaseStorage);
                CredentialsPath = EditorGUILayout.TextField("Credentials Path", CredentialsPath);
                PathToUpload = EditorGUILayout.TextField("Path To Upload", PathToUpload);
                EditorGUILayout.HelpBox("Slow, but supported on all platforms", MessageType.Info);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUI.indentLevel--;
        GUILayout.Label("Task Completion Time", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        taskCompletionTime = EditorGUILayout.FloatField("time", taskCompletionTime);
        EditorGUI.indentLevel--;
    }
}
