﻿using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public float speed = 5f;
    Animator animator;
    private float updateTime = 0.05f;
    private Vector3 lastPos;
    private float lastTime;
    private float actualSpeed;
    private float reportedSpeed;

    // Fly Paths
    public float maxHeight = 4f;
    public int pathCount = 3;
    public int pathIndex = 0;

    // Input
    private InputState upState;
    private InputState downState;

    // Response
    private ResponseState responseState;

    // Health
    private float health = 1f;
    public float damage = 0.1f; 
    

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

        if (health > 0)
        {
            move *= health;
        }
        else
        {
            move = new Vector3();
        }

        // Fly height
        if (upState.State == InputPressState.Begin)
        {
            pathIndex++;
        }
        else if (downState.State == InputPressState.Begin)
        {
            pathIndex--;
        }

        if (pathIndex < 0)
        {
            pathIndex = 0;
        }
        else if (pathIndex >= pathCount)
        {
            pathIndex = pathCount - 1;
        }

        // Move to height
        var pathUnitSize = pathCount > 1 ? maxHeight / (pathCount - 1) : maxHeight;

        var targetHeight = pathIndex * pathUnitSize;
        var actualHeight = transform.localPosition.y;
        var heightDiff = targetHeight - actualHeight;

        var timeToMove = 0.25f;
        var maxMove = pathUnitSize * Time.deltaTime / timeToMove;

        var heightDiffToUse = Mathf.Min(maxMove, Mathf.Abs(heightDiff));
        heightDiffToUse = heightDiff >= 0 ? heightDiffToUse : -heightDiffToUse;

        move += new Vector3(0, heightDiffToUse, 0);
            
        if (move != new Vector3())
        {
            transform.localPosition += move;
        }

        // Animate
        var headController = GetComponent<HeadSelection>();
        

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


        // Respond to answer
        animator.SetBool("Hurt", false);
        animator.SetBool("Cheer", false);

        if (responseState == ResponseState.Incorrect)
        {
            animator.SetBool("Hurt", true);
            headController.ChangeHead(HeadType.Hurt);
            SoundController.Instance.PlayExplosion();
            ParticleController.Instance.ShowExplosion(transform.position);

            health -= damage;
            if (health <= 0)
            {
                SoundController.Instance.PlayHurt();
                animator.SetBool("Dead", true);
                var oldChoices = ChoiceGUI.Instance.Choices;
                var oldTimeToAnswer = ChoiceGUI.Instance.timeToAnswer;

                Action doContinue = () => {
                    ChoiceGUI.Instance.timeToAnswer = oldTimeToAnswer;
                    ChoiceGUI.Instance.Choices = oldChoices;

                    animator.SetBool("Dead", false);
                    health = 1;
                    HealthBarController.Instance.SetHealth(health);
                };

                Action doStartOver = () => {
                    ChoiceGUI.Instance.timeToAnswer = oldTimeToAnswer;
                    SubjectController.Instance.GoStartOfLevel();
                    
                    animator.SetBool("Dead", false);
                    health = 1;
                    HealthBarController.Instance.SetHealth(health);
                };

                Action doMainMenu = () => {
                    doStartOver();

                    MenuController.Instance.ReturnFromGame();
                };

                ChoiceGUI.Instance.Choices = new Choice[]{
                //new Choice(){ Text="CONTINUE", IsCorrect=true, ChoiceCallback=doContinue },
                    new Choice(){ Text="TRY AGAIN", IsCorrect=true, ChoiceCallback=doStartOver },
                    new Choice(){ Text="MAIN MENU", IsCorrect=true, ChoiceCallback=doMainMenu },
                };

                ChoiceGUI.Instance.timeToAnswer = float.MaxValue;
            }
    
            HealthBarController.Instance.SetHealth(health);
        }
        else if (responseState == ResponseState.Correct)
        {
            animator.SetBool("Cheer", true);
            headController.ChangeHead(HeadType.Happy);
            SoundController.Instance.PlayCheer();
        }

                

        responseState = ResponseState.None;
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

    public void RespondToAnswer(bool isCorrect)
    {
        responseState = isCorrect ? ResponseState.Correct : ResponseState.Incorrect;
    }

    public void FinishLevel()
    {
        LevelMapController.Instance.SetLevelStars(SubjectController.Instance._level, (int)(health * 3));
        MenuController.Instance.ReturnFromGame();
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

public enum ResponseState
{
    None,
    Correct,
    Incorrect
}
