using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BigManManager))]
public class BigManManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Gather Big Men"))
        {
            var bmm = (BigManManager)target;
            bmm.BigMen = FindObjectsOfType<BigManBehaviour>(true);
            EditorUtility.SetDirty(bmm);
        }
        base.OnInspectorGUI();
    }
}
#endif

public class BigManManager : MonoBehaviour
{
    public static BigManManager Instance;

    public BigManBehaviour[] BigMen;
    public float BigManTimeToSmash = 120f;

    public bool WeWereActuallyAbleToGetTheGridCollapseWorkingItsAMiracle;

    public float WakeUpIntervals = 120f;

    private int _awokenCount;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many BigManManager", this);
            return;
        }
        Instance = this;
        
        if(!WeWereActuallyAbleToGetTheGridCollapseWorkingItsAMiracle)
        {
            SleepAndReset();
        }
        else
        {
            WakeUp();
        }
    }

    private void Update()
    {
        if (WeWereActuallyAbleToGetTheGridCollapseWorkingItsAMiracle)
        {
            return;
        }

        var targetAwokenCount = Mathf.Floor(PlayerTag.Health.TimeAlive / WakeUpIntervals);
        if (_awokenCount < targetAwokenCount && _awokenCount < BigMen.Length)
        {
            var man = BigMen[_awokenCount];
            man.WakeUp(float.MaxValue);
            _awokenCount++;
            Debug.Log("A big man has awoken", man);
        }
    }

    [ContextMenu("Wake em up")]
    public void WakeUp()
    {
        foreach (var man in BigMen)
        {
            man.WakeUp(WeWereActuallyAbleToGetTheGridCollapseWorkingItsAMiracle ? BigManTimeToSmash : float.MaxValue);
        }
    }

    public void SleepAndReset()
    {
        _awokenCount = 0;
        foreach (var man in BigMen)
        {
            man.SleepAndReset();
        }
    }
}
