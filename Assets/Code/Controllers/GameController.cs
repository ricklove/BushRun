﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Assets.Plugins.SmartLevelsMap.Scripts;
using System;

partial class GameController : MonoBehaviour
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
            GotoLevelStart(model);
            //GotoNextProblem(model);
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
            if (model.ChoicesModel.Choices.Count > 0
                && model.ChoicesModel.ActiveChoiceIndex.HasValue)
            {
                var pathCount = model.ChoicesModel.Choices.Count;
                var pathIndex = model.ChoicesModel.ActiveChoiceIndex.Value;

                if (pathIndex < 0) { pathIndex = 0; }
                if (pathIndex >= pathCount) { pathIndex = pathCount - 1; }

                var pathUnitSize = pathCount > 1 ? 1.0f / (pathCount - 1) : 1.0f;
                var targetHeight = 1.0f - pathIndex * pathUnitSize;

                model.ActivePlayer.HeightRatio = targetHeight;
            }


        }
    }

    void RespondToLevelComplete()
    {
        var model = MainModel.Instance;
        model.ScreenState = ScreenState.LevelSelection;
        //LevelMapController.Instance.SetLevelStars
        var mapLevel = model.ActiveLevel + 1;
        var pPref = new PlayerPrefsMapProgressManager();
        var oldStars = pPref.LoadLevelStarsCount(mapLevel);
        var stars = Mathf.CeilToInt(model.ActivePlayer.Health * 3.0f);

        if (stars > oldStars)
        {
            pPref.SaveLevelStarsCount(mapLevel, stars);
        }

        //model.ActiveLevel++;

        //if (model.ActiveLevel >= LEVELCOUNT)
        //{
        //    model.ActiveLevel = 0;
        //}

        //GotoNextProblem();
    }

    void RespondToAnswerImmediate(bool isCorrect)
    {
        var model = MainModel.Instance;

        // If correct, disable other answers

        if (!isCorrect)
        {
            model.ActivePlayer.PlayerState = PlayerState.Hurt;
            SoundPlayer.Instance.PlayExplosion();
            EffectsPlayer.Instance.ShowExplosion(model.ActivePlayer.GameObject.transform.position);
        }
        else
        {
            model.ActivePlayer.PlayerState = PlayerState.Happy;
            SoundPlayer.Instance.PlayCheer();
        }
    }

    void RespondToAnswerDelayed(bool isCorrect)
    {
        var model = MainModel.Instance;
        Delay(() =>
        {
            if (model.ActivePlayer.PlayerState != PlayerState.Dead)
            {
                model.ActivePlayer.PlayerState = PlayerState.Idle;
            }
        }, 2f);


        if (isCorrect)
        {
            GotoNextProblem(model);
        }
        else
        {
            var damage = 0.35f;

            model.ActivePlayer.Health -= damage;
            // TODO: Display Health Bar

        }

        if (model.ActivePlayer.Health <= 0)
        {
            model.ActivePlayer.PlayerState = PlayerState.Dead;

            Action doContinue = () =>
            {
                GotoThisProblem(model);

                model.ActivePlayer.PlayerState = PlayerState.Happy;
                model.ActivePlayer.Health = 1;
            };

            Action doStartOver = () =>
            {
                GotoLevelStart(model);

                model.ActivePlayer.PlayerState = PlayerState.Happy;
                model.ActivePlayer.Health = 1;
            };

            Action doMainMenu = () =>
            {
                doStartOver();

                model.ActivePlayer.PlayerState = PlayerState.Happy;
                model.ActivePlayer.Health = 1;

                model.ScreenState = ScreenState.LevelSelection;
            };

            model.ChoicesModel.Choices.Clear();
            model.ChoicesModel.Choices.AddRange(new Choice[]{
                //new Choice(){ Text="CONTINUE", IsCorrect=true, ChoiceCallback=doContinue },
                    new Choice(){ Text="TRY AGAIN", IsCorrect=true, ChoiceCallback=doStartOver },
                    new Choice(){ Text="MAIN MENU", IsCorrect=true, ChoiceCallback=doMainMenu },
                });

            model.ChoicesModel.NearnessRatio = 0;

        }
    }

    void DisableScreen()
    {
        var model = MainModel.Instance;
        model.ChoicesModel.Choices.Clear();
    }
}
