using UnityEngine;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    public static HealthBarController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public Rect Size;
    public Texture FillTexture;
    public Texture BackTexture;
    public int Units = 5;

    private float _ratio;

    void Start()
    {
        SetHealth(1);
    }
        
    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Size.x, Size.y, _ratio * Size.width, Size.height), FillTexture);

        var unitWidth = Size.width / Units;
        for (int i = 0; i < Units; i++)
        {
            GUI.DrawTexture(new Rect(Size.x + i * unitWidth, Size.y, unitWidth, Size.height), BackTexture);
        }
    }

    public void SetHealth(float ratio)
    {
        _ratio = ratio;
    }
}
