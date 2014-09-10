using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Assets.Plugins.SmartLevelsMap.Scripts;

class LevelSelectionController : ScreenControllerBase
{
    private float _timeToMove = 0;

    private GameObject _sign;
    private GameObject _signCamera;
    private GameObject _levelMap;
    private GameObject _character;
    private GameObject _forwardButton;
    private GameObject _backButton;

    private float _initialSignX;

    protected override ScreenState ScreenState
    {
        get { return ScreenState.LevelSelection; }
    }

    void Start()
    {
        _sign = transform.FindChild("Sign").gameObject;
        _signCamera = _sign.transform.FindChild("SignCamera").gameObject;
        _levelMap = _sign.transform.FindChild("LevelsMap").gameObject;
        _character = _levelMap.transform.FindChild("Character").FindChild("CharacterSprite").gameObject;

        _initialSignX = _sign.transform.localPosition.x;
        _sign.SetActive(false);

        // Subscribe to level change
        LevelsMap.LevelReached += (object sender, LevelReachedEventArgs e) =>
        {
            if (_isOpen)
            {
                MainModel.Instance.ActiveLevel = e.Number - 1;
            }
        };

        _forwardButton = _sign.transform.FindChild("ButtonForward").gameObject;
        _forwardButton.GetComponent<Clickable>().MouseDownCallback = () =>
        {
            MainModel.Instance.ActiveLevel = LevelsMap.GetLevelNumber() - 1;
            MainModel.Instance.ScreenState = ScreenState.Game;
        };

        _backButton = _sign.transform.FindChild("ButtonBack").gameObject;
        _backButton.GetComponent<Clickable>().MouseDownCallback = () =>
        {
            GoBackToPlayerSelection();
        };
    }

    protected override void OpenScreen()
    {
        var model = MainModel.Instance;

        // Move Sign offscreen
        var cam = transform.root.FindChild("MainCamera");
        var camX = cam.transform.position.x;
        var screenRadius = cam.GetComponent<Camera>().orthographicSize;

        var signPositionFromCamera = screenRadius * 3f;
        var signPosition = camX + signPositionFromCamera;
        var signMax = camX + signPositionFromCamera;
        var signMin = camX - signPositionFromCamera;

        if (_sign.activeSelf
            && _sign.transform.position.x > signMin
            && _sign.transform.position.x < signMax)
        {
            // Sign is already in ok position
            // TODO: Make sure other players are out of the way
        }
        else
        {
            _sign.transform.position = new Vector3(signPosition, 0, 0);
            _sign.SetActive(true);
        }

        // Move to left of sign
        model.ActivePlayer.TargetX = _sign.transform.position.x - _initialSignX;

        // Zoom into sign
        model.CameraModel.ShouldFollowActivePlayer = true;

        StartCoroutine(ZoomIntoSign());

        // Select character
        _character.GetComponent<SpriteRenderer>().sprite = model.ActivePlayer.PlayerData.Sprites.First(s => s.SpriteType == SpriteType.HeadIdle).Sprite;


        // Reset levels map
        var levelsMap = _levelMap.GetComponent<LevelsMap>();
        levelsMap.Start();
        LevelsMap.GoToLevel(model.ActiveLevel + 1);



        // Allow go back
        //model.ActivePlayer.ShouldShowSelectionBox = true;
        //model.ActivePlayer.SelectCallback = () =>
        //{
        //    model.ActivePlayer.SelectCallback = null;

        //    GoBackToPlayerSelection(model, signPositionFromCamera);
        //};
    }

    private void GoBackToPlayerSelection()
    {
        MainModel model = MainModel.Instance;

        model.ActivePlayer.TargetX -= 10f;
        model.CameraModel.TargetSize = null;
        model.CameraModel.ShouldFollowActivePlayer = true;
        model.CameraModel.ActivePlayerXOffset = 0f;
        model.CameraModel.TimeToMove = 0.5f;

        var i = 0;

        foreach (var p in model.AvailablePlayers)
        {
            if (p != model.ActivePlayer)
            {
                p.TargetX = model.ActivePlayer.TargetX - 2f - 0.5f * i;

                if (p.GameObject.transform.position.x < p.TargetX - 10f)
                {
                    p.GameObject.transform.position = new Vector3(p.TargetX - 10f, p.GameObject.transform.position.y, p.GameObject.transform.position.z);
                }

                i++;
            }
        }

        this.StartCoroutineWithDelay(() =>
        {
            //model.ActivePlayer.TargetX = model.ActivePlayer.GameObject.transform.position.x;
        }, 3f);

        this.StartCoroutineWithDelay(() =>
        {
            model.ScreenState = ScreenState.PlayerSelection;
        }, 5f);
    }

    protected override void CloseScreen()
    {
        var model = MainModel.Instance;

        // Reset camera
        //model.CameraModel.TargetSize = null;
    }

    protected override void UpdateScreen()
    {

    }

    IEnumerator ZoomIntoSign()
    {
        var model = MainModel.Instance;

        yield return new WaitForSeconds(3f);

        if (model.ScreenState == ScreenState.LevelSelection)
        {
            model.CameraModel.ShouldFollowActivePlayer = false;
            model.CameraModel.TimeToMove = 1.0f;

            model.CameraModel.TargetSize = _signCamera.GetComponent<Camera>().orthographicSize;
            model.CameraModel.TargetPosition = _signCamera.transform.position;
        }
    }

}
