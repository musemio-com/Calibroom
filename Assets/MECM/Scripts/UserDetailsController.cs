using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MECM
{
    /// <summary>
    /// Manages save/load of user details into an SO
    /// </summary>
    public class UserDetailsController : MonoBehaviour
    {
        /// <summary>
        /// Data container from user details (ID, etc)
        /// </summary>
        public UserDetails UserData;

        // Start is called before the first frame update
        void Start()
        {
            if (UserData == null)
                Debug.LogError("UserDetails SO is not loaded! Drag and drop it to the UserDetailsController of the LevelLoader gameobject");

        }

        public void SaveUserID(int id)
        {
            if (UserData != null)
                UserData.UserID = id;
            else
                Debug.LogError("UserDetails SO is not loaded! Drag and drop it to the UserDetailsController of the LevelLoader gameobject");
        }

        public int LoadUserID()
        {
            if (UserData != null)
                return UserData.UserID;
            else
            {
                Debug.LogError("UserDetails SO is not loaded! Drag and drop it to the UserDetailsController of the LevelLoader gameobject");
                return -1;
            }
        }

    }

}
