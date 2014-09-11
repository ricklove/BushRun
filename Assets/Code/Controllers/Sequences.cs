using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Sequences
{
    public static void MournActivePlayerDead(MainModel model, ScreenControllerBase self)
    {
        var i = 0;

        foreach (var p in model.AvailablePlayers)
        {
            if (p != model.ActivePlayer)
            {
                p.PlayerState = PlayerState.Happy;

                p.TargetX = model.ActivePlayer.TargetX - 2f - 0.75f * i;

                if (p.GameObject.transform.position.x < p.TargetX - 10f)
                {
                    p.GameObject.transform.position = new Vector3(p.TargetX - 10f, p.GameObject.transform.position.y, p.GameObject.transform.position.z);
                }

                i++;
            }
        }

        self.StartCoroutineWithDelay(() =>
        {
            if (model.ActivePlayer.PlayerState != PlayerState.Dead)
            {
                model.ActivePlayer.PlayerState = PlayerState.Dead;
            }
        }, 2f);

        self.StartCoroutineWithDelay(() =>
        {
            foreach (var p in model.AvailablePlayers)
            {
                if (p != model.ActivePlayer)
                {
                    p.PlayerState = PlayerState.Hurt;
                }
            }
        }, 3f);
    }

    public static void CelebrateActivePlayerDead(MainModel model, ScreenControllerBase self)
    {
        var i = 0;

        foreach (var p in model.AvailablePlayers)
        {
            if (p != model.ActivePlayer)
            {
                p.PlayerState = PlayerState.Hurt;

                p.TargetX = model.ActivePlayer.TargetX - 2f - 0.75f * i;

                if (p.GameObject.transform.position.x < p.TargetX - 10f)
                {
                    p.GameObject.transform.position = new Vector3(p.TargetX - 10f, p.GameObject.transform.position.y, p.GameObject.transform.position.z);
                }

                i++;
            }
        }

        self.StartCoroutineWithDelay(() =>
        {
            if (model.ActivePlayer.PlayerState != PlayerState.Dead)
            {
                model.ActivePlayer.PlayerState = PlayerState.Dead;
            }
        }, 3f);

        self.StartCoroutineWithDelay(() =>
        {
            foreach (var p in model.AvailablePlayers)
            {
                if (p != model.ActivePlayer)
                {
                    p.PlayerState = PlayerState.Happy;
                }
            }
        }, 4f);
    }
}

