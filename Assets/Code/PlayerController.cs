﻿using UnityEngine;
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

    // Fly height
    public float flyHeightUnitSize = 2f;
    public int maxFlyHeight = 2;
    private int flyHeight = 0;

    // Input
    private InputState upState;
    private InputState downState;
    

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

        upState = new InputState();
        downState = new InputState();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Input
        upState.ChangeState(Input.GetAxis("Vertical") > 0.01);
        downState.ChangeState(Input.GetAxis("Vertical") < -0.01);

        // Auto move
        var moveDirection = new Vector3(1, 0, 0);

        // Move with keys
        //var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);

        var move = moveDirection * Time.deltaTime * speed;




        // Fly height
        if (upState.State == InputPressState.Begin)
        {
            flyHeight++;
        }
        else if (downState.State == InputPressState.Begin)
        {
            flyHeight--;
        }

        if (flyHeight < 0)
        {
            flyHeight = 0;
        }
        else if (flyHeight > maxFlyHeight)
        {
            flyHeight = maxFlyHeight;
        }

        // Move to height
        var targetHeight = flyHeight * flyHeightUnitSize;
        var actualHeight = transform.localPosition.y;
        var heightDiff = targetHeight - actualHeight;

        var timeToMove = 0.25f;
        var maxMove = flyHeightUnitSize * Time.deltaTime / timeToMove;

        var heightDiffToUse = Mathf.Min(maxMove, Mathf.Abs(heightDiff));
        heightDiffToUse = heightDiff >= 0 ? heightDiffToUse : -heightDiffToUse;

        move += new Vector3(0, heightDiffToUse, 0);
            
        if (move != new Vector3())
        {
            transform.localPosition += move;
        }

        // Animate
        RefreshSpeed();
        
        // Animation movement z is forward
        if (Mathf.Abs(reportedSpeed - actualSpeed) > 0.05f)
        {
            // Round speed
            //actualSpeed = Mathf.CeilToInt(actualSpeed - 1 / 5) * 5; 
            
            animator.SetFloat("MovementZ", actualSpeed);
            reportedSpeed = actualSpeed;
        }

        if (transform.localPosition.y > 0.1f)
        {
            animator.SetBool("Fly", true);
        }
        else
        {
            animator.SetBool("Fly", false);
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

public class InputState
{
    public float? StartTime
    {
        get;
        set;
    }

    public float? EndTime
    {
        get;
        set;
    }

    public InputPressState State
    {
        get;
        set;
    }
              
    public float TimeSpan
    {
        get
        {
            if (!StartTime.HasValue)
            {
                return -1;
            }

            if (EndTime.HasValue)
            {
                return EndTime.Value - StartTime.Value;
            }
            else
            {
                return Time.time - StartTime.Value;
            }
        }
    }

    public void ChangeState(bool isPressed)
    {
        if (isPressed)
        {
            switch (State)
            {
                case InputPressState.Begin:
                case InputPressState.Continue:
                    State = InputPressState.Continue;
                    break;
                case InputPressState.None:
                case InputPressState.End:
                default:
                    State = InputPressState.Begin;
                    StartTime = Time.time;
                    EndTime = null;
                    break;
            }

        }
        else
        {
            switch (State)
            {
                case InputPressState.Begin:
                case InputPressState.Continue:
                    State = InputPressState.End;
                    EndTime = Time.time;
                    break;
                case InputPressState.End:
                case InputPressState.None:
                default:
                    State = InputPressState.None;
                    StartTime = null;
                    EndTime = null;
                    break;
            }
        }
    }
}

public enum InputPressState
{
    None,
    Begin,
    Continue,
    End
}
