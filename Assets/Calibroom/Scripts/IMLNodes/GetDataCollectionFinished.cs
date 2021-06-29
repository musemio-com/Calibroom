using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using XNode;

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
            UpdateEditorData._OnTaskCompletion = UpdateDataCollectionFinished;
        }

        public override object GetValue(NodePort port)
        {
            return DataCollectionFinished;
        }

        public void Update()
        {
            // Make sure that static delegate uses our UpdateDataCollection
            if (UpdateEditorData._OnTaskCompletion == null) UpdateEditorData._OnTaskCompletion = UpdateDataCollectionFinished;
        }

        public void LateUpdate()
        {
            // Only allow the flag to stay true for a frame, emulating an event
            if (DataCollectionFinished) 
                DataCollectionFinished = false;
        }


        public void OnDisable()
        {
            UpdateEditorData._OnTaskCompletion -= UpdateDataCollectionFinished;
        }

        /// <summary>
        /// Updates data collection finished flag to true
        /// </summary>
        /// <param name="emptyTime">It does nothing. Just declaring to comply with delegate</param>
        public void UpdateDataCollectionFinished()
        {
            DataCollectionFinished = true;
            //Debug.Log("DATA COLLECTION FINISHED IN IML GRAPH!!");
        }

    }

}
