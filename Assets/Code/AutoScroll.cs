using UnityEngine;
using System.Collections;

public class AutoScroll : MonoBehaviour
{
    public float speed = 2.5f;

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void FixedUpdate()
    {
        var fixedSpeed = speed * Time.fixedDeltaTime;
        gameObject.transform.localPosition += new Vector3(fixedSpeed, 0, 0);
    }
}
