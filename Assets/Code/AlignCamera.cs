using UnityEngine;
using System.Collections;

public class AlignCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Align();
    }
    
    // Update is called once per frame
    void Update()
    {
        Align();
    }

    void Align()
    {
        var camera = gameObject.GetComponent<Camera>();
        gameObject.transform.localPosition = new Vector3(
            gameObject.transform.localPosition.x, 
            camera.orthographicSize - 0.5f, 
            gameObject.transform.localPosition.z);
    }

}
