using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellCheckerWordList
{
    private static HashSet<string> _words;

    public static HashSet<string> Words
    {
        get
        {
            if (_words == null)
            {
                _words = new HashSet<string>();
                var text = Resources.Load<TextAsset>("SpellCheckerWordList_US");

                var iStart = text.text.IndexOf("---");
                var words = text.text.Substring(iStart + 3).Split('\n').Select(w=>w.Trim());

                foreach (var w in words)
                {
                    _words.Add(w);
                }
            }

            return _words;
        }
    }
}
