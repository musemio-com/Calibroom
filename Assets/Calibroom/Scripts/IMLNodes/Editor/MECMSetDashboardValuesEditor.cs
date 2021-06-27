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
            //DashboardEditorWindow.SetupScore
            //    (
            //    setValuesNode.VisuoSpatial, 
            //    setValuesNode.SpeedProcessing
            //    );
            {
                float visuoSpacialScore = (setValuesNode.OverallScore / 7132f) * 10f;
                float SpeedProcessingScore = ((setValuesNode.SpeedProcessing + setValuesNode.CycleTime) / (2812f + 9410f)) * 10f;
                DashboardEditorWindow.SetupScore(visuoSpacialScore, SpeedProcessingScore);
            }
        }
    }

}
