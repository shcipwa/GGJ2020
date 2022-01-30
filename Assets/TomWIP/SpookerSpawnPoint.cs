using UnityEngine;

public class SpookerSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        SpookerManager.Instance.RegisterSpawnPoint(this);
    }
}
