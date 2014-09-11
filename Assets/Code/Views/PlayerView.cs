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
    private GameObject _childAvatarDirection;
    private GameObject _childAvatar;
    private GameObject _headHolder;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private GameObject _selectionBox;

    private PlayerState _lastState;

    private GameObject _flyEffects;

    void Start()
    {
        _headView = new PlayerHeadView();

        _selectionBox = transform.FindChild("SelectionBox").gameObject;
        _selectionBox.GetComponent<Clickable>().MouseDownCallback = () =>
        {
            if (PlayerViewModel != null
                && PlayerViewModel.SelectCallback != null)
            {
                PlayerViewModel.SelectCallback();
            }
        };

        _flyEffects = transform.FindChild("FlyEffects").gameObject;
    }

    private AvatarType _lastAvatarType = (AvatarType)(-1);
    private void ActivateAvatar()
    {
        if (_lastAvatarType == PlayerViewModel.PlayerData.AvatarType)
        {
            return;
        }

        _lastAvatarType = PlayerViewModel.PlayerData.AvatarType;

        _childAvatarDirection = transform.GetChild(0).GetChild(0).gameObject;
        var rotationAndScale = _childAvatarDirection.transform.GetChild(0).gameObject;

        var avatarName = Enum.GetName(typeof(AvatarType), PlayerViewModel.PlayerData.AvatarType);

        _childAvatar = null;

        for (int i = 0; i < rotationAndScale.transform.childCount; i++)
        {
            var aChild = rotationAndScale.transform.GetChild(i);

            if (aChild.name == avatarName)
            {
                _childAvatar = aChild.gameObject;
                aChild.gameObject.SetActive(true);
            }
            else
            {
                aChild.gameObject.SetActive(false);
            }
        }

        if (_childAvatar == null)
        {
            _childAvatar = rotationAndScale.transform.GetChild(0).gameObject;
            _childAvatar.SetActive(true);
        }

        _headHolder = _childAvatar.transform.GetComponentInChildren<SpriteRenderer>().transform.parent.gameObject;
        _headView.Initialize(_headHolder);

        _animator = _childAvatar.transform.GetComponentInChildren<Animator>();

        _initialPosition = _childAvatar.transform.localPosition;
        _initialRotation = _childAvatar.transform.localRotation;
    }

    void Update()
    {
        if (PlayerViewModel == null)
        {
            return;
        }

        ActivateAvatar();


        UpdateSelectionBox();

        ResetAvatarPosition();

        // Move To Target
        MoveToTarget(PlayerViewModel.TargetX, PlayerViewModel.HeightRatio);

        // Animate
        RefreshSpeed();

        // Flip for backwards
        if (_actualSpeed < -0.001f)
        {
            _childAvatarDirection.transform.localRotation = Quaternion.Euler(0, 120, 0);
            _headHolder.transform.localRotation = Quaternion.Euler(0, 300, 0);
        }
        else if (_actualSpeed > 0.001f)
        {
            _childAvatarDirection.transform.localRotation = Quaternion.Euler(0, 0, 0);
            _headHolder.transform.localRotation = Quaternion.Euler(0, 240, 0);
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
            _flyEffects.SetActive(true);
        }
        else
        {
            _animator.SetBool("Fly", false);
            _flyEffects.SetActive(false);
        }

        // Animate emotions
        _animator.SetBool("Hurt", false);
        _animator.SetBool("Cheer", false);

        if (PlayerViewModel.PlayerState != _lastState)
        {
            _lastState = PlayerViewModel.PlayerState;
            _animator.SetBool("Dead", false);

            if (PlayerViewModel.PlayerState == PlayerState.Happy)
            {
                _animator.SetBool("Cheer", true);
            }
            else if (PlayerViewModel.PlayerState == PlayerState.Hurt)
            {
                _animator.SetBool("Hurt", true);
            }
            else if (PlayerViewModel.PlayerState == PlayerState.Dead)
            {
                _animator.SetBool("Dead", true);
                SoundPlayer.Instance.PlayHurt();
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
        //_childAvatar.transform.localRotation = _initialRotation;
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
    Hurt,
    Dead
}

