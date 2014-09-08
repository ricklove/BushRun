using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

class GameController_Choices
{
    public static void SetChoices(MonoBehaviour m, MainModel model, IList<Choice> choices)
    {
        foreach (var c in choices)
        {
            var choice = c;
            var choiceCallbackInner = choice.ChoiceCallback;

            Action doRespond = () =>
            {
                model.ChoicesModel.ActiveChoiceIndex = model.ChoicesModel.Choices.IndexOf(choice);

                // Remove choice
                m.StartCoroutine(Delay(() =>
                {
                    if (model.ChoicesModel.Choices.Contains(choice))
                    {
                        model.ChoicesModel.Choices.Remove(choice);

                        if (model.ChoicesModel.ActiveChoiceIndex >= model.ChoicesModel.Choices.Count)
                        {
                            model.ChoicesModel.ActiveChoiceIndex = model.ChoicesModel.Choices.Count - 1;
                        }

                        if (choiceCallbackInner != null)
                        {
                            choiceCallbackInner();
                        }
                    }

                }, 1f));

            };

            choice.ChoiceCallback = doRespond;
        }

        model.ChoicesModel.Choices.Clear();
        model.ChoicesModel.Choices.AddRange(choices);
        model.ChoicesModel.ShouldShowChoices = true;
        model.ChoicesModel.ActiveChoiceIndex = 0;
    }

    public static IEnumerator Delay(Action doAction, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        doAction();
        yield return 0;
    }

    public static void DisplayTestChoices(MonoBehaviour m, MainModel model)
    {
        Action doReset = () =>
        {
            DisplayTestChoices(m, model);
        };

        var testChoices = new Choice[]{
                    new Choice(){Text= "Wrong", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Wrong With a long text answer", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= @"Wrong With a really really really really really really 

really really really really really really really really really really really 

really really really really really really really really really really really really really really really really really really really really really really really really long text answer"
                        , IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Right", IsCorrect=true, ChoiceCallback= doReset},
                    new Choice(){Text= "Wrong Too", IsCorrect=false, ChoiceCallback= null},
                };

        SetChoices(m, model, testChoices);
    }

}
