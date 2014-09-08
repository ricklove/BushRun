using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class SubjectModel
{
    public IList<Entry> Entries { get; private set; }

    public void LoadSubject()
    {
        var subjectFile = Resources.Load("Misspellings") as TextAsset;
        var subject = subjectFile.text;

        var lines = subject.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Entries = lines.Select(l =>
        {

            var mainParts = l.Split('\t');
            var word = mainParts[0];
            var misspellings = mainParts[1].Split(' ');

            // TODO: Generate extra misspellings

            var entry = new Entry() { Word = word, Misspellings = misspellings };

            return entry;
        }).ToArray();
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
}
