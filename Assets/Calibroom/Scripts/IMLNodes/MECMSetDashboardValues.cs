using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;

namespace MECM
{
    [NodeWidth(300)]
    public class MECMSetDashboardValues : IMLNode, IFeatureIML
    {
        [Input] public float OverallScore;
        [Input] public float VisuoSpatial;
        [Input] public float SpeedProcessing;
        [Input] public float CycleTime;

        public IMLBaseDataType FeatureValues => throw new System.NotImplementedException();

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_IsUpdated; set { m_IsUpdated = value; } }
        private bool m_IsUpdated;

        public object UpdateFeature()
        {
            OverallScore = GetInputValue<float>("OverallScore");
            VisuoSpatial = GetInputValue<float>("VisuoSpatial");
            SpeedProcessing = GetInputValue<float>("SpeedProcessing");
            CycleTime = GetInputValue<float>("CycleTime");
            return this;
        }
    }

}
