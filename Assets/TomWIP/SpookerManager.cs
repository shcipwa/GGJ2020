using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookerManager : MonoBehaviour
{
    public static SpookerManager Instance;
    
    public List<SpookerBehaviour> Spookers = new List<SpookerBehaviour>();
    public List<SpookerSpawnPoint> SpawnPoints = new List<SpookerSpawnPoint>();
    public SpookerBehaviour Prefab;

    public int SpawnCountOnRespawn = 10;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many SpookerManagers!", this);
        }
        Instance = this;
        
        SpawnPoints.Clear();

        if (Spookers.Count == 0)
        {
            StartCoroutine(StartSpawnRoutine());
        }
    }

    private IEnumerator StartSpawnRoutine()
    {
        yield return new WaitForSeconds(2f);
        OnPlayerRespawn();
    }

    public void OnPlayerRespawn()
    {
        SpawnSpookers(SpawnCountOnRespawn);
    }

    public void SpawnSpookers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
            var spooker = Instantiate(Prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            Spookers.Add(spooker);
        }
    }
    
    public void KillAllSpookers()
    {
        foreach (var spooker in Spookers)
        {
            Destroy(spooker.gameObject);
        }
        Spookers.Clear();
    }

    public void RegisterSpawnPoint(SpookerSpawnPoint spawnPoint)
    {
        SpawnPoints.Add(spawnPoint);
    }
}
