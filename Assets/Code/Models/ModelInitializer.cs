using UnityEngine;
using System.Collections;

public class ModelInitializer : MonoBehaviour
{

    void Awake()
    {
        var model = MainModel.Instance;

        // Setup camera
        var camera = transform.FindChild("MainCamera").gameObject;
        camera.GetComponent<CameraView>().CameraViewModel = new CameraViewModel(model);
        model.CameraModel.GameObject = camera;
    }

}