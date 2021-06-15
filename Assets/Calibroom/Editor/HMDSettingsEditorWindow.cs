using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HMDSettingsEditorWindow : EditorWindow
{
    #region HMD Settings
    bool HMD_UsePos = true;
    bool HMD_UsePositionVelocity = true;
    bool HMD_UsePositionAcceleration = true;
    bool HMD_UseRot = true;
    bool HMD_UseRotationVelocity = true;
    bool HMD_UseRotationAcceleration = true;
    #endregion

    public static void ShowWindow()
    {
        GetWindow(typeof(HMDSettingsEditorWindow),false,"Left Hand Settings");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        GUILayout.Label("Position Data", style,GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        HMD_UsePos = GUILayout.Toggle(HMD_UsePos,"Position");
        HMD_UsePositionVelocity = GUILayout.Toggle(HMD_UsePositionVelocity,"Position Velocity");
        HMD_UsePositionAcceleration = GUILayout.Toggle(HMD_UsePositionAcceleration,"Position Acceleration");

        EditorGUILayout.Space();
        GUILayout.Label("Rotation Data", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.Space();

        HMD_UseRot = GUILayout.Toggle(HMD_UseRot,"Rotation");
        HMD_UseRotationVelocity = GUILayout.Toggle(HMD_UseRotationVelocity,"Rotation Velocity");
        HMD_UseRotationAcceleration = GUILayout.Toggle(HMD_UseRotationAcceleration,"Rotation Acceleration");
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        

        if(GUILayout.Button("SAVE"))
        {
            DashboardRefs _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
            _ref.headMountedDisplay.allowedAttributes._Postion = HMD_UsePos;
            _ref.headMountedDisplay.allowedAttributes._PositionVelocity = HMD_UsePositionVelocity;
            _ref.headMountedDisplay.allowedAttributes._PositionAcceleration = HMD_UsePositionAcceleration;
            _ref.headMountedDisplay.allowedAttributes._Rotation = HMD_UseRot;
            _ref.headMountedDisplay.allowedAttributes._RotationVelocity = HMD_UseRotationVelocity;
            _ref.headMountedDisplay.allowedAttributes._RotationAcceleration = HMD_UseRotationAcceleration;
            if (HMD_UsePos && HMD_UsePositionVelocity && HMD_UsePositionAcceleration && HMD_UseRot && HMD_UseRotationVelocity && HMD_UseRotationAcceleration)
                _ref.headMountedDisplay.AllowAllAttributes = true;
        }
    }
}
