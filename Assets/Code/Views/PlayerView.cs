using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerView : MonoBehaviour
{
    public float maxHeight = 4f;
    public IPlayerViewModel PlayerViewModel;

    private PlayerHeadView _headView;

    // Animations
    private Animator _animator;

    // Reset Avatar Position
    private GameObject _childAvatar;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private GameObject _selectionBox;

    private PlayerState _lastState;

    void Start()
    {
        _headView = new PlayerHeadView();
        _headView.Initialize(this);

        _animator = GetComponentInChildren<Animator>();

        _childAvatar = transform.GetChild(0).gameObject;
        _initialPosition = _childAvatar.transform.localPosition;
        _initialRotation = _childAvatar.transform.localRotation;

        _selectionBox = transform.FindChild("SelectionBox").gameObject;
        _selectionBox.GetComponent<Clickable>().MouseDownCallback = () =>
        {
            if (PlayerViewModel != null
                && PlayerViewModel.SelectCallback != null)
            {
                PlayerViewModel.SelectCallback();
            }
        };
    }

    void Update()
    {
        if (PlayerViewModel == null)
        {
            return;
        }

        UpdateSelectionBox();

        ResetAvatarPosition();

        // Move To Target
        MoveToTarget(PlayerViewModel.TargetX, PlayerViewModel.HeightRatio);

        // Animate
        RefreshSpeed();

        // Flip for backwards
        if (_actualSpeed < -0.1f)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.z, transform.localScale.z);
        }
        else if (_actualSpeed > 0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.z, transform.localScale.z);
        }

        // Animate movement
        if (Mathf.Abs(_reportedSpeed - _actualSpeed) > 0.05f)
        {
            // Animation movement z is forward
            _animator.SetFloat("MovementZ", Mathf.Abs(_actualSpeed));
            _reportedSpeed = _actualSpeed;
        }

        // Animate flying
        if (transform.localPosition.y > 0.1f)
        {
            _animator.SetBool("Fly", true);
        }
        else
        {
            _animator.SetBool("Fly", false);
        }

        // Animate emotions
        _animator.SetBool("Hurt", false);
        _animator.SetBool("Cheer", false);

        if (PlayerViewModel.PlayerState != _lastState)
        {
            _lastState = PlayerViewModel.PlayerState;

            if (PlayerViewModel.PlayerState == PlayerState.Happy)
            {
                _animator.SetBool("Cheer", true);
            }
            else if (PlayerViewModel.PlayerState == PlayerState.Hurt)
            {
                _animator.SetBool("Hurt", true);
            }
        }

        _headView.Update(PlayerViewModel);
    }


    private void UpdateSelectionBox()
    {
        _selectionBox.GetComponent<SpriteRenderer>().enabled = PlayerViewModel.ShouldShowSelectionBox;
    }

    private void ResetAvatarPosition()
    {
        // Move player back to initial points (animation is slowly moving it off)
        _childAvatar.transform.localRotation = _initialRotation;
        _childAvatar.transform.localPosition = _initialPosition;
    }


    private Vector3 _lastPos;
    private float _lastTime;
    private float _actualSpeed;
    private float _reportedSpeed;

    void RefreshSpeed()
    {
        const float updateTime = 0.05f;

        var timeDiff = Time.time - _lastTime;

        if (timeDiff > updateTime)
        {
            _actualSpeed = (transform.localPosition.x - _lastPos.x) / timeDiff;
            _actualSpeed /= Math.Abs(transform.localScale.x);

            _lastTime = Time.time;
            _lastPos = transform.localPosition;
        }
    }

    void MoveToTarget(float targetX, float heightRatio)
    {
        var speed = PlayerViewModel.SpeedRatio * PlayerViewModel.MaxSpeed;
        var maxDiff = Time.deltaTime * speed;

        // Move to height
        var targetHeight = PlayerViewModel.HeightRatio * maxHeight;
        var heightDiff = targetHeight - transform.localPosition.y;

        var heightDiffToUse = Mathf.Min(maxDiff, Mathf.Abs(heightDiff));
        heightDiffToUse = heightDiff >= 0 ? heightDiffToUse : -heightDiffToUse;

        // Move to x
        var xDiff = targetX - transform.localPosition.x;

        var xDiffToUse = Mathf.Min(maxDiff, Mathf.Abs(xDiff));
        xDiffToUse = xDiff >= 0 ? xDiffToUse : -xDiffToUse;

        var move = new Vector3(xDiffToUse, heightDiffToUse, 0);

        if (move != new Vector3())
        {
            transform.localPosition += move;
        }
    }
}

public interface IPlayerViewModel
{
    PlayerData PlayerData { get; }
    PlayerState PlayerState { get; }

    float HeightRatio { get; }
    float TargetX { get; }

    bool ShouldShowSelectionBox { get; }
    Action SelectCallback { get; }

    float SpeedRatio { get; }
    float MaxSpeed { get; }
}

public enum PlayerState
{
    Idle,
    Happy,
    Hurt
}
