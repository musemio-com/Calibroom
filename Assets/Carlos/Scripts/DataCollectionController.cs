using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;


namespace MECM
{
    /// <summary>
    /// Controls when data starts being collected and when it stops being collected
    /// </summary>
    [ExecuteAlways]
    public class DataCollectionController : MonoBehaviour
    {

        #region Variables

        /// <summary>
        /// Events that fires the toggle data collection on/off (used outside of IMLGraph)
        /// </summary>
        [SerializeField]
        private bool m_ToggleCollectDataEvent;

        /// <summary>
        /// Toggles data collection on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleDataCollection;


        /// <summary>
        /// Events that fires the toggle run model on/off (used outside of IMLGraph)
        /// </summary>
        [SerializeField]
        private bool m_ToggleTrainModelEvent;
        /// <summary>
        /// Toggles run model on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleTrainModel;


        /// <summary>
        /// Events that fires the toggle run model on/off (used outside of IMLGraph)
        /// </summary>
        [SerializeField]
        private bool m_ToggleRunModelEvent;
        /// <summary>
        /// Toggles run model on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleRunModel;

        /// <summary>
        /// User ID that we are collecting data from
        /// </summary>
        [SendToIMLGraph]
        public int UserID;

        #endregion

        #region Unity Messages
        // Update is called once per frame
        void Update()
        {
            // If we reach to this script with the toggle true, make sure to restart it
            if (ToggleDataCollection)
            {
                ToggleDataCollection = false;
            }
            if (ToggleTrainModel)
            {
                ToggleTrainModel = false;
            }
            if (ToggleRunModel)
            {
                ToggleRunModel = false;
            }

            // If the event was fired, flag data collection toggle to true
            if (m_ToggleCollectDataEvent || ToggleDataCollection)
            {
                ToggleDataCollection = true;
                m_ToggleCollectDataEvent = false;
                Debug.Log("Toggling data collection!");
            }
            if (m_ToggleTrainModelEvent || ToggleTrainModel)
            {
                ToggleTrainModel = true;
                m_ToggleTrainModelEvent = false;
                Debug.Log("Toggling Train Model!");
            }
            if (m_ToggleRunModelEvent || ToggleRunModel)
            {
                ToggleRunModel = true;
                m_ToggleRunModelEvent = false;
                Debug.Log("Toggling Run Model!");
            }


        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Fires the collect data event (toggles on/off collecting data)
        /// </summary>
        /// <returns></returns>
        public void FireToggleCollectDataEvent()
        {
            m_ToggleCollectDataEvent = true;
        }

        /// <summary>
        /// Fires the Train model event (toggles on/off model inference)
        /// </summary>
        /// <returns></returns>
        public void FireToggleTrainModelEvent()
        {
            m_ToggleTrainModelEvent = true;
        }

        /// <summary>
        /// Fires the run model event (toggles on/off model inference)
        /// </summary>
        /// <returns></returns>
        public void FireToggleRunModelEvent()
        {
            m_ToggleRunModelEvent = true;
        }


        #endregion

    }

}
