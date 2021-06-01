using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirebaseDataObject", menuName = "MECM/FirebaseDataObject", order = 1)]
public class UploadInfoScriptableObject : ScriptableObject
{
    public bool UploadEnabled;
    public string FirebaseID;
}
