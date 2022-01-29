using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    public static PlayerTag Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many player tags!", this);
        }

        Instance = this;
    }
}
