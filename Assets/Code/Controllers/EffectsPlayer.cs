using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EffectsPlayer : MonoBehaviour
{
    public static EffectsPlayer Instance;
    
    void Awake()
    {
        Instance = this;
    }

    private GameObject[] _explosions;

    // Use this for initialization
    void Start()
    {
        _explosions = Resources.LoadAll<GameObject>("Explosions");
        //loadFlareSystems = Resources.LoadAll("Flares", typeof(GameObject));
        //loadDirectionalSystems = Resources.LoadAll("Directional", typeof(GameObject));
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void ShowExplosion(Vector3 position)
    {
        ShowAnyParticles(_explosions, position);
    }

    void ShowAnyParticles(GameObject[] particleSystems, Vector3 position)
    {
        if (particleSystems == null || particleSystems.Length <= 0)
        {
            return;
        }
        
        ShowParticles(particleSystems [Random.Range(0, particleSystems.Length)], position);
    }

    void ShowParticles(GameObject particleSystem, Vector3 position)
    {
        GameObject go = Instantiate(particleSystem, position, Quaternion.identity) as GameObject;
        Destroy(go, 10);
    }
}
