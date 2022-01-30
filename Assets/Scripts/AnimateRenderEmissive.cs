using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public enum GridEventType
{
    None,
    WhiteFlash,
    DestroyGrid,
    FailRepair,
    RepairGrid
}
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
    public float BreakForce = 1f;
    public float BrokenProbability = 0.2f;
    private float _value = 0;
    private GridEventType _currentEvent = GridEventType.None;

    private float _timeToTriggerAdjacent = -1;

    public bool Trigger;
    private Rigidbody _body;
    private bool _isDestroyed;
    private Vector3 _originalPosition;
    private Vector3 _repairPosition;
    private Quaternion _repairRotation;
    private Color _currentColor;

    private bool _needsRepair;
    private float _startValue = 1;


    public bool IsActive()
    {
        return !_isDestroyed;
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        FadeSpeed += Random.value * FadeSpeedRandom;
        _body = GetComponent<Rigidbody>();

        _originalPosition = transform.position;

    }

    // Update is called once per frame
    void Update()
    { 
        if (Trigger)
        {
            Trigger = false;
            TriggerCell(GridEventType.WhiteFlash);
        }

        if (_timeToTriggerAdjacent > 0 && _timeToTriggerAdjacent < Time.time)
        {
            _timeToTriggerAdjacent = -1;
            GridManager.Instance.TriggerAdjacent(GridCoord,_currentEvent);
        }
        
        _value -= FadeSpeed * Time.deltaTime;
        
        if (_value <= 0)
        {
            //_renderer.enabled = false;
            _value = 0;
            _currentEvent = GridEventType.None;
        }
        else
        {
            if (_currentEvent == GridEventType.RepairGrid)
            {
                transform.position = Vector3.Lerp(_repairPosition, _originalPosition,1-_value);
                transform.rotation = Quaternion.Lerp(_repairRotation, Quaternion.identity,1-_value);
                var currentScaledValue = _value * (1-_startValue);
                transform.localScale = Mathf.Lerp(0.8f,2,1-currentScaledValue) * Vector3.one;
            }

            if (_currentEvent == GridEventType.DestroyGrid || _currentEvent == GridEventType.FailRepair)
            {
                var currentScaledValue = _value * (1-_startValue);
                transform.localScale = Mathf.Lerp(2,0.8f,1-currentScaledValue) * Vector3.one;
            }

            _renderer.enabled = true;
        }
        SetRenderer();
    }
    
    public void TriggerCell(GridEventType eventType)
    {
        if (!_isDestroyed && eventType == GridEventType.WhiteFlash && _value <= 0)
        {
            _startValue = _value;
            _currentColor = Color.white;
            _value = 1;
            _timeToTriggerAdjacent = Time.time + TriggerDelay + Random.value * TriggerDelayRandom;
            _currentEvent = GridEventType.WhiteFlash;
        }
        
        if (!_isDestroyed && (eventType == GridEventType.DestroyGrid || eventType == GridEventType.FailRepair) && 
            (_currentEvent != GridEventType.DestroyGrid || _currentEvent != GridEventType.FailRepair))
        {
            _startValue = _value;
            _value = 1;
            _currentColor = Color.red;
            _body.isKinematic = false;
            _body.AddForceAtPosition(Random.onUnitSphere * BreakForce,transform.position + Random.onUnitSphere,ForceMode.Impulse);
            _isDestroyed = true;
            _timeToTriggerAdjacent = Time.time + .25f;
            
            _currentEvent = eventType;

            if (eventType == GridEventType.DestroyGrid)
            {
                _needsRepair = Random.value < BrokenProbability;
            }
        }

        if (eventType == GridEventType.RepairGrid && _needsRepair && (_currentEvent != GridEventType.DestroyGrid && _currentEvent != GridEventType.FailRepair))
        {
            _isDestroyed = false;
            TriggerCell(GridEventType.FailRepair);
            return;
        }
        else if (eventType == GridEventType.RepairGrid && (_currentEvent != GridEventType.DestroyGrid && _currentEvent != GridEventType.FailRepair) && _isDestroyed)
        {
            _startValue = _value;
            _value = 1;
            _isDestroyed = false;
            _body.isKinematic = true;

            _repairPosition = transform.position;
            _repairRotation = transform.rotation;
            _timeToTriggerAdjacent = Time.time + .5f;
            _currentEvent = GridEventType.RepairGrid;
            _currentColor = Color.white;
        }
    }

    private void SetRenderer()
    {
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        
        if (_needsRepair)
        {
            _propBlock.SetColor("_EmissiveColor", Color.white);
            _propBlock.SetFloat("_EmissiveAmount", (Time.time % 0.25 < 0.1f) ? 1 : 0f);
        }
        else
        {
            if (_currentEvent == GridEventType.RepairGrid)
            {
                _propBlock.SetFloat("_EmissiveAmount", Mathf.Pow(_value,0.2f) * MaxBrightness);
                _propBlock.SetColor("_EmissiveColor", _currentColor);
            }
            else
            {
                _propBlock.SetColor("_EmissiveColor", _currentColor);
                _propBlock.SetFloat("_EmissiveAmount", Mathf.Pow(_value,FadePower) * MaxBrightness);
            }
        }
        
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    public bool Repair()
    {
        var neededRepair = _needsRepair;
        _needsRepair = false;

        return neededRepair;
    }
}
