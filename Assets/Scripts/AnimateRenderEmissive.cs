using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRenderEmissive : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public float TriggerDelay = 0.1f;
    public float MaxBrightness = 5f;
    public Vector2Int GridCoord;
    public Color Color1, Color2;
    public float FadeSpeed = 1f;
    public float FadePower = 1f;
    private float _value = 0;

    private float _timeToTriggerAdjacent;

    public bool Trigger;

    
    // Start is called before the first frame update
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (Trigger)
        {
            _value = MaxBrightness;
            Trigger = false;
        }

        _value -= FadeSpeed * Time.time;
        SetRenderer();
    }

    private void SetRenderer()
    {
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        //_propBlock.SetColor("_EmissiveColor", Color.Lerp(Color1, Color2, (Mathf.Sin(Time.time * Speed + Offset) + 1) / 2f));
        _propBlock.SetFloat("_EmissiveAmount", Mathf.Pow(_value,FadePower));
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void TriggerCell()
    {
        _timeToTriggerAdjacent = Time.time + TriggerDelay;
        
    }
}
