﻿using UnityEngine;
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
                var model = MainModel.Instance;
                model.ActiveLevel = e.Number - 1;
                model.ScreenState = ScreenState.Game;
            }
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
        _levelMap.GetComponent<LevelsMap>().Start();


        // Allow go back
        //model.ActivePlayer.ShouldShowSelectionBox = true;
        model.ActivePlayer.SelectCallback = () =>
        {
            model.ActivePlayer.TargetX = signPosition - signPositionFromCamera;
            model.CameraModel.TargetSize = null;
            model.CameraModel.ShouldFollowActivePlayer = true;
            model.ActivePlayer.SelectCallback = null;

            this.StartCoroutineWithDelay(() =>
            {
                model.ScreenState = ScreenState.PlayerSelection;
            }, 1f);
        };
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
