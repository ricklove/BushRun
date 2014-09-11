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

                var groups = CreateWordDiffGroups(word, misspellings);
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

    public static IList<WordDiffGroup> CreateWordDiffGroups(string word, string misspellings)
    {
        return CreateWordDiffGroups(word, misspellings.Split(' '));
    }

    public static IList<WordDiffGroup> CreateWordDiffGroups(string word, string[] misspellings)
    {
        var diffs = misspellings.Select(m => CreateWordDiff(word, m)).ToList();
        //diffs = diffs.Where(d => d.FromText.Length == d.ToText.Length).ToList();
        //var groups = diffs.GroupBy(d => d.Index * 2 + d.FromText.Length * 31 + d.ToText.Length * 501);
        var groups = diffs.GroupBy(d => d.WordWithSingleBlank);

        return groups.Select(g => new WordDiffGroup() { Word = word, Diffs = g.ToList() }).ToList();
    }

    public static WordDiff CreateWordDiff(string original, string change)
    {
        var shouldIncludeOneCharacter = true;
        var shouldAllowBreakDoubles = false;

        if (original.Length <= 0 || change.Length <= 0)
        {
            return new WordDiff(original, change, 0, 0, 0);
        }

        if (original == change)
        {
            return new WordDiff(original, change, 0, 0, 0);
        }

        var indexOfFirstChange = 0;

        for (int i = 0; i < original.Length; i++)
        {
            if (i >= change.Length)
            {
                indexOfFirstChange = i;
                break;
            }

            if (original[i] != change[i])
            {
                indexOfFirstChange = i;
                break;
            }

            indexOfFirstChange = i;

            if (i == original.Length - 1)
            {
                indexOfFirstChange++;
            }
        }

        var indexOfLastChange = original.Length - 1;
        var indexOfLastChange_Change = original.Length - 1;
        for (int i = 0; i < original.Length; i++)
        {
            var iRev = original.Length - 1 - i;
            var iRevChange = change.Length - 1 - i;

            if (iRevChange < 0)
            {
                indexOfLastChange = iRev;
                indexOfLastChange_Change = iRevChange;
                break;
            }

            if (original[iRev] != change[iRevChange])
            {
                indexOfLastChange = iRev;
                indexOfLastChange_Change = iRevChange;
                break;
            }

            indexOfLastChange = iRev;
            indexOfLastChange_Change = iRevChange;

            if (i == original.Length - 1)
            {
                indexOfLastChange--;
                indexOfLastChange_Change--;
            }
        }

        var changeLen = indexOfLastChange - indexOfFirstChange + 1;
        var changeLen_Change = indexOfLastChange_Change - indexOfFirstChange + 1;

        while (changeLen < 0 && changeLen_Change >= 0)
        {
            if (indexOfFirstChange > 0)
            {
                indexOfFirstChange--;
                changeLen++;
                changeLen_Change++;
            }
            else
            {
                changeLen++;
                changeLen_Change++;
            }
        }

        var diff = new WordDiff(original, change, indexOfFirstChange, changeLen, changeLen_Change);

        while (!shouldAllowBreakDoubles && diff.HasBreaksBetweenDoubles)
        {
            if (diff.HasBreaksBetweenDoubles_Before)
            {
                indexOfFirstChange--;
                changeLen++;
                changeLen_Change++;
            }
            else
            {
                changeLen++;
                changeLen_Change++;
            }

            diff = new WordDiff(original, change, indexOfFirstChange, changeLen, changeLen_Change);
        }

        while (shouldIncludeOneCharacter
            && (changeLen <= 0 || changeLen_Change <= 0))
        {
            if (indexOfFirstChange > 0)
            {
                indexOfFirstChange--;
                changeLen++;
                changeLen_Change++;
            }
            else
            {
                changeLen++;
                changeLen_Change++;
            }

            diff = new WordDiff(original, change, indexOfFirstChange, changeLen, changeLen_Change);
        }

        return diff;
    }

    public class WordDiffGroup
    {
        public string Word { get; set; }
        public List<WordDiff> Diffs { get; set; }

        public override string ToString()
        {
            return Word + "::" + Diffs.First().WordWithBlanks + "->"
                + Diffs.Aggregate(new System.Text.StringBuilder(), (b, t) => b.Append(t.ToText + ";")).ToString() + "::"
                + Diffs.Aggregate(new System.Text.StringBuilder(), (b, t) => b.Append(t.ToString() + ";")).ToString();
        }
    }

    public class WordDiff
    {
        public string Word { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public string FromText { get; set; }
        public string ToText { get; set; }

        public WordDiff(string word, string toWord, int index, int countFromText, int countToText)
        {
            index = index < 0 ? 0 : index;
            countFromText = countFromText < 0 ? 0 : countFromText;
            countToText = countToText < 0 ? 0 : countToText;

            //countFromText = index + countFromText > word.Length ? word.Length - index : countFromText;
            //countToText = index + countToText > toWord.Length ? toWord.Length - index : countToText;


            Word = word;
            Index = index;
            Count = countFromText;
            FromText = word.Substring(index, countFromText);
            ToText = toWord.Substring(index, countToText);
        }

        public string BeforeText { get { return Count > 0 ? Word.Substring(0, Index) : Word; } }
        public string AfterText { get { return Count > 0 ? Word.Substring(Index + Count) : ""; } }
        public bool HasBreaksBetweenDoubles
        {
            get
            {
                return HasBreaksBetweenDoubles_Before
                    || HasBreaksBetweenDoubles_After;
            }
        }

        public bool HasBreaksBetweenDoubles_Before
        {
            get
            {
                return BeforeText != ""
                    && (BeforeText.LastOrDefault() == FromText.FirstOrDefault()
                    || BeforeText.LastOrDefault() == ToText.FirstOrDefault());
            }
        }

        public bool HasBreaksBetweenDoubles_After
        {
            get
            {
                return AfterText != ""
                    && (AfterText.FirstOrDefault() == FromText.LastOrDefault()
                    || AfterText.FirstOrDefault() == ToText.LastOrDefault());
            }
        }

        public string WordWithBlanks
        {
            get
            {
                if (Count > 0)
                {
                    return BeforeText + CreateBlanks(Count) + AfterText;
                }
                else
                {
                    return Word;
                }

            }
        }

        public string WordWithSingleBlank
        {
            get
            {
                if (Count > 0)
                {
                    return BeforeText + "•" + AfterText;
                }
                else
                {
                    return Word;
                }

            }
        }

        private string CreateBlanks(int Count)
        {
            var blanks = "";

            for (int i = 0; i < Count; i++)
            {
                blanks += "•";
            }

            return blanks;
        }

        public override string ToString()
        {
            return WordWithBlanks + "('" + FromText + "'->'" + ToText + "')";
        }
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