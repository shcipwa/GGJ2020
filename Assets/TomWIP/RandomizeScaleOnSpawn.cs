using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizeScaleOnSpawn : MonoBehaviour
{
    public float MinScale = 1f;
    public float MaxScale = 1.4f;

    private void Awake()
    {
        var scale = Random.Range(MinScale, MaxScale);
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
