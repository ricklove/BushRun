using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerSelectable : MonoBehaviour
{
    public PlayerInfo playerInfo;
    private HeadSelection _headSelection;

    void Start()
    {
        _headSelection = GetComponentInChildren<HeadSelection>();
    }

    void Update()
    {
        var allHeads = playerInfo.IdleHeads.Union(playerInfo.HappyHeads).Union(playerInfo.HurtHeads).ToArray();
        _headSelection.idleHeads = allHeads;
    }

    void OnMouseDown()
    {
        CharacterSelectionMenu.Instance.SelectPlayer(playerInfo, gameObject);
    }
}

[System.Serializable]
public class PlayerInfo
{
    public string PlayerID;
    public Sprite[] IdleHeads;
    public Sprite[] HappyHeads;
    public Sprite[] HurtHeads;

    public static string CurrentPlayerID
    {
        get
        {
            return PlayerPrefs.GetString("PlayerID", "Player1");
        }
        set
        {
            PlayerPrefs.SetString("PlayerID", value);
        }
    }
}