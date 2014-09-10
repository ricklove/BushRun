using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ChoicesModel
{
    public List<Choice> Choices { get; private set; }
    public int? ActiveChoiceIndex { get; set; }
    public Choice ActiveChoice { get { return ActiveChoiceIndex.HasValue && ActiveChoiceIndex >= 0 && ActiveChoiceIndex < Choices.Count ? Choices[ActiveChoiceIndex.Value] : null; } }
    public float NearnessRatio { get; set; }

    public ChoicesModel()
    {
        Choices = new List<Choice>();
        RestoreScreenDefaults();
    }

    public void RestoreScreenDefaults()
    {
        Choices.Clear();
        NearnessRatio = 0;
        ActiveChoiceIndex = null;
    }
}

public class Choice
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public Action ChoiceCallback { get; set; }
}
