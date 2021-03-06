﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GuiViewModel : IGuiViewModel
{
    private MainModel _model;

    public GuiViewModel(MainModel model)
    {
        _model = model;
    }

    public bool ShouldShowProgressBars { get { return _model.ScreenState == ScreenState.Game; } }
    public float Health { get { return _model.ActivePlayer.Health; } }
    public float LevelProgress { get { return _model.ActiveLevelProgress; } }

    public IList<GuiItem> GuiItems
    {
        get
        {
            // TODO: Update this only when contents change

            var items = new List<GuiItem>();

            // Choices
            if (_model.ChoicesModel.Choices.Any())
            {
                var problemTop = Screen.height * 0.15f;
                var problemHeight = Screen.height * 0.8f;
                var problemRight = Screen.width * 0.95f;
                var problemWidth = Screen.width * 0.3f;
                var problemLeft = problemRight - problemWidth;

                // Question
                var questionTop = problemTop;
                var questionHeight = problemHeight * 0.2f;

                var question = _model.ChoicesModel.Question;
                if (string.IsNullOrEmpty(question))
                {
                    questionHeight = 0;
                }
                else
                {
                    items.Add(new GuiItem()
                    {
                        Text = question,
                        StyleType = GuiStyleType.Question,
                        Left = problemLeft,
                        Top = questionTop,
                        Width = problemWidth,
                        Height = questionHeight * 0.8f,
                    });
                }

                // Choices
                var choices = _model.ChoicesModel.Choices;
                var choicesTop = questionTop + questionHeight;
                var choicesHeight = problemHeight - questionHeight;

                var choiceWidth = problemWidth;
                var choicesLeft = problemLeft;

                var selectedHeightBoost = 0.5f;
                var totalChoiceHeightLength = choices.Count + selectedHeightBoost;
                var choiceHeightDistance = choicesHeight / totalChoiceHeightLength;

                var selectedChoiceHeightDistance = choiceHeightDistance * (1.0f + selectedHeightBoost);

                var normalHeight = choiceHeightDistance * 0.8f;
                var selectedHeight = selectedChoiceHeightDistance * 0.8f;

                //  Nearness
                var nearnessRatio = _model.ChoicesModel.NearnessRatio;

                var playerViewportPosition = _model.CameraModel.GameObject.GetComponent<Camera>().WorldToViewportPoint(_model.ActivePlayer.GameObject.transform.position);
                var playerRight = (playerViewportPosition.x * Screen.width) + Screen.width * 0.05f;
                var playerY = (1 - playerViewportPosition.y) * Screen.height;

                var wholeDistance = choicesLeft - playerRight;
                var distance = wholeDistance * (1 - nearnessRatio);
                var nearnessYOffset = wholeDistance - distance;

                var i = 0;
                var selectedChoiceIndex = _model.ChoicesModel.ActiveChoiceIndex;

                foreach (var choice in _model.ChoicesModel.Choices)
                {
                    var left = choicesLeft;
                    var top = choicesTop + choiceHeightDistance * i;
                    var width = choiceWidth;
                    var height = normalHeight;

                    left -= nearnessYOffset;

                    if (i == selectedChoiceIndex)
                    {
                        height = selectedHeight;
                    }
                    else if (i > selectedChoiceIndex)
                    {
                        // Add extra if past selected choice
                        top += selectedChoiceHeightDistance - choiceHeightDistance;
                    }

                    var isActive = choice == _model.ChoicesModel.ActiveChoice;
                    var item = new GuiItem()
                    {
                        Text = choice.Text,
                        StyleType = isActive ? GuiStyleType.Choice_Highlight : GuiStyleType.Choice,
                        Left = choicesLeft,
                        Top = top,
                        Width = width,
                        Height = height,
                        ClickCallback = choice.ChoiceCallback
                    };

                    items.Add(item);
                    i++;
                }
            }

            // Health Bar
            // Progress Bar
            // Coins
            // Stars

            return items;
        }
    }



}
