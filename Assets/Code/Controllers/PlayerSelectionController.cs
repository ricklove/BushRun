using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class PlayerSelectionController : MonoBehaviour
{
    public float maxTimeToMove = 30f;

    private GameObject _playerPrefab;
    private bool _isSetup = false;
    private float _timeToMove = 0;

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
            _timeToMove = Time.time + maxTimeToMove;

            // Create a player selection for each Active player
            if (_playerPrefab == null)
            {
                _playerPrefab = transform.root.FindChild("Players").FindChild("PlayerPrefab").gameObject;
                _playerPrefab.SetActive(false);
            }

            if (model.AvailablePlayers.Count != model.PlayerDataModel.AvailablePlayers.Count)
            {
                foreach (var playerData in model.PlayerDataModel.AvailablePlayers)
                {
                    if (!model.AvailablePlayers.Any(p => p.PlayerData == playerData))
                    {

                        var playerModel = new PlayerModel();
                        model.AvailablePlayers.Add(playerModel);

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

            // Enable selection
            foreach (var p in model.AvailablePlayers)
            {
                var pLocal = p;

                pLocal.SelectCallback = () =>
                {
                    model.ActivePlayer.ShouldShowSelectionBox = false;
                    model.ActivePlayer.PlayerState = PlayerState.Hurt;
                    model.ActivePlayer = pLocal;
                    model.ActivePlayer.ShouldShowSelectionBox = true;
                    model.ActivePlayer.PlayerState = PlayerState.Happy;
                };
            }

            // Move players to good position
            PositionPlayers(model.AvailablePlayers);

            if (model.ActivePlayer == null)
            {
                model.ActivePlayer = model.AvailablePlayers[0];
            }

            model.ActivePlayer.ShouldShowSelectionBox = true;
        }

        // Make them move around after awhile
        if (Time.time > _timeToMove)
        {
            _timeToMove = Time.time + Random.Range(maxTimeToMove * 0.5f, maxTimeToMove);

            foreach (var p in model.AvailablePlayers)
            {
                p.SpeedRatio = 0.5f;
            }

            PositionPlayers(model.AvailablePlayers.ToList().RandomizeOrder());
        }

    }

    private void PositionPlayers(List<PlayerModel> players)
    {
        var cam = transform.root.FindChild("MainCamera");
        var camX = cam.transform.position.x;

        var min = camX - 3f;
        var max = camX + 3f;

        var screenRadius = cam.GetComponent<Camera>().orthographicSize;

        var change = (max - min) / players.Count;

        for (int i = 0; i < players.Count; i++)
        {
            var x = min + change * i;

            players[i].TargetX = x;

            // Move near if far away
            var trans = players[i].GameObject.transform;

            if (trans.localPosition.x > camX + screenRadius)
            {
                trans.localPosition = new Vector3(max + screenRadius + (i * change), 0, 0);
            }
            else if (trans.localPosition.x < camX - screenRadius)
            {
                trans.localPosition = new Vector3(min - screenRadius - (i * change), 0, 0);
            }
        }
    }

    void DisableScreen()
    {
        var model = MainModel.Instance;

        foreach (var p in model.AvailablePlayers)
        {
            p.ShouldShowSelectionBox = false;
            p.SelectCallback = null;
            p.SpeedRatio = 1f;
        }

    }
}
