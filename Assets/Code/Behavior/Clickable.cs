using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour
{
    public float test;

    public System.Action MouseDownCallback { get; set; }

    void OnMouseDown()
    {
        if (MouseDownCallback != null)
        {
            MouseDownCallback();
        }
    }
}
