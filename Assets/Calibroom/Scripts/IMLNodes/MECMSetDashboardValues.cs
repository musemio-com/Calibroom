﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using InteractML;
using UnityEditor;
using MECM;

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
            UpdateScore._OnUpdateScore(OverallScore, SpeedProcessing, CycleTime);

            return this;
        }
    }

    public class UpdateScore
    {
        public delegate void UpdateScoreDelegate(float t, float t2, float t3);
        public static UpdateScoreDelegate _OnUpdateScore;
    }
}
