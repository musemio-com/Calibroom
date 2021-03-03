using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;


namespace MECM
{
    /// <summary>
    /// Controls when data starts being collected and when it stops being collected
    /// </summary>
    public class DataCollectionController : MonoBehaviour
    {

        #region Variables

        private bool m_StartDataCollection;

        private bool m_StopDataCollection;

        /// <summary>
        /// Event that toggles data collection on/off
        /// </summary>
        [SendToIMLGraph]
        public bool ToggleDataCollection;

        #endregion

        #region Unity Messages
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

    }

}
