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
        PlayerDataModel = new PlayerDataModel();
        Players = new List<PlayerModel>();
    }

    public PlayerDataModel PlayerDataModel { get; private set; }
    public List<PlayerModel> Players { get; private set; }

    public ScreenState ScreenState { get; set; }
}

public enum ScreenState
{
    PlayerSelection,
}

