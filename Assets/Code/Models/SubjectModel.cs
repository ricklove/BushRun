using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class SubjectModel
{
    public static bool CREATE_FILE = false;
    public static bool GENERATE_EXTRA_MISSPELLINGS = false;

    public IList<Entry> Entries { get; private set; }

    private string _subjectName;

    public string SubjectName
    {
        get
        {
            if (_subjectName == null)
            {
                _subjectName = PlayerPrefs.GetString("SubjectName", "Subject");
            }

            return _subjectName;
        }
        set
        {
            _subjectName = value;
            PlayerPrefs.SetString("SubjectName", _subjectName);
        }
    }


    public void LoadSubject()
    {
        string name = "WordGroups01";

        var subjectFile = Resources.Load(name) as TextAsset;
        //var subjectFile = Resources.Load("Misspellings") as TextAsset;
        //var subjectFile = Resources.Load("WordGroups") as TextAsset;
        var subject = subjectFile.text;

        var lines = subject.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // Handle WordGroups
        if (lines[0].Contains(": "))
        {
            var gLines = lines.Select(l => l.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[1]).ToList();
            var gWords = gLines.SelectMany(l => l.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Select(w => w.Trim()).ToList();

            lines = gWords
            .Select(l => l.Trim().ToLowerInvariant())
            .ToArray();
        }

        Entries = lines.Select(l =>
        {
            var mainParts = l.Split('\t');
            var word = mainParts[0];
            var misspellings = mainParts.Length > 1 ? mainParts[1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) : new string[0];

            if (GENERATE_EXTRA_MISSPELLINGS & misspellings.Length < 10)
            {
                var extraMisspellings = Misspellings.CreateMisspellings(word, SpellCheckerWordList.Words).Take(10);
                misspellings = misspellings.Union(extraMisspellings).ToArray();
            }

            var entry = new Entry(word, misspellings);

            return entry;
        }).ToArray();

        if (CREATE_FILE)
        {
            var fileText = new System.Text.StringBuilder();

            Entries.ToList().ForEach(e =>
                {
                    fileText.Append(e.Word + "\t");
                    e.Misspellings.ToList().ForEach(m => fileText.Append(m + " "));
                    fileText.AppendLine();
                });

            var fileTextStr = fileText.ToString();
        }

        SubjectName = name;
    }

    public SubjectModel()
    {
    }
}

public class Entry
{
    public string Word
    {
        get;
        set;
    }

    public string[] Misspellings
    {
        get;
        set;
    }

    public Entry(string word, string misspellings)
        : this(word, misspellings.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
    {

    }

    public Entry(string word, string[] misspellings)
    {
        Word = word;
        Misspellings = misspellings.Where(m => m.Trim().Any()).ToArray();
    }
}
