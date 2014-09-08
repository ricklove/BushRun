using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CameraViewModel : ICameraViewModel
{
    private MainModel _model;

    public CameraViewModel(MainModel model)
    {
        _model = model;
    }

    public bool ShouldFollowActivePlayer { get { return _model.CameraModel.ShouldFollowActivePlayer; } }
    public float? ActivePlayerX { get { return _model.ActivePlayer != null ? _model.ActivePlayer.GameObject.transform.position.x : (float?)null; } }
    public float ActivePlayerXOffset { get { return _model.CameraModel.ActivePlayerXOffset; } }

    public float? TimeToMove { get { return _model.CameraModel.TimeToMove; } }

    public float? TargetSize { get { return _model.CameraModel.TargetSize; } }
    public Vector3? TargetPosition { get { return _model.CameraModel.TargetPosition; } }

}
