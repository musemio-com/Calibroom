using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrackersInfoObject", menuName = "MECM/TrackersInfo", order = 1)]
public class TrackersInfoScriptableObject : ScriptableObject
{
    public bool R_UsePosition;
    public bool R_UseRotation;
    public bool R_UseGrabbedObjects;
    public bool L_UsePosition;
    public bool L_UseRotation;
    public bool L_UseGrabbedObjects;
    public bool HMD_UsePosition;
    public bool HMD_UseRotation;
}
