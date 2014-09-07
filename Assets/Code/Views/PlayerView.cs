using UnityEngine;
using System.Collections.Generic;

public class PlayerView : MonoBehaviour
{
    public float maxSpeed = 4f;
    public float maxHeight = 4f;
    public IPlayerViewModel PlayerViewModel;

    private PlayerHeadView _headView;

    // Animations
    private Animator _animator;

    // Reset Avatar Position
    private GameObject _childAvatar;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    void Start()
    {
        _headView = new PlayerHeadView();
        _headView.Initialize(this);

        _animator = GetComponentInChildren<Animator>();

        _childAvatar = transform.GetChild(0).gameObject;
        _initialPosition = _childAvatar.transform.localPosition;
        _initialRotation = _childAvatar.transform.localRotation;
    }

    void Update()
    {
        if (PlayerViewModel == null)
        {
            return;
        }

        ResetAvatarPosition();

        // Move To Target
        MoveToTarget(PlayerViewModel.TargetX, PlayerViewModel.HeightRatio);

        // Animate
        RefreshSpeed();

        // Animate movement
        if (Mathf.Abs(_reportedSpeed - _actualSpeed) > 0.05f)
        {
            // Animation movement z is forward
            _animator.SetFloat("MovementZ", _actualSpeed);
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


        _headView.Update(PlayerViewModel);
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
            _actualSpeed /= transform.localScale.x;

            _lastTime = Time.time;
            _lastPos = transform.localPosition;
        }
    }

    void MoveToTarget(float targetX, float heightRatio)
    {
        var maxDiff = Time.deltaTime * maxSpeed;

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
}

public enum PlayerState
{
    Idle,
    Happy,
    Hurt
}
