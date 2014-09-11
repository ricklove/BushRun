using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class SubjectController : MonoBehaviour
{
    public static SubjectController Instance;

    void Awake()
    {
        Instance = this;
    }

    public int levels = 25;
    public int _level = 0;
    public int _nextIndex = 0;
    public SubjectEntries _subject;
    private Entry[] _entries;

    void Start()
    {
        _subject = new SubjectEntries();
        _subject.LoadSubject();

        //GoStart();
    }

    public void ChangeLevel(int level)
    {
        _level = level;

        if (_level > levels)
        {
            _level = 0;
        }

        var entriesPerLevel = Mathf.CeilToInt(_subject._entries.Length / levels);
        _entries = _subject._entries.Skip(level * entriesPerLevel).Take(entriesPerLevel).ToArray();
        _nextIndex = 0;
        //GoNext();
    }

    public void GoStart()
    {
        ChangeLevel(0);
        _nextIndex = 0;
        GoNext();
    }

    public void GoStartOfLevel()
    {
        _nextIndex = 0;
        GoNext();
    }

    public void GoNext()
    {
        var choices = new List<Choice>();

        var entry = _entries[_nextIndex];

        Action correctCallback = () =>
        {
            GoNext();
        };

        choices.Add(new Choice() { Text = entry.Word, IsCorrect = true, ChoiceCallback = correctCallback });

        foreach (var w in entry.Misspellings)
        {
            choices.Add(new Choice() { Text = w, IsCorrect = false, ChoiceCallback = null });
        }

        // Randomize order
        var r = choices.ToList();
        choices.Clear();

        while (r.Any())
        {
            // Range is maximally exclusive
            var i = UnityEngine.Random.Range(0, r.Count - 1 + 1);
            choices.Add(r[i]);
            r.RemoveAt(i);
        }

        ChoiceGUI.Instance.Choices = choices.ToArray();
        HealthBarController.Instance.SetProgress(1.0f * _nextIndex / _entries.Length);

        _nextIndex++;

        if (_nextIndex >= _entries.Length)
        {
            PlayerController.Instance.FinishLevel();
            _nextIndex = 0;
            //ChangeLevel(_level + 1);
        }
    }

}

public class SubjectEntries
{
    public Entry[] _entries;

    public void LoadSubject()
    {
        var subjectFile = Resources.Load("Misspellings") as TextAsset;
        var subject = subjectFile.text;

        var lines = subject.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        _entries = lines.Select(l =>
        {

            var mainParts = l.Split('\t');
            var word = mainParts[0];
            var misspellings = mainParts[1].Split(' ');
            var entry = new Entry(word, misspellings);

            return entry;
        }).ToArray();
    }
}

//public class Entry
//{
//    public string Word
//    {
//        get;
//        set;
//    }

//    public string[] Misspellings
//    {
//        get;
//        set;
//    }
//}
