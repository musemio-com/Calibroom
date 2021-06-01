using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HMDSettingsEditorWindow : EditorWindow
{
    // #region Head Mounted Display Settings
    // bool HMD_Position_X;
    // bool HMD_Position_Y;
    // bool HMD_Position_Z;
    // bool HMD_PositionVelocity_X;
    // bool HMD_PositionVelocity_Y;
    // bool HMD_PositionVelocity_Z;
    // bool HMD_PositionAcceleration_X;
    // bool HMD_PositionAcceleration_Y;
    // bool HMD_PositionAcceleration_Z;

    // bool HMD_Rotation_X;
    // bool HMD_Rotation_Y;
    // bool HMD_Rotation_Z;
    // bool HMD_Rotation_W;
    // bool HMD_RotationVelocity_X;
    // bool HMD_RotationVelocity_Y;
    // bool HMD_RotationVelocity_Z;
    // bool HMD_RotationVelocity_W;
    // bool HMD_RotationAcceleration_X;
    // bool HMD_RotationAcceleration_Y;
    // bool HMD_RotationAcceleration_Z;
    // bool HMD_RotationAcceleration_W;
    // #endregion
    #region HMD Settings
    bool HMD_UsePos;
    bool HMD_UsePositionVelocity;
    bool HMD_UsePositionAcceleration;
    bool HMD_UseRot;
    bool HMD_UseRotationVelocity;
    bool HMD_UseRotationAcceleration;
    #endregion

    public static void ShowWindow()
    {
        GetWindow(typeof(HMDSettingsEditorWindow),false,"Left Hand Settings");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Position Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        HMD_UsePos = GUILayout.Toggle(HMD_UsePos,"Position");
        HMD_UsePositionVelocity = GUILayout.Toggle(HMD_UsePositionVelocity,"Position Velocity");
        HMD_UsePositionAcceleration = GUILayout.Toggle(HMD_UsePositionAcceleration,"Position Acceleration");

        EditorGUILayout.Space();
        GUILayout.Label("Rotation Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        HMD_UseRot = GUILayout.Toggle(HMD_UseRot,"Rotation");
        HMD_UseRotationVelocity = GUILayout.Toggle(HMD_UseRotationVelocity,"Rotation Velocity");
        HMD_UseRotationAcceleration = GUILayout.Toggle(HMD_UseRotationAcceleration,"Rotation Acceleration");


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
            trackersInfo.HeadMountedDisplayAllowedCoord = new AllowedCoord();
            trackersInfo.HeadMountedDisplayAllowedCoord._Postion = HMD_UsePos;
            trackersInfo.HeadMountedDisplayAllowedCoord._PositionVelocity = HMD_UsePositionVelocity;
            trackersInfo.HeadMountedDisplayAllowedCoord._PositionAcceleration = HMD_UsePositionAcceleration;

            trackersInfo.HeadMountedDisplayAllowedCoord._Rotation = HMD_UseRot;
            trackersInfo.HeadMountedDisplayAllowedCoord._RotationVelocity = HMD_UseRotationVelocity;
            trackersInfo.HeadMountedDisplayAllowedCoord._RotationAcceleration = HMD_UseRotationAcceleration;
        }
    }
}
