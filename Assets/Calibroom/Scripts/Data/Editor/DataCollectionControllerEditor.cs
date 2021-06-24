using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MECM
{
    [CustomEditor(typeof(DataCollectionController))]
    public class DataCollectionControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Debug button to toggle data collection on/off
            var dataCollector = (target as DataCollectionController);
            if (dataCollector.DebugMode)
            {
                if (GUILayout.Button("Toggle Data Collection"))
                {
                    dataCollector.ToggleCollectingData();
                }

            }
        }
    }

}
