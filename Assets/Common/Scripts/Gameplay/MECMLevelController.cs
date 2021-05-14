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

        /// <summary>
        /// Gets the user ID from the UI (used to pass the id to something else)
        /// </summary>
        private UISelectIDController m_UISelectIDCtrlr;
             
        // Awake is called before start
        private void Awake()
        {
            m_LevelLoader = GetComponent<GameLevelController>();
            m_UserDetailsCtrlr = GetComponent<UserDetailsController>();
            m_UISelectIDCtrlr = FindObjectOfType<UISelectIDController>();
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
                    int userID = 0;
                    if (m_UISelectIDCtrlr != null)
                    {
                        userID = m_UISelectIDCtrlr.GetUserIDInt();
                        m_UserDetailsCtrlr.SaveUserID(userID);
                    }
                    else
                        Debug.LogError("UISelectIDController not found in sceneOpen!");

                }

                // Attempt to load next level
                LoadPuzzleLevel(m_LevelLoader.IndexCurrentLevel, m_UserDetailsCtrlr.LoadUserID(), m_LevelLoader);

            }
        }

        /// <summary>
        /// Loads one of the puzzle levels 
        /// 0 = sceneOpen
        /// 1 = room tutorial
        /// 2 = room A
        /// 3 = room B
        /// 4 = room C
        /// 5 = room D
        /// 6 = sceneClose
        /// </summary>
        /// <param name="index"></param>
        private void LoadPuzzleLevel(int index, int userID, GameLevelController levelLoader)
        {

            // Load next level, depending on the userID
            if (userID <= 30 || userID > 80)
            {
                // Check in which scene are we in
                switch (index)
                {
                    // sceneOpen
                    case 0:
                        levelLoader.LoadNextLevel(); // next is 1 (room tutorial)
                        break;
                    // room tutorial
                    case 1:
                        levelLoader.LoadNextLevel(); // next is 2 (room A)
                        break;
                    // room A
                    case 2:
                        levelLoader.LoadNextLevel(); // next is 3 (room B)
                        break;
                    // room B
                    case 3:
                        levelLoader.LoadGameLevel(6); // 6 is sceneClose
                        break;
                    default:
                        break;
                }
            }
            else if (userID > 30 )
            {
                // Check in which scene are we in
                switch (index)
                {
                    // sceneOpen
                    case 0:
                        levelLoader.LoadNextLevel(); // next is 1 (room tutorial)
                        break;
                    // room tutorial
                    case 1:
                        levelLoader.LoadGameLevel(4); // skip to 4 (room C)
                        break;
                    // room C
                    case 4:
                        levelLoader.LoadNextLevel(); // next is 5 (room D)
                        break;
                    // room D
                    case 5:
                        levelLoader.LoadNextLevel(); // next is 6 (sceneClose)
                        break;
                    default:
                        break;
                }

            }

        }
    }

}
