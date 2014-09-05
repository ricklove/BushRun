using UnityEngine;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    public static HealthBarController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public ProgressBar healthBar = new ProgressBar();
    public ProgressBar progressBar = new ProgressBar();

    void Start()
    {
        SetHealth(1);
        SetProgress(1);
    }
        
    void OnGUI()
    {
        healthBar.DrawGUI();
        progressBar.DrawGUI();
    }

    public void SetHealth(float ratio)
    {
        healthBar.SetProgress(ratio);
    }

    public void SetProgress(float ratio)
    {
        progressBar.SetProgress(ratio);
    }
}

[System.Serializable]
public class ProgressBar
{
    public Rect Size;
    public Texture FillTexture;
    public Texture BackTexture;
    public int Units = 5;
    
    private float _ratio;
    
    public void DrawGUI()
    {
        GUI.DrawTexture(new Rect(Size.x, Size.y, _ratio * Size.width, Size.height), FillTexture);
        
        var unitWidth = Size.width / Units;
        for (int i = 0; i < Units; i++)
        {
            GUI.DrawTexture(new Rect(Size.x + i * unitWidth, Size.y, unitWidth, Size.height), BackTexture);
        }
    }
    
    public void SetProgress(float ratio)
    {
        _ratio = ratio;
    }
}
