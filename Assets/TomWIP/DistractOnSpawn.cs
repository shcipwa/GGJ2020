using UnityEngine;

public class DistractOnSpawn : MonoBehaviour
{
    public float DistractionRange = 30f;
    public float DistractionTime = 10f;
    public LayerMask Mask;
    
    private void Start()
    {
        var hits = Physics.OverlapSphere(transform.position, DistractionRange, Mask, QueryTriggerInteraction.Ignore);
        foreach (var collider in hits)
        {
            // assumes SpookerAI lives on collider parent
            if (collider.transform.parent == null)
            {
                continue;
            }

            var spooker = collider.transform.parent.GetComponent<SpookerBehaviour>();
            if (spooker != null)
            {
                spooker.Distract(transform.position, DistractionTime);
            }
        }
    }
}
