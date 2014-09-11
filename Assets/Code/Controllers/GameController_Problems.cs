using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

partial class GameController
{
    const int LEVELCOUNT = 25;
    private ProblemMode ProblemMode = ProblemMode.PartialWord;

    private int _loadedLevel = -1;
    private int _nextProblemIndex = 0;

    private Entry[] _entries;

    public void GotoLevelStart(MainModel model)
    {
        _nextProblemIndex = 0;
        GotoNextProblem(model);
    }

    public void GotoThisProblem(MainModel model)
    {
        _nextProblemIndex--;
        if (_nextProblemIndex < 0) { _nextProblemIndex = 0; }
        GotoNextProblem(model);
    }

    public void GotoNextProblem(MainModel model)
    {

        if (model.SubjectModel.Entries == null)
        {
            model.SubjectModel.LoadSubject();
        }

        if (_entries == null
            || _loadedLevel != model.ActiveLevel)
        {
            var allEntries = model.SubjectModel.Entries;
            var entriesPerLevel = Mathf.CeilToInt(allEntries.Count / LEVELCOUNT);
            _entries = model.SubjectModel.Entries.Skip(model.ActiveLevel * entriesPerLevel).Take(entriesPerLevel).ToArray();
            _nextProblemIndex = 0;
            _loadedLevel = model.ActiveLevel;
        }

        if (_nextProblemIndex >= _entries.Length)
        {
            RespondToLevelComplete();
        }
        else
        {
            var choices = new List<Choice>();
            var entry = _entries[_nextProblemIndex];

            // Get correct word
            // Get misspellings
            // Get question
            var question = "";
            var correctAnswer = "";
            var wrongAnswers = new List<string>();


            if (ProblemMode == ProblemMode.PartialWord)
            {
                // Create Partial Answers
                var word = entry.Word;
                var misspellings = entry.Misspellings;

                var groups = WordDiffHelper.CreateWordDiffGroups(word, misspellings);
                var rGroup = groups.Skip(UnityEngine.Random.Range(0, groups.Count())).First();

                question = rGroup.Diffs.First().WordWithSingleBlank;
                correctAnswer = rGroup.Diffs.First().FromText;
                rGroup.Diffs.ForEach(d => wrongAnswers.Add(d.ToText));
            }
            else
            {
                correctAnswer = entry.Word;
                wrongAnswers = entry.Misspellings.ToList();
            }

            // Take only n wrong choices
            var wrongChoices = 3;
            var firstChoices = wrongChoices / 2;
            var afterChoices = wrongChoices - firstChoices;

            wrongAnswers = wrongAnswers.Take(firstChoices).Union(wrongAnswers.Skip(firstChoices).RandomizeOrder().Take(afterChoices)).ToList();

            // Create choices
            choices.Add(new Choice() { Text = correctAnswer, IsCorrect = true, ChoiceCallback = null });
            foreach (var w in wrongAnswers)
            {
                choices.Add(new Choice() { Text = w, IsCorrect = false, ChoiceCallback = null });
            }

            SetupChoices(model, question, choices);
        }

        _nextProblemIndex++;
    }

    private void SetupChoices(MainModel model, string question, IList<Choice> choices)
    {
        // Setup callback
        foreach (var c in choices)
        {
            var choice = c;
            var isCorrect = choice.IsCorrect;

            Action choiceCallback = () =>
            {
                // Goto choice
                model.ChoicesModel.ActiveChoiceIndex = model.ChoicesModel.Choices.IndexOf(choice);
                RespondToAnswerImmediate(isCorrect);

                // Delay non-game response
                this.StartCoroutineWithDelay(() =>
                {
                    // Remove choice
                    if (model.ChoicesModel.Choices.Contains(choice))
                    {
                        model.ChoicesModel.Choices.Remove(choice);
                        model.ChoicesModel.ActiveChoiceIndex = null;

                        //if (model.ChoicesModel.ActiveChoiceIndex >= model.ChoicesModel.Choices.Count)
                        //{
                        //    model.ChoicesModel.ActiveChoiceIndex = model.ChoicesModel.Choices.Count - 1;
                        //}

                        // Do Callback
                        RespondToAnswerDelayed(isCorrect);
                    }

                }, 1f);

            };

            c.ChoiceCallback = choiceCallback.OnlyOnce();

        }

        var randomOrder = choices.RandomizeOrder();

        model.ChoicesModel.Choices.Clear();
        model.ChoicesModel.Choices.AddRange(randomOrder);
        //model.ChoicesModel.ShouldShowChoices = true;
        model.ChoicesModel.ActiveChoiceIndex = null;

        model.ChoicesModel.Question = question;
    }




    public void DisplayTestChoices(MainModel model)
    {
        var testChoices = new Choice[]{
                    new Choice(){Text= "Wrong", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Wrong With a long text answer", IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= @"Wrong With a really really really really really really 

really really really really really really really really really really really 

really really really really really really really really really really really really really really really really really really really really really really really really long text answer"
                        , IsCorrect=false, ChoiceCallback= null},
                    new Choice(){Text= "Right", IsCorrect=true, ChoiceCallback= null},
                    new Choice(){Text= "Wrong Too", IsCorrect=false, ChoiceCallback= null},
                };

        SetupChoices(model, "TEST QUESTION", testChoices);
    }

}


public enum ProblemMode
{
    WholeWord,
    PartialWord
}