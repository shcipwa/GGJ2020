using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRenderEmissive : MonoBehaviour
{
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    public float TriggerDelayRandom = 0.01f;
    public float TriggerDelay = 0.1f;
    public float MaxBrightness = 5f;
    public Vector2Int GridCoord;
    public Color Color1, Color2;
    public float FadeSpeed = 1f;
    public float FadeSpeedRandom = 0.2f;
    public float FadePower = 1f;
    private float _value = 0;

    private float _timeToTriggerAdjacent = -1;

    public bool Trigger;

    
    // Start is called before the first frame update
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        FadeSpeed += Random.value * FadeSpeedRandom;
    }

    // Update is called once per frame
    void Update()
    { 
        if (Trigger)
        {
            TriggerCell();
        }

        if (_timeToTriggerAdjacent > 0 && _timeToTriggerAdjacent < Time.time)
        {
            _timeToTriggerAdjacent = -1;
            GridManager.Instance.TriggerAdjacent(GridCoord);
        }

        
        _value -= FadeSpeed * Time.deltaTime;
        if (_value < 0)
        {
            _value = 0;
        }
        SetRenderer();
    }
    
    public void TriggerCell()
    {
        if (_value > 0)
        {
            return;
        }
        
        _value = 1;
        Trigger = false;
        
        _timeToTriggerAdjacent = Time.time + TriggerDelay + Random.value * TriggerDelayRandom;
    }

    private void SetRenderer()
    {
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        //_propBlock.SetColor("_EmissiveColor", Color.Lerp(Color1, Color2, (Mathf.Sin(Time.time * Speed + Offset) + 1) / 2f));
        _propBlock.SetFloat("_EmissiveAmount", Mathf.Pow(_value,FadePower) * MaxBrightness);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }
}
