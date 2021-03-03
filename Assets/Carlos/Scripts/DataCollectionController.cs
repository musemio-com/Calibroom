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
        private bool m_CollectDataEvent;

        /// <summary>
        /// Toggles data collection on/off (used in IMLGraph)
        /// </summary>
        [SendToIMLGraph, HideInInspector]
        public bool ToggleDataCollection;

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

            // If the event was fired, flag data collection toggle to true
            if (m_CollectDataEvent || ToggleDataCollection)
            {
                ToggleDataCollection = true;
                m_CollectDataEvent = false;
            }

        }

        #endregion

    }

}
