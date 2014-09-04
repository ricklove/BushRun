using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class ChoiceGUI : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;
    public float timeToAnswer = 5f;
    public Choice[] _choices = null;
    private float timeStart = -1;
    public GUIStyle choiceStyle = GUI.skin.box;
    public GUIStyle selectedChoiceStyle = GUI.skin.box;

    public Choice[] Choices
    {
        get
        { 
            if (_choices == null)
            {
                Action doReset = () => 
                {
                    Choices = null;
                };

                _choices = new Choice[]{
                    new Choice(){Text= "Wrong", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Wrong With a long text answer", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= @"Wrong With a really really really really really really 

really really really really really really really really really really really 

really really really really really really really really really really really really really really really really really really really really really really really really long text answer"
                        , IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Right", IsCorrect=true, ChoiceCallback= doReset},
                    new Choice(){Text= "Wrong Too", IsCorrect=false, ChoiceCallback= null},
                };
            }

            return _choices;
        }
        set
        {
            _choices = value;
            timeStart = Time.time;
        }
    }

    void OnGUI()
    {
        var choices = Choices;

        var pController = player.GetComponent<PlayerController>();
        pController.pathCount = choices.Length;

        var choicesTop = Screen.height * 0.15f;
        var choicesHeight = Screen.height * 0.8f;

        var choicesRight = Screen.width * 0.95f;
        var choiceWidth = Screen.width * 0.3f;
        var choicesLeft = choicesRight - choiceWidth;

        // Nearness
        if (timeStart < 0)
        {
            timeStart = Time.time;
        }

        var nearnessRatio = (Time.time - timeStart) / timeToAnswer;

        var p = mainCamera.WorldToViewportPoint(player.transform.position);
        var playerRight = (p.x * Screen.width) + Screen.width * 0.05f;
        var playerY = (1 - p.y) * Screen.height;
        
        var wholeDistance = choicesLeft - playerRight;
        var distance = wholeDistance * (1 - nearnessRatio);
        var nearnessYOffset = wholeDistance - distance;

        var selectedChoiceIndex = choices.Length - 1 - pController.pathIndex;

        // Draw choices
        for (int i = 0; i < choices.Length; i++)
        {
            var rightHeightBoost = 0.5f;
            var totalChoiceHeightLength = choices.Length + rightHeightBoost;
            var choiceHeightDistance = choicesHeight / totalChoiceHeightLength;

            var wrongChoiceHeightDistance = choiceHeightDistance;
            var rightChoiceHeightDistance = choiceHeightDistance * (1.0f + rightHeightBoost);

            var wrongHeight = wrongChoiceHeightDistance * 0.8f;
            var rightHeight = rightChoiceHeightDistance * 0.8f;

            var choice = choices [i];
            var left = choicesLeft;
            var top = choicesTop + wrongChoiceHeightDistance * i;
            var width = choiceWidth;
            var height = wrongHeight;  

            left -= nearnessYOffset;

            if (i == selectedChoiceIndex)
            {
                height = rightHeight;
            }
            else if (i > selectedChoiceIndex)
            {
                top += rightChoiceHeightDistance - wrongChoiceHeightDistance;
            }

            var fontSize = (int)(height * 0.6f);
            
            var style = choiceStyle;
            
            if (i == selectedChoiceIndex)
            {
                style = selectedChoiceStyle;
            }

            style.fontSize = fontSize;


            // Reduce font size if needed
            var content = new GUIContent(choice.Text);
            float mHeight = style.CalcHeight(content, width);

            while (mHeight > height * 0.8f)
            {
                var diffRatio = mHeight / (height * 0.8f);
                var reduce = 1 / diffRatio;
                var halfReduce = (1 + reduce) / 2;
                var ratio = Math.Min(halfReduce, 0.8f);

                style.fontSize = (int)(style.fontSize * ratio);
                mHeight = style.CalcHeight(content, width);
            } 

            if (GUI.Button(new Rect(left, top, width, height), choice.Text, style))
            {
                pController.pathIndex = choices.Length - 1 - i;
            }
            
        }

        // Trigger choice
        if (nearnessRatio >= 1)
        {
            var choice = Choices [selectedChoiceIndex];

            if (choice.IsCorrect)
            {
                pController.RespondToAnswer(true);
            }
            else
            {
                pController.RespondToAnswer(false);

                // Remove that choice
                var ch = Choices.ToList();
                ch.RemoveAt(selectedChoiceIndex);
                Choices = ch.ToArray();
            }

            if (choice.ChoiceCallback != null)
            {
                choice.ChoiceCallback();
            }
        }
    }
}

public class Choice
{
    public string Text
    {
        get;
        set;
    }

    public bool IsCorrect
    {
        get;
        set;
    }

    public Action ChoiceCallback{ get; set; }
}
