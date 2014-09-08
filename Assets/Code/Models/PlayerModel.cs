using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerModel
{
    public GameObject GameObject { get; set; }
    public PlayerData PlayerData { get; set; }

    public PlayerState PlayerState { get; set; }
    public float Health { get; set; }
    public float HeightRatio { get; set; }
    public float TargetX { get; set; }
    public float SpeedRatio { get; set; }
    public float MaxSpeed { get; set; }
    public bool ShouldShowSelectionBox { get; set; }
    public Action SelectCallback { get; set; }

    public PlayerModel()
    {
        PlayerState = PlayerState.Idle;
        Health = 1;
        HeightRatio = 0;
        TargetX = 0;
        SpeedRatio = 1;
        MaxSpeed = 4;
        ShouldShowSelectionBox = false;
        SelectCallback = null;
    }
}
