using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ActionHelpers
{
    public static Coroutine StartCoroutineWithDelay(this MonoBehaviour self, Action doAction, float delayTime)
    {
        return self.StartCoroutine(Delay2(doAction, delayTime));
    }

    public static IEnumerator Delay2(this Action doAction, float delayTime)
    {
        yield return new UnityEngine.WaitForSeconds(delayTime);
        doAction();
        yield return 0;
    }

    public static Action OnlyOnce(this Action doAction)
    {
        var hasDone = false;

        return () =>
        {
            if (!hasDone)
            {
                hasDone = true;
                doAction();
            }
        };
    }

}
