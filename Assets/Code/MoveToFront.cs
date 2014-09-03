using UnityEngine;
using System.Collections;
using System.Linq;

public class MoveToFront : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        // Move to front of camera
        var cam = CameraHelper.Cameras.First(c => c.gameObject.activeSelf);


        // Reset transform
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));


        // Move towards camera
        var pos = transform.TransformPoint(new Vector3());
        var camPos = cam.transform.position;
        //var posTowardsCam = new Vector3(pos.x, pos.y, (camPos.z - pos.z) * 0.7f + pos.z);

        // TODO: Use thickness of head
        var posTowardsCam = new Vector3(pos.x, pos.y, -.2f + pos.z);

        var localPos = transform.InverseTransformPoint(posTowardsCam);
        transform.localPosition = localPos;

        // Billboard
//        var posAtCam = new Vector3(pos.x, pos.y, camPos.z);
//        var posUp = transform.TransformPoint(new Vector3(0, 100000, 0));
//        transform.LookAt(cam.transform, posUp);
    }
}
