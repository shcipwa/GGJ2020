using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnVolume : MonoBehaviour
{
    public Vector3 Size = new Vector3(100, 1, 100);
    public int SpawnCount;
    public GameObject Prefab;
    public bool SpawnOnAwake = false;

    private void Awake()
    {
        if (SpawnOnAwake)
        {
            DoAllSpawns();
        }
    }

    [ContextMenu("Do spawns")]
    private void DoAllSpawns()
    {
        for (int i = 0; i < SpawnCount; i++)
        {
            DoSpawn(GetSpawnPosition());
        }
    }
    
    private Vector3 GetSpawnPosition()
    {
        var volumePoint = new Vector3(Random.Range(-Size.x, Size.x),
                                    Random.Range(-Size.y, Size.y),
                                    Random.Range(-Size.z, Size.z));
        var rotatedPoint = transform.rotation * volumePoint;
        return transform.position + rotatedPoint;
    }
    
    private void DoSpawn(Vector3 position)
    {
        Instantiate(Prefab, position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        var matrixCache = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Size);
        Gizmos.matrix = matrixCache;
    }
}
