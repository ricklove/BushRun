using UnityEngine;
using System.Collections;

public class HeadSelection : MonoBehaviour
{

    public Sprite[] heads = new Sprite[2];
    private SpriteRenderer headRenderer;

    private float nextChangeTime = 0;
    private int iHead = 0;

    // Use this for initialization
    void Awake()
    {
        headRenderer = GetComponentInChildren<SpriteRenderer>();
        headRenderer.sprite = heads [0];

        if (heads.Length < 2)
        {
            heads = new Sprite[]{heads [0], heads [1]};
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextChangeTime)
        {
            nextChangeTime = Random.Range(Time.time, Time.time + 10);

            iHead++;

            if (iHead >= heads.Length)
            {
                iHead = 0;
            }

            headRenderer.sprite = heads [iHead];
        } 
    }
}
