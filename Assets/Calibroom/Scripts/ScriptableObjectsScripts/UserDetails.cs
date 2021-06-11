﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MECM
{
    /// <summary>
    /// Holds information about a user
    /// </summary>
    public class UserDetails : ScriptableObject
    {
        /// <summary>
        /// ID of the user (to be used when saving data, loading certain scenes)
        /// </summary>
        public int UserID;
    }

}