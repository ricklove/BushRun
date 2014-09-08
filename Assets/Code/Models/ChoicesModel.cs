using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChoicesModel
{
    public bool ShouldShowChoices { get; set; }
    public List<Choice> Choices { get; private set; }
    public int ActiveChoiceIndex { get; set; }
    public Choice ActiveChoice { get { return ActiveChoiceIndex >= 0 && ActiveChoiceIndex < Choices.Count ? Choices[ActiveChoiceIndex] : null; } }
    public float NearnessRatio { get; set; }

    public ChoicesModel()
    {
        ShouldShowChoices = false;
        Choices = new List<Choice>();
    }

}

public class Choice
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public Action ChoiceCallback { get; set; }
}
