using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Subject : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var subjectFile = Resources.Load("Misspellings") as TextAsset;
        var subject = subjectFile.text;

        var lines = subject.Split(new char[]{'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        var words = lines.Select(l => {

            var mainParts = l.Split('\t');
            var word = mainParts [0];
            var misspellings = mainParts [1].Split(' ');
            var entry = new { Word=word, Misspellings=misspellings };

            return entry;
        }).ToList();



        var choiceGUI = GetComponent<ChoiceGUI>();
        //choiceGUI.Choices 

        var index = 0;

        Action doGoNext = null;

        doGoNext = () => {
            var choices = new List<Choice>();

            var entry = words [index];

            Action correctCallback = () => {
                // This will have value when it is called
                doGoNext();
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

            choiceGUI.Choices = choices.ToArray();
            index++;

            if (index >= words.Count)
            {
                index = 0;
            }
        };

        doGoNext();
    }

    
}
