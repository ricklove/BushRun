using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerViewModel : IPlayerViewModel
{
    private PlayerModel _model;

    public PlayerViewModel(PlayerModel model)
    {
        _model = model;
    }

    public PlayerData PlayerData { get { return _model.PlayerData; } }
    public PlayerState PlayerState { get { return _model.PlayerState; } }
    public float HeightRatio { get { return _model.HeightRatio; } }
    public float TargetX { get { return _model.TargetX; } }
    public bool ShouldShowSelectionBox { get { return _model.ShouldShowSelectionBox; } }
    public float SpeedRatio { get { return _model.SpeedRatio; } }
}
