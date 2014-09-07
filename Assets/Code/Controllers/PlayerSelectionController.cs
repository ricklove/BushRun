using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class PlayerSelectionController : MonoBehaviour
{
    private GameObject _playerPrefab;
    private bool _isSetup = false;

    void Update()
    {
        var model = MainModel.Instance;

        if (model.ScreenState != ScreenState.PlayerSelection)
        {
            DisableScreen();
            _isSetup = false;
            return;
        }

        if (!_isSetup)
        {
            _isSetup = true;
            // Create a player selection for each Active player
            if (_playerPrefab == null)
            {
                _playerPrefab = transform.root.FindChild("Players").FindChild("PlayerPrefab").gameObject;
                _playerPrefab.SetActive(false);
            }

            if (model.Players.Count != model.PlayerDataModel.AvailablePlayers.Count)
            {
                foreach (var playerData in model.PlayerDataModel.AvailablePlayers)
                {
                    if (!model.Players.Any(p => p.PlayerData == playerData))
                    {

                        var playerModel = new PlayerModel();
                        model.Players.Add(playerModel);

                        var playerGameObject = (Instantiate(_playerPrefab.transform) as Transform).gameObject;
                        playerGameObject.transform.parent = _playerPrefab.transform.parent;
                        playerGameObject.SetActive(true);
                        playerGameObject.GetComponent<PlayerView>().PlayerViewModel = new PlayerViewModel(playerModel);

                        playerModel.GameObject = playerGameObject;
                        playerModel.PlayerData = playerData;

                        playerModel.GameObject.transform.localPosition = new Vector3(-1000, 0, 0);
                    }
                }
            }



            // Move players to good position
            var cam = transform.root.FindChild("MainCamera");
            var camX = cam.transform.position.x;

            var min = camX - 3f;
            var max = camX + 3f;

            var screenRadius = cam.GetComponent<Camera>().orthographicSize;

            var change = (max - min) / model.Players.Count;

            for (int i = 0; i < model.Players.Count; i++)
            {
                var x = min + change * i;

                model.Players[i].TargetX = x;

                // Move near if far away
                var trans = model.Players[i].GameObject.transform;

                if (trans.localPosition.x > x + screenRadius)
                {
                    trans.localPosition = new Vector3(max + screenRadius + (i * change), 0, 0);
                }
                else if (trans.localPosition.x < x - screenRadius)
                {
                    trans.localPosition = new Vector3(min - screenRadius - (i * change), 0, 0);
                }
            }


        }

    }

    void DisableScreen()
    {

    }
}
