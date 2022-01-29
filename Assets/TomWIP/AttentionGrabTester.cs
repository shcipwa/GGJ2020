using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AttentionGrabTester))]
public class AttentionGrabTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Get attention"))
        {
            var tester = (AttentionGrabTester)target;
            tester.GrabAttention();
        }
        base.OnInspectorGUI();
    }
}
#endif

public class AttentionGrabTester : MonoBehaviour
{
    public float Range = 10f;
    public float AttentionTime = 2f;
    
    public void GrabAttention()
    {
        var hits = Physics.OverlapSphere(transform.position, Range);
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
                spooker.DrawFocus(transform, AttentionTime);
            }
        }
    }
}
