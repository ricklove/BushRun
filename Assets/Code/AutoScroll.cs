using UnityEngine;
using System.Collections;

public class AutoScroll : MonoBehaviour
{
    public float speed = 0.1f;

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.localPosition += new Vector3(speed, 0, 0);
    }
}
