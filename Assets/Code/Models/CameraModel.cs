using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CameraModel
{
    public GameObject GameObject { get; set; }

    public bool ShouldFollowActivePlayer { get; set; }
    public float? TimeToMove { get; set; }

    public Vector3? TargetPosition { get; set; }
    public float? TargetSize { get; set; }

    public float ActivePlayerXOffset { get; set; }

    public CameraModel()
    {
        ShouldFollowActivePlayer = false;
        ActivePlayerXOffset = 3f;
        TargetPosition = null;
        TargetSize = null;
        TimeToMove = 0.5f;
    }
}
