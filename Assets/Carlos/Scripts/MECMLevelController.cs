using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MECM
{
    /// <summary>
    /// Wrapper that manages specific requirements for level loading in Calibroom
    /// </summary>
    [RequireComponent(typeof(GameLevelController), typeof(UserDetailsController))]
    public class MECMLevelController : MonoBehaviour
    {
        /// <summary>
        /// Instance of the level loader we are wrapping
        /// </summary>
        private GameLevelController m_LevelLoader;
        
        /// <summary>
        /// 0 - 1f level loading progress
        /// </summary>
        public float LoadingProgress { get { return m_LevelLoader.LoadingProgress; } }

        /// <summary>
        /// Handles loading/saving of user details (id, etc...)
        /// </summary>
        private UserDetailsController m_UserDetailsCtrlr;

        // Awake is called before start
        private void Awake()
        {
            m_LevelLoader = GetComponent<GameLevelController>();
            m_UserDetailsCtrlr = GetComponent<UserDetailsController>();
        }

        /// <summary>
        /// Loads next level, taking into account current user ID
        /// </summary>
        public void LoadNextLevel()
        {
            if (m_LevelLoader != null && m_UserDetailsCtrlr != null)
            {
                // If we are in the sceneOpen (where the user selects the user id)
                if (m_LevelLoader.IndexCurrentLevel == 0)
                {
                    // Save User ID
                    var userIDCtrlr = FindObjectOfType<UISelectIDController>();
                    int userID = 0;
                    if (userIDCtrlr != null)
                    {
                        userID = userIDCtrlr.GetUserIDInt();
                        m_UserDetailsCtrlr.SaveUserID(userID);
                    }
                    else
                        Debug.LogError("UISelectIDController not found in sceneOpen!");                        
                }

                // Load next level, depending on the userID
                if (m_UserDetailsCtrlr.LoadUserID() <= 30)
                {
                    // We load next level (level 1, we are in level 0)
                    m_LevelLoader.LoadNextLevel();
                }
                else if (m_UserDetailsCtrlr.LoadUserID() > 30)
                {
                    // Load room C (level 4?)
                    Debug.Log("Need to implement rooms C and D");
                }
            }
        }
    }

}
