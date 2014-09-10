using System.Collections.Generic;
using System.Linq;

public class Misspellings
{
    public static IList<string> CreateMisspellingsList(string word)
    {
        return CreateMisspellingsList(word, null);
    }

    public static IList<string> CreateMisspellingsList(string word, HashSet<string> realWords)
    {
        return CreateMisspellings(word, realWords).ToList();
    }

    public static IEnumerable<string> CreateMisspellings(string word)
    {
        return CreateMisspellings(word, null);
    }

    public static IEnumerable<string> CreateMisspellings(string word, HashSet<string> realWords)
    {
        // Create misspellings using mappings
        var potentialReplacements = Mappings.Where(m => word.Contains(m.Key)).SelectMany(m => m.Value).ToList();

        var oReplacements = potentialReplacements.OrderBy(m => m.Priority).ToList();

        foreach (var r in oReplacements)
        {
            var misspellings = ReplaceAny(word, r.Find, r.Replace);

            foreach (var m in misspellings)
            {
                if (realWords == null || !realWords.Contains(m))
                {
                    yield return m;
                }
            }
        }
    }

    public static IList<string> ReplaceAny(string word, string find, string replace)
    {
        var replacements = new List<string>();

        var i = word.IndexOf(find, 0);

        while (i >= 0)
        {
            var r = (i > 0 ? word.Substring(0, i) : "")
                + replace
                + (i + find.Length < word.Length ? word.Substring(i + find.Length) : "");

            replacements.Add(r);
            i = word.IndexOf(find, i + 1);
        }

        return replacements;
    }

    private class Replacement
    {
        public string Find { get; set; }
        public string Replace { get; set; }
        public int Priority { get; set; }
    }

    private static Dictionary<string, List<Replacement>> _mappings;
    private static Dictionary<string, List<Replacement>> Mappings
    {
        get
        {
            if (_mappings == null)
            {
                _mappings = new Dictionary<string, List<Replacement>>();

                var lines = mappingsString.Split(new char[] { '\r', '\n' })
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0 && !l.StartsWith("//")).ToList();

                var i = 0;

                lines.ForEach(l =>
                {
                    var items = l.Split(' ').Select(c => c.Trim()).Where(c => c.Length > 0).ToList();

                    var iItem = 0;

                    items.ForEach(f =>
                    {
                        if (!_mappings.ContainsKey(f))
                        {
                            _mappings.Add(f, new List<Replacement>());
                        }

                        var mappings = _mappings[f];

                        items.ForEach(r =>
                        {
                            if (r == f)
                            {
                                return;
                            }

                            mappings.Add(new Replacement() { Find = f, Replace = r, Priority = i + iItem * 1000 });
                            i++;
                        });

                        iItem++;
                    });
                });
            }

            return _mappings;
        }
    }

    private static string mappingsString = @"
// Doubles
aa a
bb b
cc c
dd d
ee e
ff f
gg g
hh h
ii i
jj j
kk k
ll l
mm m
nn n
oo o
pp p
qq q
rr r
ss s
tt t
uu u
vv v
ww w
xx x
yy y
zz z

// Common Mistakes
io ia
ie ei
ea a
ou uo o
eo ea
oe o
ei i

le el
ar er or
c s ss sc
wr r
r rr
l ll
t tt
nm m
mp m
ex es
ru u
ve f
dg g

tn ten tin
tion sion

z s

a e i
a u
e o

// Lost N
an a
en e
in i
on o
un u

// Vowels
a e i o u aa ae ai ao au ea ee ei eo eu ia ie ii io iu oa oe oi oo ou ua ue ui uo uu


";
}
