using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RightHandSettingsEditorWindow : EditorWindow
{

    #region Right Hand Settings
    bool R_UsePos = true;
    bool R_UsePositionVelocity = true;
    bool R_UsePositionAcceleration = true;
    bool R_UseRot = true;
    bool R_UseRotationVelocity = true;
    bool R_UseRotationAcceleration = true;
    bool R_UseGrabbedObject = true;
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
            DashboardRefs _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
            _ref.rightHandController.allowedAttributes._Postion = R_UsePos;
            _ref.headMountedDisplay.allowedAttributes._PositionVelocity = R_UsePositionVelocity;
            _ref.rightHandController.allowedAttributes._PositionAcceleration = R_UsePositionAcceleration;
            _ref.rightHandController.allowedAttributes._Rotation = R_UseRot;
            _ref.rightHandController.allowedAttributes._RotationVelocity = R_UseRotationVelocity;
            _ref.rightHandController.allowedAttributes._RotationAcceleration = R_UseRotationAcceleration;
            _ref.rightHandController.allowedAttributes.GrabbedObject = R_UseGrabbedObject;
            if (R_UsePos && R_UsePositionVelocity && R_UsePositionAcceleration && R_UseRot && R_UseRotationVelocity && R_UseRotationAcceleration && R_UseGrabbedObject)
                _ref.rightHandController.AllowAllAttributes = true;
        }

    }
}
