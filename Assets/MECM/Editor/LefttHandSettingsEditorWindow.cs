using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LefttHandSettingsEditorWindow : EditorWindow
{
    // #region Left Hand Settings
    // bool L_Position_X;
    // bool L_Position_Y;
    // bool L_Position_Z;
    // bool L_PositionVelocity_X;
    // bool L_PositionVelocity_Y;
    // bool L_PositionVelocity_Z;
    // bool L_PositionAcceleration_X;
    // bool L_PositionAcceleration_Y;
    // bool L_PositionAcceleration_Z;

    // bool L_Rotation_X;
    // bool L_Rotation_Y;
    // bool L_Rotation_Z;
    // bool L_Rotation_W;
    // bool L_RotationVelocity_X;
    // bool L_RotationVelocity_Y;
    // bool L_RotationVelocity_Z;
    // bool L_RotationVelocity_W;
    // bool L_RotationAcceleration_X;
    // bool L_RotationAcceleration_Y;
    // bool L_RotationAcceleration_Z;
    // bool L_RotationAcceleration_W;
    // bool L_UseGrabbedObject;
    // #endregion
    #region Left Hand Settings
    bool L_UsePos;
    bool L_UsePositionVelocity;
    bool L_UsePositionAcceleration;
    bool L_UseRot;
    bool L_UseRotationVelocity;
    bool L_UseRotationAcceleration;
    bool L_UseGrabbedObject;
    #endregion

    public static void ShowWindow()
    {
        GetWindow(typeof(LefttHandSettingsEditorWindow),false,"Left Hand Settings");
    }
    private void OnGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Position Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        L_UsePos = GUILayout.Toggle(L_UsePos,"Position");
        L_UsePositionVelocity = GUILayout.Toggle(L_UsePositionVelocity,"Position Velocity");
        L_UsePositionAcceleration = GUILayout.Toggle(L_UsePositionAcceleration,"Position Acceleration");

        EditorGUILayout.Space();
        GUILayout.Label("Rotation Data", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        L_UseRot = GUILayout.Toggle(L_UseRot,"Rotation");
        L_UseRotationVelocity = GUILayout.Toggle(L_UseRotationVelocity,"Rotation Velocity");
        L_UseRotationAcceleration = GUILayout.Toggle(L_UseRotationAcceleration,"Rotation Acceleration");


        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        L_UseGrabbedObject = GUILayout.Toggle(L_UseGrabbedObject,"Grabbed Object");
        EditorGUI.indentLevel--;


        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        

        if(GUILayout.Button("SAVE"))
        {
            //TrackersInfoScriptableObject trackersInfo = Resources.Load<TrackersInfoScriptableObject>("TrackersInfoObject");
            //if(trackersInfo == null)
            //{
            //    trackersInfo = ScriptableObject.CreateInstance<TrackersInfoScriptableObject>();
            //    AssetDatabase.CreateAsset(trackersInfo,"Assets/MECM/Resources/TrackersInfoObject.asset");
            //    AssetDatabase.SaveAssets();
            //}
            //trackersInfo.leftHandAllowedCoord = new AllowedCoord();
            //trackersInfo.leftHandAllowedCoord._Postion = L_UsePos;
            //trackersInfo.leftHandAllowedCoord._PositionVelocity = L_UsePositionVelocity;
            //trackersInfo.leftHandAllowedCoord._PositionAcceleration = L_UsePositionAcceleration;

            //trackersInfo.leftHandAllowedCoord._Rotation = L_UseRot;
            //trackersInfo.leftHandAllowedCoord._RotationVelocity = L_UseRotationVelocity;
            //trackersInfo.leftHandAllowedCoord._RotationAcceleration = L_UseRotationAcceleration;

            //trackersInfo.leftHandAllowedCoord.GrabbedObject = L_UseGrabbedObject;
        }
    }
}
