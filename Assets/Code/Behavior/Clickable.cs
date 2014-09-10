using UnityEngine;
using System.Collections;

public class Clickable : MonoBehaviour
{
    public System.Action MouseDownCallback { get; set; }

    void OnMouseDown()
    {
        if (MouseDownCallback != null)
        {
            MouseDownCallback();
        }
    }
}
