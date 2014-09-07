using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerModel
{
    public GameObject GameObject { get; set; }

    public float Health { get; set; }
    public PlayerData PlayerData { get; set; }
    public PlayerState PlayerState { get; set; }

    public float HeightRatio { get; set; }
    public float TargetX { get; set; }
}
