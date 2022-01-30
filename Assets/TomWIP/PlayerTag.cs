using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    public static PlayerTag Instance;
    public static PlayerHealth Health;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Too many player tags!", this);
        }

        Instance = this;
        Health = GetComponent<PlayerHealth>();
        if (Health == null)
        {
            Debug.LogError("Missing PlayerHealth component", this);
        }
    }
}
