using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RightHandSettingsEditorWindow : EditorWindow
{
    // #region Right Hand Settings
    // bool R_Position_X;
    // bool R_Position_Y;
    // bool R_Position_Z;
    // bool R_PositionVelocity_X;
    // bool R_PositionVelocity_Y;
    // bool R_PositionVelocity_Z;
    // bool R_PositionAcceleration_X;
    // bool R_PositionAcceleration_Y;
    // bool R_PositionAcceleration_Z;

    // bool R_Rotation_X;
    // bool R_Rotation_Y;
    // bool R_Rotation_Z;
    // bool R_Rotation_W;
    // bool R_RotationVelocity_X;
    // bool R_RotationVelocity_Y;
    // bool R_RotationVelocity_Z;
    // bool R_RotationVelocity_W;
    // bool R_RotationAcceleration_X;
    // bool R_RotationAcceleration_Y;
    // bool R_RotationAcceleration_Z;
    // bool R_RotationAcceleration_W;
    // bool R_UseGrabbedObject;
    // #endregion
    #region Right Hand Settings
    bool R_UsePos;
    bool R_UsePositionVelocity;
    bool R_UsePositionAcceleration;
    bool R_UseRot;
    bool R_UseRotationVelocity;
    bool R_UseRotationAcceleration;
    bool R_UseGrabbedObject;
    #endregion
    public static void ShowWindow()
    {
        GetWindow(typeof(RightHandSettingsEditorWindow),false,"Right Hand Settings");
    }
    private void OnEnable()
    {
     // check saved variables in resources   
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Position Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        R_UsePos = GUILayout.Toggle(R_UsePos,"Position");
        R_UsePositionVelocity = GUILayout.Toggle(R_UsePositionVelocity,"Position Velocity");
        R_UsePositionAcceleration = GUILayout.Toggle(R_UsePositionAcceleration,"Position Acceleration");

        EditorGUILayout.Space();
        GUILayout.Label("Rotation Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        R_UseRot = GUILayout.Toggle(R_UseRot,"Rotation");
        R_UseRotationVelocity = GUILayout.Toggle(R_UseRotationVelocity,"Rotation Velocity");
        R_UseRotationAcceleration = GUILayout.Toggle(R_UseRotationAcceleration,"Rotation Acceleration");


        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        R_UseGrabbedObject = GUILayout.Toggle(R_UseGrabbedObject,"Grabbed Object");
        EditorGUI.indentLevel--;


        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        

        if(GUILayout.Button("SAVE"))
        {
            TrackersInfoScriptableObject trackersInfo = Resources.Load<TrackersInfoScriptableObject>("TrackersInfoObject");
            if(trackersInfo == null)
            {
                trackersInfo = ScriptableObject.CreateInstance<TrackersInfoScriptableObject>();
                AssetDatabase.CreateAsset(trackersInfo,"Assets/MECM/Resources/TrackersInfoObject.asset");
                AssetDatabase.SaveAssets();
            }
            trackersInfo.rightHandAllowedCoord = new AllowedCoord();
            trackersInfo.rightHandAllowedCoord._Postion = R_UsePos;
            trackersInfo.rightHandAllowedCoord._PositionVelocity = R_UsePositionVelocity;
            trackersInfo.rightHandAllowedCoord._PositionAcceleration = R_UsePositionAcceleration;

            trackersInfo.rightHandAllowedCoord._Rotation = R_UseRot;
            trackersInfo.rightHandAllowedCoord._RotationVelocity = R_UseRotationVelocity;
            trackersInfo.rightHandAllowedCoord._RotationAcceleration = R_UseRotationAcceleration;

            trackersInfo.rightHandAllowedCoord.GrabbedObject = R_UseGrabbedObject;
        }
    }
}
