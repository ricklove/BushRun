using UnityEngine;
using System.Collections;

public class HeadSelection : MonoBehaviour
{

    public Sprite[] idleHeads;
    public Sprite[] happyHeads;
    public Sprite[] hurtHeads;
    private SpriteRenderer headRenderer;
    private float nextChangeTime = 0;
    private int iHead = 0;
    private HeadType headState;

    // Use this for initialization
    void Awake()
    {
        headRenderer = GetComponentInChildren<SpriteRenderer>();
        if (idleHeads.Length > 0)
        {
            headRenderer.sprite = idleHeads [0];
        }

    }
    
    // Update is called once per frame
    void Update()
    {
        Sprite[] heads = null;

        switch (headState)
        {
            case HeadType.Happy:
                heads = happyHeads;
                break;
            case HeadType.Hurt:
                heads = hurtHeads;
                break;
            case HeadType.Idle:
            default:
                heads = idleHeads;
                break;
        }

        if (heads == null
            || heads.Length <= 0)
        {
            heads = idleHeads;
        }

        if (heads == null)
        {
            return;
        }

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

    public void ChangeHead(HeadType headType)
    {
        headState = headType;
        nextChangeTime = -1;
    }
}

public enum HeadType
{
    Idle,
    Happy,
    Hurt
}
