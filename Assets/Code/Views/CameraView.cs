using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class CameraView : MonoBehaviour
{
    public ICameraViewModel CameraViewModel;

    private Vector3? _targetPosition;
    private float? _targetSize;
    private float? _timeAtMoveFinish;

    private Vector3 _initialPosition;
    private float _initialSize;

    void Start()
    {
        _initialPosition = transform.position;
        _initialSize = GetComponent<Camera>().orthographicSize;
    }

    void Update()
    {
        if (CameraViewModel == null)
        {
            return;
        }

        Vector3? targetPos = null;
        float? targetSize = null;

        if (CameraViewModel.TargetPosition.HasValue)
        {
            targetPos = CameraViewModel.TargetPosition.Value;
        }

        if (CameraViewModel.TargetSize.HasValue)
        {
            targetSize = CameraViewModel.TargetSize.Value;
        }

        if (CameraViewModel.ShouldFollowActivePlayer
            && CameraViewModel.ActivePlayerX.HasValue)
        {
            targetPos = new Vector3(CameraViewModel.ActivePlayerX.Value, _initialPosition.y, _initialPosition.z);
            targetSize = _initialSize;
        }

        if ((targetPos.HasValue && targetPos != _targetPosition)
            || (targetSize.HasValue && targetSize != _targetSize))
        {
            _targetPosition = targetPos;
            _targetSize = targetSize;

            _timeAtMoveFinish = Time.time + CameraViewModel.TimeToMove ?? 0;

            if (Time.time > _timeAtMoveFinish - 0.05f)
            {
                transform.position = _targetPosition.Value;
                GetComponent<Camera>().orthographicSize = _targetSize.Value;
            }
            else
            {
                StartCoroutine(MoveToTarget(_timeAtMoveFinish));
            }
        }
    }

    IEnumerator MoveToTarget(float? timeID)
    {
        while (Time.time < _timeAtMoveFinish)
        {
            var timeLeft = _timeAtMoveFinish.Value - Time.time;

            var diff = _targetPosition.Value - transform.position;
            var diffInDelta = diff * Time.deltaTime / timeLeft;

            transform.position += diffInDelta;

            var diffSize = _targetSize.Value - GetComponent<Camera>().orthographicSize;
            var diffSizeInDelta = diffSize * Time.deltaTime / timeLeft;
            GetComponent<Camera>().orthographicSize += diffSizeInDelta;

            yield return 0;

            if (timeID != _timeAtMoveFinish)
            {
                yield break;
            }
        }

        transform.position = _targetPosition.Value;
        GetComponent<Camera>().orthographicSize = _targetSize.Value;
    }
}

public interface ICameraViewModel
{
    bool ShouldFollowActivePlayer { get; }
    float? ActivePlayerX { get; }
    float? TimeToMove { get; }

    float? TargetSize { get; }
    Vector3? TargetPosition { get; }

}
