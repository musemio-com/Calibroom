using UnityEngine;
using InteractML;
using MECM;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace MECM
{
    [CustomNodeEditor(typeof(MECMSetDashboardValues))]
    public class MECMSetDashboardValuesEditor : IMLNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            var setValuesNode = target as MECMSetDashboardValues;
            // Set values on dashboard window
            if (setValuesNode != null)
                DashboardEditorWindow.SetupScore
                    (
                    setValuesNode.OverallScore, 
                    setValuesNode.VisuoSpatial, 
                    setValuesNode.SpeedProcessing, 
                    setValuesNode.CycleTime
                    );
        }
    }

}
