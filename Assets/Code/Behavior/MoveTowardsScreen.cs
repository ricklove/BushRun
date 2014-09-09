using UnityEngine;
using System.Collections;
using System.Linq;

public class MoveTowardsScreen : MonoBehaviour
{
    public float distance = 0.2f;

    private Vector3 _initialPosition;

    // Use this for initialization
    void Start()
    {
        _initialPosition = transform.localPosition;
    }
    
    // Update is called once per frame
    void LateUpdate()
    {
        // Reset transform
        //transform.localPosition = _initialPosition;

        // Move towards screen
        var centerPos = transform.TransformPoint(new Vector3());
        var posTowardsScreen = new Vector3(centerPos.x, centerPos.y, centerPos.z - distance);
        var localPos = transform.InverseTransformPoint(posTowardsScreen);

        transform.localPosition = _initialPosition + localPos;

    }
}
