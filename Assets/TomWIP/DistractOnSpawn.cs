using System.Collections;
using UnityEngine;

public class DistractOnSpawn : MonoBehaviour
{
    public float DistractionRange = 30f;
    public float DistractionTime = 10f;
    public LayerMask Mask;
    
    // called by PoolSystem
    private void OnPoolSpawn()
    {
        // not in place at time of pool spawn so need to defer
        StartCoroutine(DistractRoutine());
    }

    private IEnumerator DistractRoutine()
    {
        yield return null;
        
        var hits = Physics.OverlapSphere(transform.position, DistractionRange, Mask, QueryTriggerInteraction.Ignore);
        //Debug.Log($"OverlapSphere got {hits.Length} hits");
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
                //Debug.Log($"Trying to distract spooker @ {transform.position}", this);
                spooker.Distract(transform.position, DistractionTime);
            }
        }
    }
}
