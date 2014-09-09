using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Assets.Plugins.SmartLevelsMap.Scripts;

class LevelSelectionController : MonoBehaviour
{
    private bool _isSetup = false;
    private float _timeToMove = 0;

    private GameObject _sign;
    private GameObject _signCamera;
    private GameObject _levelMap;
    private GameObject _character;

    private float _initialSignX;

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
            if (_isSetup)
            {
                var model = MainModel.Instance;
                model.ActiveLevel = e.Number - 1;
                model.ScreenState = ScreenState.Game;
            }
        };
    }

    void Update()
    {
        var model = MainModel.Instance;

        if (model.ScreenState != ScreenState.LevelSelection)
        {
            if (_isSetup)
            {
                _isSetup = false;
                DisableScreen();
            }

            return;
        }

        if (!_isSetup)
        {
            _isSetup = true;

            // Move Sign offscreen
            var cam = transform.root.FindChild("MainCamera");
            var camX = cam.transform.position.x;
            var screenRadius = cam.GetComponent<Camera>().orthographicSize;

            _sign.transform.localPosition = new Vector3(camX + screenRadius * 3f, 0, 0);
            _sign.SetActive(true);

            // Move to left of sign
            model.ActivePlayer.TargetX = _sign.transform.position.x - _initialSignX;

            // Zoom into sign
            model.CameraModel.ShouldFollowActivePlayer = true;

            StartCoroutine(ZoomIntoSign());

            // Select character
            _character.GetComponent<SpriteRenderer>().sprite = model.ActivePlayer.PlayerData.Sprites.First(s => s.SpriteType == SpriteType.HeadIdle).Sprite;


            // Reset levels map
            _levelMap.GetComponent<LevelsMap>().Start();
        }


    }

    IEnumerator ZoomIntoSign()
    {
        var model = MainModel.Instance;

        yield return new WaitForSeconds(3f);

        model.CameraModel.ShouldFollowActivePlayer = false;
        model.CameraModel.TimeToMove = 1.0f;

        model.CameraModel.TargetSize = _signCamera.GetComponent<Camera>().orthographicSize;
        model.CameraModel.TargetPosition = _signCamera.transform.position;

    }


    void DisableScreen()
    {
        var model = MainModel.Instance;
        //_sign.SetActive(false);
    }
}
