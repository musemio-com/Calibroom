using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;
using UnityEditor;
using MECM;

namespace MECM
{
    [NodeWidth(300)]
    public class MECMSetDashboardValues : IMLNode, IFeatureIML, IUpdatableIML
    {
        [Input] public float OverallScore;
        [Input] public float VisuoSpatial;
        [Input] public float SpeedProcessing;
        [Input] public float CycleTime;
        [Input] public bool SetScore;

        public IMLBaseDataType FeatureValues => throw new System.NotImplementedException();

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_IsUpdated; set { m_IsUpdated = value; } }
        private bool m_IsUpdated;

        public object UpdateFeature()
        {
            //OverallScore = GetInputValue<float>("OverallScore");
            VisuoSpatial = GetInputValue<float>("VisuoSpatial");
            SpeedProcessing = GetInputValue<float>("SpeedProcessing");
            SetScore = GetInputValue<bool>("SetScore");
            if (SetScore)
            {
                //CycleTime = GetInputValue<float>("CycleTime");
                UpdateScore._OnUpdateScore(VisuoSpatial, SpeedProcessing, CycleTime);
                Debug.Log($"Sending Score to Dashboard {VisuoSpatial} and {SpeedProcessing}");
            }

            return this;
        }

        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            // allow setScore for a frame emulating event
            if (SetScore) SetScore = false;
        }
    }

    public class UpdateScore
    {
        public delegate void UpdateScoreDelegate(float t, float t2, float t3);
        public static UpdateScoreDelegate _OnUpdateScore;
    }
}