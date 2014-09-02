using UnityEngine;
using System.Collections;

public class HeadSelection : MonoBehaviour
{

    public Sprite head;

    // Use this for initialization
    void Awake()
    {
        var headRenderer = GetComponentInChildren<SpriteRenderer>();
        headRenderer.sprite = head;
    }
	
    // Update is called once per frame
    void Update()
    {
    }
}
