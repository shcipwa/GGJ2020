using UnityEngine;

public class BigManManager : MonoBehaviour
{
    public static BigManManager Instance;

    public BigManBehaviour[] BigMen;
    public float BigManTimeToSmash = 120f;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many BigManManager", this);
            return;
        }
        Instance = this;
    }

    public void WakeUp()
    {
        foreach (var man in BigMen)
        {
            man.WakeUp(BigManTimeToSmash);
        }
    }

    public void SleepAndReset()
    {
        foreach (var man in BigMen)
        {
            man.SleepAndReset();
        }
    }
}
