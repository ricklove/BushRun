using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Assets.Plugins.SmartLevelsMap.Scripts;

class GameController : MonoBehaviour
{
    private bool _isSetup = false;

    void Start()
    {

    }

    void Update()
    {
        var model = MainModel.Instance;

        if (model.ScreenState != ScreenState.Game)
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

            model.CameraModel.ShouldFollowActivePlayer = true;

            // Display Choices
            GameController_Choices.DisplayTestChoices(this, model);
        }

        if (_isSetup)
        {
            var speed = 10f;
            var timeToMoveCamera = 0.25f;

            var size = model.CameraModel.GameObject.GetComponent<Camera>().orthographicSize;
            model.CameraModel.TimeToMove = timeToMoveCamera;
            model.CameraModel.ActivePlayerXOffset = size * 1.5f - (speed * timeToMoveCamera);


            // Make max speed higher than actual speed to ensure the player character can keep up with the game
            model.ActivePlayer.MaxSpeed = speed * 1.5f;
            model.ActivePlayer.SpeedRatio = 1f;
            model.ActivePlayer.TargetX += Time.deltaTime * speed;



            // Change height to choice
            var pathCount = model.ChoicesModel.Choices.Count;
            var pathIndex = model.ChoicesModel.ActiveChoiceIndex;
            var pathUnitSize = pathCount > 1 ? 1.0f / (pathCount - 1) : 1.0f;
            var targetHeight = 1.0f - pathIndex * pathUnitSize;

            model.ActivePlayer.HeightRatio = targetHeight;

            // TODO: Display Subject


        }
    }

    void DisableScreen()
    {
        var model = MainModel.Instance;
    }
}
