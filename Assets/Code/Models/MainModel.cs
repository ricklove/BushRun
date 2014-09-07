using System.Collections.Generic;
using System.Linq;

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

    private MainModel() {
        CameraModel = new CameraModel();
        PlayerDataModel = new PlayerDataModel();
        AvailablePlayers = new List<PlayerModel>();

        ScreenState = ScreenState.PlayerSelection;
        ActivePlayer = null;
    }

    public ScreenState ScreenState { get; set; }

    public CameraModel CameraModel { get; private set; }
    public PlayerDataModel PlayerDataModel { get; private set; }

    public List<PlayerModel> AvailablePlayers { get; private set; }
    public PlayerModel ActivePlayer { get; set; }

}

public enum ScreenState
{
    PlayerSelection,
    LevelSelection,
}

