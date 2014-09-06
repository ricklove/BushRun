using UnityEngine;
using System.Collections;

public class CharacterSelectionMenu : MonoBehaviour
{
    public static CharacterSelectionMenu Instance;

    void Awake()
    {
        Instance = this;
    }

    public PlayerInfo[] playerInfos;

    public PlayerInfo SelectedPlayerInfo{ get; set; }
    
    private GameObject _player;
    private GameObject _playerHolder;

    void Start()
    {
        _player = transform.FindChild("Player").gameObject;
        _playerHolder = transform.FindChild("PlayerHolder").gameObject;

        _player.SetActive(false);

        var i = 0;

        foreach (var pInfo in playerInfos)
        {
            var c = (Instantiate(_player.transform) as Transform).gameObject;
            var cp = c.GetComponent<PlayerSelectable>();
            cp.playerInfo = pInfo;

            c.transform.parent = _playerHolder.transform;
            c.transform.localPosition = new Vector3(2 * i, 0, 0);
            c.SetActive(true);

            i++;
        }

        SelectedPlayerInfo = playerInfos [0];
    }
    
    void Update()
    {
    
    }

    public void SelectPlayer(PlayerInfo playerInfo, GameObject player)
    {
        _playerHolder.transform.position = -player.transform.localPosition;

        if (SelectedPlayerInfo == playerInfo)
        {
            UsePlayer(playerInfo);
        }

        SelectedPlayerInfo = playerInfo;
    }

    public void UsePlayer(PlayerInfo playerInfo)
    {
        MenuController.Instance.ReturnFromMenu(MenuState.CharacterSelection);
    }
}
