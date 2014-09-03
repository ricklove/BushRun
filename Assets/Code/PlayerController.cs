using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    Animator animator;
    private float updateTime = 0.05f;
    private Vector3 lastPos;
    private float lastTime;
    private float actualSpeed;
    private float reportedSpeed;
    private float timeAtUp;

    // Fly height
    private float flyHeightUnitSize = 1.75f;
    private int flyHeight = 0;
    private int maxFlyHeight = 2;
    

    // Reset child
    private GameObject childAvatar;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Use this for initialization
    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        childAvatar = transform.GetChild(0).gameObject;
        initialPosition = childAvatar.transform.localPosition;
        initialRotation = childAvatar.transform.localRotation;
    }
    
    // Update is called once per frame
    void Update()
    {
        // Move with keys
        //var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        var isUp = Input.GetAxis("Vertical") > 0;
        var isDown = Input.GetAxis("Vertical") < 0;

        // Auto move
        var moveDirection = new Vector3(1, 0, 0);

        var move = moveDirection * Time.deltaTime * speed;



        RefreshSpeed();
        
        // Animation movement z is forward
        if (Mathf.Abs(reportedSpeed - actualSpeed) > 0.05f)
        {
            // Round speed
            //actualSpeed = Mathf.CeilToInt(actualSpeed - 1 / 5) * 5; 
            
            animator.SetFloat("MovementZ", actualSpeed);
            reportedSpeed = actualSpeed;
        }

        var useFlyHeight = true;

        if (useFlyHeight)
        {
            // Fly height
            if (isUp)
            {
                flyHeight++;
            } else if (isDown)
            {
                flyHeight--;
            }

            if (flyHeight < 0)
            {
                flyHeight = 0;
            } else if (flyHeight > maxFlyHeight)
            {
                flyHeight = maxFlyHeight;
            }

            // Move to height
            var targetHeight = flyHeight * flyHeightUnitSize;
            var actualHeight = transform.localPosition.y;
            var heightDiff = targetHeight - actualHeight;

            var timeToMove = 0.25f;
            var maxMove = flyHeightUnitSize * Time.deltaTime / timeToMove;

            var heightToUse = Mathf.Min(maxMove, Mathf.Abs(heightDiff));
            heightToUse = heightDiff >= 0 ? heightToUse : -heightToUse;

            //heightToUse = heightDiff;

            var targetPos = new Vector3(transform.localPosition.x, heightToUse, transform.localPosition.z);
            var diff = targetPos - transform.localPosition;
            move += diff;
            

            if (transform.localPosition.y > 0.1f)
            {
                animator.SetBool("Fly", true);
            } else
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Fly", false);
            }
        } else
        {
            // Animate

            var timeUp = 0f;

            if (isUp)
            {
                if (timeAtUp < 0)
                {
                    timeAtUp = Time.time;
                } else
                {
                    timeUp = Time.time - timeAtUp;
                }
            
            } else
            {
                timeAtUp = -1;
            }

            // Always fly
            if (timeUp > 0)
        //if (timeUp > 0.25f)
            {
                animator.SetBool("Fly", true);
            } else if (timeUp > 0)
            {
                animator.SetBool("Jump", true);
            } else
            {
                animator.SetBool("Jump", false);
            }

            if (isDown)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Fly", false);
            }
        }


        if (move != new Vector3())
        {
            // Move the controller
            //controller.Move(move);

            transform.localPosition += move;
        }

        // Move player back to initial points (animation is slowly moving it off)
        childAvatar.transform.localRotation = initialRotation;
        childAvatar.transform.localPosition = initialPosition;
    }

    void RefreshSpeed()
    {
        var timeDiff = Time.time - lastTime;
        
        if (timeDiff > updateTime)
        {
            actualSpeed = (transform.localPosition.x - lastPos.x) / timeDiff;
            actualSpeed /= transform.localScale.x;
            
            lastTime = Time.time;
            lastPos = transform.localPosition;
        }
    }
}
