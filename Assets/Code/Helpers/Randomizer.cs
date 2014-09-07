using System;
using System.Collections.Generic;
using System.Linq;

public static class Randomizer
{
    public static List<T> RandomizeOrder<T>(this List<T> items)
    {
        // Randomize order
        var r = items.ToList();
        items = new List<T>(items.Count);

        while (r.Any())
        {
            // Range is maximally exclusive
            var i = UnityEngine.Random.Range(0, r.Count - 1 + 1);
            items.Add(r[i]);
            r.RemoveAt(i);
        }

        return items;
    }
}
