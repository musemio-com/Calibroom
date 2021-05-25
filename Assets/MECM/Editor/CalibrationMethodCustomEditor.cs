using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[CustomEditor(typeof(CalibrationMethodManager))]
public class CalibrationMethodCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Apply"))
        {
            CalibrationMethodManager cmm = (CalibrationMethodManager)target;
            cmm.InitializeMECM();
            //MECMSetupEditorWindow.Open((CalibrationMethodManager)target);
        }
    }
}
