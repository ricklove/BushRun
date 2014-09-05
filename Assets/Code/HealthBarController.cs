using UnityEngine;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
    public static HealthBarController Instance;
    
    void Awake()
    {
        Instance = this;
    }

    public Rect HealthBarDimens;
    public bool VerticleHealthBar;
    public Texture HealthBubbleTexture;
    public Texture HealthTexture;
    public float HealthBubbleTextureRotation;
    private HealthSystem _healthBar;
    
    void Start()
    {
        _healthBar = new HealthSystem(HealthBarDimens, VerticleHealthBar, HealthBubbleTexture, HealthTexture, HealthBubbleTextureRotation);
        _healthBar.Initialize();

        SetHealth(1);
    }
        
    void OnGUI()
    {
        _healthBar.DrawBar();
    }

    public void SetHealth(float ratio)
    {
        _healthBar.SetRatio(ratio);
    }
}
