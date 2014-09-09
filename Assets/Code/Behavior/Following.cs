using UnityEngine;
using System.Collections;

public class Following : MonoBehaviour
{
    public GameObject Target = null;
    public bool TargetActivePlayer = false;
    public bool TargetCamera = false;

    public Vector3 Scale = new Vector3(1, 1, 1);
    private Vector3 _lastScrollerPosition;
    private Vector3 _lastSelfPosition;

    private bool _hasStarted = false;

    void Start()
    {

    }
    
    void LateUpdate()
    {
        if (TargetActivePlayer)
        {
            Target = MainModel.Instance.ActivePlayer.GameObject;
        }

        if (TargetCamera)
        {
            Target = MainModel.Instance.CameraModel.GameObject;
        }

        if (Target != null)
        {
            // Use change since last
            if (_hasStarted)
            {
                var selfChange = gameObject.transform.localPosition - _lastSelfPosition;
                var scrollerChange = Target.transform.localPosition - _lastScrollerPosition;

                var scaledChange = Vector3.Scale(scrollerChange, Scale);

                var diff = scaledChange - selfChange;

                gameObject.transform.localPosition += diff;
            }

            _lastSelfPosition = gameObject.transform.localPosition;
            _lastScrollerPosition = Target.transform.localPosition;
            _hasStarted = true;
        }
    }
}
