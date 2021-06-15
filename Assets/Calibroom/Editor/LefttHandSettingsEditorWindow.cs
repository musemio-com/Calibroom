using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LefttHandSettingsEditorWindow : EditorWindow
{

    #region Left Hand Settings
    bool L_UsePos = true;
    bool L_UsePositionVelocity = true;
    bool L_UsePositionAcceleration = true;
    bool L_UseRot = true;
    bool L_UseRotationVelocity = true;
    bool L_UseRotationAcceleration = true;
    bool L_UseGrabbedObject = true;
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
            DashboardRefs _ref = Resources.Load<DashboardRefs>("ScriptableObjects/DashboardRefs");
            _ref.leftHandController.allowedAttributes._Postion = L_UsePos;
            _ref.leftHandController.allowedAttributes._PositionVelocity = L_UsePositionVelocity;
            _ref.leftHandController.allowedAttributes._PositionAcceleration = L_UsePositionAcceleration;
            _ref.leftHandController.allowedAttributes._Rotation = L_UseRot;
            _ref.leftHandController.allowedAttributes._RotationVelocity = L_UseRotationVelocity;
            _ref.leftHandController.allowedAttributes._RotationAcceleration = L_UseRotationAcceleration;
            _ref.leftHandController.allowedAttributes.GrabbedObject = L_UseGrabbedObject;
            if (L_UsePos && L_UsePositionVelocity && L_UsePositionAcceleration && L_UseRot && L_UseRotationVelocity && L_UseRotationAcceleration && L_UseGrabbedObject)
                _ref.leftHandController.AllowAllAttributes = true;
        }
    }
}
