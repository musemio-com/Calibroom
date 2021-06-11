using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrackersInfoScriptableObject : ScriptableObject
{
    public AllowedCoord rightHandAllowedCoord;
    public AllowedCoord leftHandAllowedCoord;
    public AllowedCoord HeadMountedDisplayAllowedCoord;
    public bool StartTrackingOnSceneStart;
}

[Serializable]
public class AllowedCoord
{
    public bool _Postion;
    public bool _PositionVelocity;
    public bool _PositionAcceleration;

    public bool _Rotation;
    public bool _RotationVelocity;
    public bool _RotationAcceleration;
    public bool GrabbedObject;
}
// [Serializable]
// public class LeftHandAllowedCoord
// {
//     public bool _Postion;
//     public bool _PositionVelocity;
//     public bool _PositionAcceleration;
//     public RotValues _Rotation;
//     public RotValues _RotationVelocity;
//     public RotValues _RotationAcceleration;
//     public bool GrabbedObject;
// }
// [Serializable]
// public class HMDAllowedCoord
// {
//     public PosValues _Postion;
//     public PosValues _PositionVelocity;
//     public PosValues _PositionAcceleration;
//     public RotValues _Rotation;
//     public RotValues _RotationVelocity;
//     public RotValues _RotationAcceleration;
// }
// [Serializable]
// public struct PosValues
// {
//     public bool x;
//     public bool y;
//     public bool z;
// }
// [Serializable]
// public struct RotValues
// {
//     public bool x;
//     public bool y;
//     public bool z;
//     public bool w;
// }