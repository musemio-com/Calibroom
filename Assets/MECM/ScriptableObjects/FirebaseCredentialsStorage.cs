using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirebaseDataObject", menuName = "MECM/FirebaseDataObject", order = 2)]
public class FirebaseCredentialsStorage : ScriptableObject
{
    public string FirebaseURL;
    public string FirebaseID;
    public string UploadPath;
}
