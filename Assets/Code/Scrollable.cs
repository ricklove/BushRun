using UnityEngine;
using System.Collections;

public class Scrollable : MonoBehaviour
{
    public GameObject Scroller = null;
    public Vector3 Scale = new Vector3(1, 1, 1);
    private Vector3 lastScrollerPosition;
    private Vector3 lastSelfPosition;

    private bool hasStarted = false;

    // Use this for initialization
    void Start()
    {
//        if (Scroller != null)
//        {
//            lastSelfPosition = gameObject.transform.localPosition;
//            lastScrollerPosition = Scroller.transform.localPosition;
//        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Scroller != null)
        {
            // TODO: Scale change

            // Use change since last
            if (hasStarted)
            {
                var selfChange = gameObject.transform.localPosition - lastSelfPosition;
                var scrollerChange = Scroller.transform.localPosition - lastScrollerPosition;

                var scaledChange = Vector3.Scale(scrollerChange, Scale);

                var diff = scaledChange - selfChange;

                gameObject.transform.localPosition += diff;
            }

            lastSelfPosition = gameObject.transform.localPosition;
            lastScrollerPosition = Scroller.transform.localPosition;
            hasStarted = true;
        }
    }
}
