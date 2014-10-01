using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;

partial class GameController
{
    const int LEVELCOUNT = 25;
    private ProblemMode ProblemMode = ProblemMode.WholeWord;

    private int _loadedLevel = -1;

    private Entry[] _entries;
    private List<Problem> _subProblems;

    private ProblemIndex _nextProblemIndex = new ProblemIndex();

    public class Problem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public IList<string> WrongAnswers { get; set; }

        public Problem()
        {
            Question = "";
            Answer = "";
            WrongAnswers = new List<string>();
        }

        public override string ToString()
        {
            return Question + "? "
                + Answer + " ("
                + WrongAnswers.Aggregate("", (b, s) => b += s + ";")
                + ")";
        }
    }

    public class ProblemIndex
    {
        public int Index { get; private set; }
        public int SubIndex { get; private set; }

        public ProblemIndex() : this(0, 0) { }
        public ProblemIndex(int index, int subIndex)
        {
            Index = index;
            SubIndex = subIndex;
        }

    }

    public void GotoLevelStart(MainModel model)
    {
        _nextProblemIndex = new ProblemIndex();
        GotoNextProblem(model);
    }

    public void GotoThisProblem(MainModel model)
    {
        if (_nextProblemIndex.SubIndex > 0)
        {
            _nextProblemIndex = new ProblemIndex(_nextProblemIndex.Index, 0);
        }
        else if (_nextProblemIndex.Index <= 0)
        {
            _nextProblemIndex = new ProblemIndex();
        }
        else
        {
            _nextProblemIndex = new ProblemIndex(_nextProblemIndex.Index - 1, 0);
        }

        GotoNextProblem(model);
    }

    public void GotoNextProblem(MainModel model)
    {
        if (_entries != null
            && _entries.Length > 0
            && _nextProblemIndex.Index >= _entries.Length)
        {
            RespondToLevelComplete();
            return;
        }

        Problem problem = GetProblem(model);

        // Create choices
        var choices = new List<Choice>();

        choices.Add(new Choice() { Text = problem.Answer, IsCorrect = true, ChoiceCallback = null });
        foreach (var w in problem.WrongAnswers)
        {
            choices.Add(new Choice() { Text = w, IsCorrect = false, ChoiceCallback = null });
        }

        SetupChoices(model, problem.Question, choices);

        if (_nextProblemIndex.SubIndex < _subProblems.Count - 1)
        {
            _nextProblemIndex = new ProblemIndex(_nextProblemIndex.Index, _nextProblemIndex.SubIndex + 1);
        }
        else
        {
            _nextProblemIndex = new ProblemIndex(_nextProblemIndex.Index + 1, 0);
        }
    }

    private Problem GetProblem(MainModel model)
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
            _nextProblemIndex = new ProblemIndex();
            _loadedLevel = model.ActiveLevel;
        }

        if (_nextProblemIndex.SubIndex == 0)
        {
            var entry = _entries[_nextProblemIndex.Index];
            _subProblems = CreateSubProblems(entry, ProblemMode);
            _subProblems.ForEach(p => LimitWrongAnswers(p));
        }

        return _subProblems[_nextProblemIndex.SubIndex];
    }

    private static void LimitWrongAnswers(Problem problem)
    {
        // Ensure no correct wrong answers
        problem.WrongAnswers = problem.WrongAnswers.Where(w => w != problem.Answer).ToList();

        // Take only n wrong choices
        var wrongChoices = 3;
        var firstChoices = wrongChoices / 2;
        var afterChoices = wrongChoices - firstChoices;

        problem.WrongAnswers = problem.WrongAnswers.Take(firstChoices).Union(problem.WrongAnswers.Skip(firstChoices).RandomizeOrder().Take(afterChoices)).ToList();
    }

    public static List<Problem> CreateSubProblems(Entry entry, ProblemMode problemMode)
    {
        List<Problem> problems;

        if (problemMode == ProblemMode.PartialWord)
        {
            problems = CreateSubProblems_PartialWord(entry);
        }
        else if (problemMode == ProblemMode.EachLetter)
        {
            problems = CreateSubProblems_WholeWord(entry).Union(
                CreateSubProblems_EachLetter(entry)).ToList();
        }
        else // Whole Word
        {
            problems = CreateSubProblems_WholeWord(entry);
        }

        return problems;
    }

    public static List<Problem> CreateSubProblems_WholeWord(Entry entry)
    {
        var problem = new Problem();
        problem.Answer = entry.Word;
        problem.WrongAnswers = entry.Misspellings.ToList();

        var problems =
        new List<Problem>() { problem };
        return problems;
    }

    public static List<Problem> CreateSubProblems_PartialWord(Entry entry)
    {
        var problem = new Problem();

        // Create Partial Answers
        var word = entry.Word;
        var misspellings = entry.Misspellings;

        var groups = WordDiffHelper.CreateWordDiffGroups(word, misspellings);
        var rGroup = groups.Skip(UnityEngine.Random.Range(0, groups.Count())).First();

        problem.Question = rGroup.Diffs.First().WordWithSingleBlank;
        problem.Answer = rGroup.Diffs.First().FromText;
        rGroup.Diffs.ForEach(d => problem.WrongAnswers.Add(d.ToText));

        var problems = new List<Problem>() { problem }; ;
        return problems;
    }

    public static List<Problem> CreateSubProblems_EachLetter(Entry entry)
    {
        var word = entry.Word;
        var misspellings = entry.Misspellings;

        var shouldOnlyUseMisspellings = false;
        // Don't use extra misspellings at end (to avoid bad words)
        var shouldOnlyUseMisspellingsAtEnd = true;

        var diffLetters = word.Select((c, i) =>
        {
            List<char> wrongLetters = new List<char>();

            // Include last letter and next letter
            if (i > 0)
            {
                wrongLetters.Add(word[i - 1]);
            }

            if (i < word.Length - 1)
            {
                wrongLetters.Add(word[i + 1]);
            }

            // Add letters from misspellings
            wrongLetters.AddRange(
                misspellings.Where(m => m.Length > i && m[i] != c).Select(m => m[i]));

            if (!shouldOnlyUseMisspellings)
            {
                if (shouldOnlyUseMisspellingsAtEnd && i == word.Length - 1)
                {
                    // Don't use extra misspellings at end
                }
                else
                {
                    wrongLetters.AddRange(
                        Misspellings.CreateMisspellings(c.ToString()).SelectMany(m => m));
                }
            }

            wrongLetters = wrongLetters.Distinct().ToList();

            return new
            {
                Letter = c,
                Index = i,
                WrongLetters = wrongLetters
            };
        }).ToList();

        var skipLetters = diffLetters.Where(d => d.WrongLetters.Any()).ToList();
        var skipLettersComplete = skipLetters.Select((d, iSkip) =>
        {
            var index = iSkip == 0 ? 0 : d.Index;
            var count = iSkip < skipLetters.Count - 1 ? skipLetters[iSkip + 1].Index - index : word.Length - index;
            return new
            {
                Index = index,
                Count = count,
                Before = word.Substring(0, index),
                After = word.Substring(index + count),
                CorrectPart = word.Substring(index, count),
                WrongParts = d.WrongLetters.Select(l => l.ToString() + word.Substring(index + 1, count - 1)).ToList()
            };
        }).ToList();

        return skipLettersComplete.Select(d => new Problem()
        {
            Question = d.Before + WordDiff.CreateBlanks(d.Count) + "…",//+ (d.After.Any() ? "…" : ""),
            Answer = d.CorrectPart,
            WrongAnswers = d.WrongParts.Select(l => l).ToList(),
        }).ToList();

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
    PartialWord,
    EachLetter
}