using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace MECM
{
    public class GetDataCollectionFinished : IMLNode, IUpdatableIML
    {
        /// <summary>
        /// Is the data collection finished?
        /// </summary>
        [Output]
        public bool DataCollectionFinished;

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_isUpdated; set => m_isUpdated = value; }
        private bool m_isUpdated;

        public override void Initialize()
        {
            base.Initialize();
            UpdateEditorData._OnTaskTimeDelegate += UpdateDataCollectionFinished;
        }

        public void Update()
        {
            // Only allow the flag to stay true for a frame, emulating an event
            if (DataCollectionFinished) DataCollectionFinished = false;
        }

        /// <summary>
        /// Updates data collection finished flag to true
        /// </summary>
        /// <param name="emptyTime">It does nothing. Just declaring to comply with delegate</param>
        public void UpdateDataCollectionFinished(float emptyTime)
        {
            DataCollectionFinished = true;
            Debug.Log("DATA COLLECTION FINISHED IN IML GRAPH!!");
        }
    }

}
