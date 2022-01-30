using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialization()
    {
        Debug.Log("Doing init stuff");
        DOTween.SetTweensCapacity(1000, 100);
        
        
    }
}