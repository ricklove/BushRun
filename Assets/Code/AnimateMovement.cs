using UnityEngine;
using System.Collections;

public class AnimateMovement : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        lastTime = Time.time;
        lastPos = transform.localPosition;
    }
    
    // Update is called once per frame
    private float updateTime = 0.5f;
    private Vector3 lastPos;
    private float lastTime;
    private float speed;
    private float reportedSpeed;

    void UpdateSpeed()
    {
        var timeDiff = Time.time - lastTime;
        
        if (timeDiff > updateTime)
        {
            speed = (transform.localPosition.x - lastPos.x) / timeDiff;
            speed /= transform.localScale.x;
            
            lastTime = Time.time;
            lastPos = transform.localPosition;
        }
    }

    void Update()
    {
        var animator = GetComponent<Animator>();

        UpdateSpeed();

        // Animation movement z is forward
        if (Mathf.Abs(reportedSpeed - speed) > 0.5)
        {
            animator.SetFloat("MovementZ", speed);
            reportedSpeed = speed;
        }

        if (Mathf.FloorToInt(Time.time) % 15 == 0)
        {
            animator.SetBool("Dive", true);

        } else if (Mathf.FloorToInt(Time.time) % 5 == 0)
        {
            animator.SetBool("Jump", true);
        } else
        {
            animator.SetBool("Dive", false);
            animator.SetBool("Jump", false);
        }
    }
}
