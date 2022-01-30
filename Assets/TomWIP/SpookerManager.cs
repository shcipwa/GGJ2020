using System.Collections.Generic;
using UnityEngine;

public class SpookerManager : MonoBehaviour
{
    public static SpookerManager Instance;
    
    public List<SpookerBehaviour> Spookers = new List<SpookerBehaviour>();
    public SpookerBehaviour Prefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many SpookerManagers!", this);
        }
        Instance = this;
    }

    public void KillAllSpookers()
    {
        foreach (var spooker in Spookers)
        {
            Destroy(spooker.gameObject);
        }
        Spookers.Clear();
    }
}
