using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterSelectionMenu : MonoBehaviour
{
    public static CharacterSelectionMenu Instance;

    void Awake()
    {
        Instance = this;
    }

    public PlayerInfo[] playerInfos;

    public PlayerInfo SelectedPlayerInfo { get; set; }

    private GameObject _player;
    private GameObject _group;
    private GameObject _playerHolder;
    private GameObject[] _players;

    void Start()
    {
        _player = transform.FindChild("Player").gameObject;
        _group = transform.FindChild("Group").gameObject;
        _playerHolder = _group.transform.FindChild("PlayerHolder").gameObject;

        _player.SetActive(false);

        var i = 0;

        var players = new List<GameObject>();

        foreach (var pInfo in playerInfos)
        {
            var c = (Instantiate(_player.transform) as Transform).gameObject;
            var cp = c.GetComponent<PlayerSelectable>();
            cp.playerInfo = pInfo;

            c.transform.parent = _playerHolder.transform;
            c.transform.localPosition = new Vector3(2 * i, 0, 0);
            c.SetActive(true);

            players.Add(c);

            i++;
        }

        SelectedPlayerInfo = null;

        var cPlayer = MainModel.Instance.PlayerDataModel.PlayerID;
        var m = players.FirstOrDefault(p => p.GetComponent<PlayerSelectable>().playerInfo.PlayerID == cPlayer);
        if (m != null)
        {
            SelectPlayer(m.GetComponent<PlayerSelectable>().playerInfo, m);
        }
        else
        {
            SelectedPlayerInfo = playerInfos[0];
        }
    }

    void Update()
    {

    }

    public void SelectPlayer(PlayerInfo playerInfo, GameObject player)
    {
        _playerHolder.transform.localPosition = -player.transform.localPosition;

        if (SelectedPlayerInfo == playerInfo)
        {
            UsePlayer(playerInfo);
        }

        SelectedPlayerInfo = playerInfo;
    }

    public void UsePlayer(PlayerInfo playerInfo)
    {
        MenuController.Instance.ReturnFromMenu(MenuState.CharacterSelection);
        MainModel.Instance.PlayerDataModel.PlayerID = playerInfo.PlayerID;
    }
}
