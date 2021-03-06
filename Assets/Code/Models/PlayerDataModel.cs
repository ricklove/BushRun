using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataModel
{
    private string _playerID;

    public string PlayerID
    {
        get
        {
            if (string.IsNullOrEmpty(_playerID))
            {
                _playerID = PlayerPrefs.GetString("PlayedID", "Player1");
            }

            return _playerID;
        }
        set
        {
            _playerID = value;
            PlayerPrefs.SetString("PlayedID", value);
        }
    }

    public PlayerData PlayerData { get; set; }
    public IList<PlayerData> AvailablePlayers { get; private set; }

    public PlayerDataModel()
    {
        AvailablePlayers = new List<PlayerData>();
        LoadStaticHeads();
    }

    private void LoadStaticHeads()
    {
        var names = new string[] { "Caleb", "Emily", "Lydia", "Matthew" };
        var avatars = new AvatarType[] { AvatarType.Robot, AvatarType.Guy, AvatarType.Guy, AvatarType.Robot };
        var types = System.Enum.GetValues(typeof(SpriteType)).Cast<SpriteType>();

        var sprites = Resources.LoadAll<Sprite>("");

        var i = 0;

        foreach (var n in names)
        {
            if (!sprites.Any(s => s.name.Contains(n)))
            {
                i++;
                continue;
            }

            var p = new PlayerData() { ID = n, AvatarType = avatars[i] };
            AvailablePlayers.Add(p);



            foreach (var ty in types)
            {
                var typeName = System.Enum.GetName(typeof(SpriteType), ty);
                var namePart = n + typeName;

                foreach (var s in sprites)
                {
                    if (s.name.Contains(namePart))
                    {
                        p.Sprites.Add(new SpriteInfo() { Sprite = s, SpriteType = ty });
                    }
                }
            }

            i++;
        }
    }
}

public class PlayerData
{
    public string ID { get; set; }
    public AvatarType AvatarType { get; set; }
    public List<SpriteInfo> Sprites { get; private set; }

    public PlayerData()
    {
        Sprites = new List<SpriteInfo>();
        AvatarType = AvatarType.Guy;
    }
}

public class SpriteInfo
{
    public SpriteType SpriteType { get; set; }
    public Sprite Sprite { get; set; }
}

public enum SpriteType
{
    HeadIdle,
    HeadHappy,
    HeadHurt,
}


public enum AvatarType
{
    Guy,
    Robot,
    //HeroBoy
}

