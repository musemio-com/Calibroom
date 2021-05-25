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
    AnimBool SpawnStartEndObjects;
    bool StartTrackingWhenSceneActive;
    AnimBool customTrackingTrigger;
    Color StartObjectColor;
    Color EndObjectColor;
    AnimBool UploadData;
    AnimBool firebaseEnabled;
    AnimBool RestEnabled;
    string firebaseStorage;
    string CredentialsPath;
    string PathToUpload;

    [MenuItem("MECM/Dashboard")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DashboardEditorWindow));
    }
    private void OnGUI()
    {
        GUILayout.Label("DASHBOARD", EditorStyles.boldLabel);
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
        StartTrackingWhenSceneActive = EditorGUILayout.ToggleLeft("Start Tracking when scene starts", StartTrackingWhenSceneActive);
        SpawnStartEndObjects.target = EditorGUILayout.ToggleLeft("Spawn Start Stop Tracking Objects", SpawnStartEndObjects.target);

        if (EditorGUILayout.BeginFadeGroup(SpawnStartEndObjects.faded))
        {
            EditorGUI.indentLevel++;
            StartObjectColor = EditorGUILayout.ColorField("Start Object Color", StartObjectColor);
            EndObjectColor = EditorGUILayout.ColorField("Stop Object Color", EndObjectColor);
            EditorGUILayout.HelpBox("You can also Start/Stop tracking in code", MessageType.Info);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndFadeGroup();

        customTrackingTrigger.target = EditorGUILayout.ToggleLeft("Custom Trigger Object", customTrackingTrigger.target);
        if (EditorGUILayout.BeginFadeGroup(customTrackingTrigger.faded))
        {
            EditorGUI.indentLevel++;
            customTrackingTriggerObject = EditorGUILayout.ObjectField("Custom Object", customTrackingTriggerObject, typeof(GameObject), false) as GameObject;
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        GUILayout.Label("Upload Data to server", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        UploadData.target = EditorGUILayout.ToggleLeft("Upload Data", UploadData.target);
        if (EditorGUILayout.BeginFadeGroup(UploadData.faded))
        {
            EditorGUI.indentLevel++;
            firebaseEnabled.target = EditorGUILayout.ToggleLeft("Firebase", firebaseEnabled.target);
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

            RestEnabled.target = EditorGUILayout.ToggleLeft("REST API", RestEnabled.target);
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
        //EditorGUILayout.EndToggleGroup();

    }
}
