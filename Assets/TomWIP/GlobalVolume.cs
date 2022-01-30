using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GlobalVolume : MonoBehaviour
{
    private static Volume Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many global volumes!", this);
            return;
        }

        Instance = GetComponent<Volume>();

        if (Instance == null)
        {
            Debug.LogError("Couldn't find global volume!", this);
        }
    }

    public static VolumeProfile GetProfile()
    {
        return Instance.profile;
    }
}
