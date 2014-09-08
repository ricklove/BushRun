using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainModel
{
    private static MainModel _instance;

    public static MainModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MainModel();
            }

            return _instance;
        }
        set { _instance = value; }
    }

    private MainModel()
    {
        CameraModel = new CameraModel();
        PlayerDataModel = new PlayerDataModel();
        AvailablePlayers = new List<PlayerModel>();

        ScreenState = ScreenState.PlayerSelection;
        ChoicesModel = new ChoicesModel();
        SubjectModel = new SubjectModel();
    }

    public ScreenState ScreenState { get; set; }

    public CameraModel CameraModel { get; private set; }
    public PlayerDataModel PlayerDataModel { get; private set; }

    public List<PlayerModel> AvailablePlayers { get; private set; }

    private PlayerModel _activePlayer;

    public PlayerModel ActivePlayer
    {
        get
        {
            if (_activePlayer == null)
            {
                var playerID = PlayerPrefs.GetString("PlayerID", "Caleb");
                var player = AvailablePlayers.FirstOrDefault(p => p.PlayerData.ID == playerID);
                _activePlayer = player;
            }

            return _activePlayer;
        }
        set
        {
            _activePlayer = value;
            PlayerPrefs.SetString("PlayerID", _activePlayer.PlayerData.ID);
        }
    }

    public int ActiveLevel { get; set; }

    public ChoicesModel ChoicesModel { get; set; }
    public SubjectModel SubjectModel { get; private set; }
}

public enum ScreenState
{
    PlayerSelection,
    LevelSelection,
    Game
}

