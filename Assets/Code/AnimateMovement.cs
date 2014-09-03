using UnityEngine;
using System.Collections;

public class AnimateMovement : MonoBehaviour
{
    public int gestureCount = 2;

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

        animator.SetInteger("GestureID", 0);
        animator.SetBool("Fly", false);
        animator.SetBool("Dive", false);
        animator.SetBool("Jump", false);

        if (Mathf.FloorToInt(Time.time) % 30 == 0)
        {
            animator.SetInteger("GestureID", Random.Range(1, gestureCount + 1));
            
        } else if (Mathf.FloorToInt(Time.time) % 20 == 0)
        {
            animator.SetBool("Fly", true);
            
        } else if (Mathf.FloorToInt(Time.time) % 12 == 0)
        {
            animator.SetBool("Dive", true);

        } else if (Mathf.FloorToInt(Time.time) % 4 == 0)
        {
            animator.SetBool("Jump", true);
        } 
    }
}
