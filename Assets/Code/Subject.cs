using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Subject : MonoBehaviour
{
    public static Subject Instance;

    void Awake()
    {
        Instance = this;
    }

    public int _nextIndex = 0;
    public Entry[] _entries;

    void Start()
    {
        var subjectFile = Resources.Load("Misspellings") as TextAsset;
        var subject = subjectFile.text;

        var lines = subject.Split(new char[]{'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        _entries = lines.Select(l => {

            var mainParts = l.Split('\t');
            var word = mainParts [0];
            var misspellings = mainParts [1].Split(' ');
            var entry = new Entry(){ Word=word, Misspellings=misspellings };

            return entry;
        }).ToArray();

        _nextIndex = 0;
        GoNext();
    }

    public void GoStart()
    {
        _nextIndex = 0;
        GoNext();
    }

    public void GoNext()
    {
        var choices = new List<Choice>();
        
        var entry = _entries [_nextIndex];
        
        Action correctCallback = () => {
            GoNext();
        };
        
        choices.Add(new Choice(){ Text= entry.Word, IsCorrect = true, ChoiceCallback=correctCallback});
        
        foreach (var w in entry.Misspellings)
        {
            choices.Add(new Choice(){ Text= w, IsCorrect = false, ChoiceCallback=null});
        }
        
        // Randomize order
        var r = choices.ToList();
        choices.Clear();
        
        while (r.Any())
        {
            // Range is maximally exclusive
            var i = UnityEngine.Random.Range(0, r.Count - 1 + 1);
            choices.Add(r [i]);
            r.RemoveAt(i);
        }
        
        ChoiceGUI.Instance.Choices = choices.ToArray();

        HealthBarController.Instance.SetProgress(1.0f * _nextIndex / _entries.Length);

        _nextIndex++;
        
        if (_nextIndex >= _entries.Length)
        {
            _nextIndex = 0;
        }
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
