using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class ScreenControllerBase : MonoBehaviour
{
    protected bool _isOpen = false;

    void Update()
    {
        var model = MainModel.Instance;

        if (model.ScreenState != ScreenState)
        {
            // Close only after coroutines are done
            if (_isOpen && !_coroutines.Any())
            {
                _isOpen = false;
                CloseScreen();
            }

            return;
        }

        // Open only after coroutines are done
        if (!_isOpen && !_coroutines.Any())
        {
            _isOpen = true;

            // Reset player and camera
            foreach (var p in model.AvailablePlayers)
            {
                p.RestoreScreenDefaults();
            }

            model.CameraModel.RestoreScreenDefaults();
            model.ChoicesModel.RestoreScreenDefaults();

            OpenScreen();
        }

        if (_isOpen)
        {
            UpdateScreen();
        }
    }

    private static List<Action> _coroutines = new List<Action>();

    protected void StartCoroutineWithDelay(Action doAction, float timeToWait)
    {
        _coroutines.Add(doAction);
        
        Action wrapper = () => {
            // Remove when done
            _coroutines.Remove(doAction);
            doAction();
        }; 

        StartCoroutine(ActionHelpers.Delay(wrapper, timeToWait));
    }

    protected abstract ScreenState ScreenState { get; }
    protected abstract void OpenScreen();
    protected abstract void CloseScreen();
    protected abstract void UpdateScreen();
}

