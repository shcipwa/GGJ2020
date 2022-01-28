using UnityEngine;

public class BasicEmissionPulse : MonoBehaviour
{
    private static MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;

    public Color BaseColor = Color.white;
    public float BasePower = 0;
    public float Power = 3;
    public float Speed = 3;
    
    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _propertyBlock.Clear();
        var power = Mathf.Pow(Mathf.Sin(Time.realtimeSinceStartup * Speed) + BasePower + 1, Power);
        _propertyBlock.SetColor("_EmissiveColor", BaseColor * power);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
