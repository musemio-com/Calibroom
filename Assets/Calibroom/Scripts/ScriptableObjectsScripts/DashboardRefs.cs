using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DashboardRefs : ScriptableObject
{
    public int userID;
    public Controller rightHandController;
    public Controller leftHandController;
    public Controller headMountedDisplay;
    public bool TrackOnSceneActive;
    public UploadSettings uploadSettings;
}

[Serializable]
public class Controller
{
    public string ControllerNameRef;
    public AllowedAttributes allowedAttributes;
}
[Serializable]
public class AllowedAttributes
{
    public AllowedAttributes(bool _Pos,bool _PosVel,bool _PosAcc,bool _Rot,bool _RotVel,bool _RotAcc,bool _GrabbedObj)
    {
        _Postion = _Pos;
        _PositionVelocity = _PosVel;
        _PositionAcceleration = _PosAcc;
        _Rotation = _Rot;
        _RotationVelocity = _RotVel;
        _RotationAcceleration = _RotAcc;
        GrabbedObject = _GrabbedObj;
    }
    public bool _Postion;
    public bool _PositionVelocity;
    public bool _PositionAcceleration;

    public bool _Rotation;
    public bool _RotationVelocity;
    public bool _RotationAcceleration;
    public bool GrabbedObject;
}
[Serializable]
public class UploadSettings
{
    public bool enable;
    public string firebaseStorageID;
}
