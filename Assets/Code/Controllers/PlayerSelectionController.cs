using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class PlayerSelectionController : ScreenControllerBase
{
    public float maxTimeToMove = 30f;

    private GameObject _playerPrefab;
    private float _timeToMove = 0;

    private GameObject _forwardButton;
    //private Vector3 _forwardButtonInitialPosition;

    void Start()
    {
        _forwardButton = transform.FindChild("ButtonForward").gameObject;
        _forwardButton.GetComponent<Clickable>().MouseDownCallback = () =>
        {
            if (MainModel.Instance.ActivePlayer != null)
            {
                MainModel.Instance.ScreenState = ScreenState.LevelSelection;
            }
        };

        //_forwardButtonInitialPosition = _forwardButton.transform.localPosition;
    }

    protected override ScreenState ScreenState
    {
        get { return ScreenState.PlayerSelection; }
    }

    protected override void OpenScreen()
    {
        var model = MainModel.Instance;
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
                //if (model.ActivePlayer != pLocal)
                //{
                model.ActivePlayer.ShouldShowSelectionBox = false;
                model.ActivePlayer.PlayerState = PlayerState.Hurt;
                model.ActivePlayer = pLocal;
                model.ActivePlayer.ShouldShowSelectionBox = true;
                model.ActivePlayer.PlayerState = PlayerState.Happy;
                //}

                _forwardButton.SetActive(true);
            };
        }

        // Move players to good position
        PositionPlayers(model.AvailablePlayers);


        // Auto select
        var autoSelect = false;

        if (autoSelect)
        {
            if (MainModel.Instance.ActivePlayer != null)
            {
                _forwardButton.SetActive(true);
            }

            if (model.ActivePlayer == null)
            {
                model.ActivePlayer = model.AvailablePlayers[0];
            }

            model.ActivePlayer.ShouldShowSelectionBox = true;
        }
        else
        {
            _forwardButton.SetActive(false);
        }
    }

    protected override void CloseScreen()
    {
        var model = MainModel.Instance;

        foreach (var p in model.AvailablePlayers)
        {
            if (p != model.ActivePlayer)
            {
                p.TargetX -= 10f;
            }
        }

        _forwardButton.SetActive(false);
    }

    protected override void UpdateScreen()
    {
        var model = MainModel.Instance;

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

        if (MainModel.Instance.CameraModel.ShouldFollowActivePlayer)
        {
            MainModel.Instance.CameraModel.TargetPosition = new Vector3(MainModel.Instance.ActivePlayer.TargetX, cam.transform.position.y, cam.transform.position.z);
            camX = MainModel.Instance.ActivePlayer.TargetX;
            MainModel.Instance.CameraModel.ShouldFollowActivePlayer = false;
        }

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

            if (trans.localPosition.x > camX + screenRadius * 2)
            {
                trans.localPosition = new Vector3(max + screenRadius * 2 + (i * change), 0, 0);
            }
            else if (trans.localPosition.x < camX - screenRadius * 2)
            {
                trans.localPosition = new Vector3(min - screenRadius * 2 - (i * change), 0, 0);
            }
        }
    }

}
