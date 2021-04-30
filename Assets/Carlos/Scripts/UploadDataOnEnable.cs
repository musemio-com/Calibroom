using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using System.IO;

namespace MECM
{
    /// <summary>
    /// Uploads the data from the user at the OnEnable event
    /// </summary>
    [RequireComponent(typeof(UserDetailsController))]
    public class UploadDataOnEnable : MonoBehaviour
    {
        /// <summary>
        /// Used to load data
        /// </summary>
        private UserDetailsController m_UserDetailsController;
        /// <summary>
        /// Used to upload data to server
        /// </summary>
        private FirebaseController m_FirebaseController;

        private void Awake()
        {
            // Get refs to variables
            if (m_UserDetailsController == null)
                m_UserDetailsController = GetComponent<UserDetailsController>();
            if (m_FirebaseController == null)
                m_FirebaseController = FindObjectOfType<FirebaseController>();

        }
        
        private void OnEnable()
        {
            UploadData();
        }

        /// <summary>
        /// Loads the user ID and uploads the data to the server
        /// </summary>
        private void UploadData()
        {
            string userID = "";
            // Load user ID
            if (m_UserDetailsController != null)
            {
                userID = m_UserDetailsController.LoadUserID().ToString();
            }
            else
            {
                Debug.LogError("Unable to load User Details Controller, abort upload!");
                return;
            }

            // Upload all local data from user ID to server
            if (m_FirebaseController != null)
            {
                string userDataDirectory = Path.Combine(IMLDataSerialization.GetTrainingExamplesDataPath(), userID);
                m_FirebaseController.UploadAsync(userDataDirectory, userID);
            }
            else
            {
                Debug.LogError("Unable to find Firebase Controller, abort upload!");
                return;
            }

        }

    }

}
