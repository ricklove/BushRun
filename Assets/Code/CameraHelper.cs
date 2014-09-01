using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraHelper : MonoBehaviour
{
    public static List<Camera> Cameras = new List<Camera>();

    void Awake()
    {
        Cameras.Add(GetComponent<Camera>());
    }
}
