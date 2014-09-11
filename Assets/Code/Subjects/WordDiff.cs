using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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

    public static string CreateBlanks(int Count)
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

public static class WordDiffHelper
{
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
}