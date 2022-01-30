using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(SpookerAttack))]
[RequireComponent(typeof(NavMeshAgent))]
public class SpookerBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Wandering,
        Attacking,
        Distracted
    }

    private NavMeshAgent _navAgent;
    private SpookerAttack _attack;

    public Animator SpookerAnimator;
    public Transform FocusAimer;
    public MultiAimConstraint AimConstraint;
    public MultiRotationConstraint AimTwistHelper;
    public Transform VisionPoint;
    public LayerMask VisionOccluders;
    public float VisionDistance = 20;
    public float WanderSpeed = 1f;
    public float ChaseSpeed = 2.8f;
    
    public State CurrentState;

    private float _idleTime;
    private float _timeToWait;
    private float _timeSinceSeen;
    private float _focusTimer;
    private bool _hasFocus;
    private Vector3 _focusPosition;
    private Transform _focusObject;
    private Coroutine _focusRoutine;
    private Coroutine _focusInRoutine;
    private Coroutine _focusOutRoutine;

    private float _distractTimer;
    

    private int _moveParamHash = Animator.StringToHash("MoveSpeed");

    private void Awake()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        _attack = GetComponent<SpookerAttack>();
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Wandering:
                WanderingUpdate();
                break;
            case State.Attacking:
                AttackingUpdate();
                break;
            case State.Distracted:
                DistractedUpdate();
                break;
        }
        
        UpdateAnimator();
    }

    private void IdleUpdate()
    {
        if (ShouldGoAggressive())
        {
            BeginAttacking();
            return;
        }

        _idleTime += Time.deltaTime;
        if (_idleTime >= _timeToWait)
        {
            BeginWandering();
        }
    }

    private void WanderingUpdate()
    {
        if (ShouldGoAggressive())
        {
            BeginAttacking();
            return;
        }
        
        if (Vector3.SqrMagnitude(transform.position - _navAgent.destination) < .1f)
        {
            BeginIdle();
        }
    }

    private void AttackingUpdate()
    {
        if (HasVision())
        {
            _timeSinceSeen = 0f;
            var playerPos = PlayerTag.Instance.transform.position;
            _navAgent.SetDestination(playerPos);

            if (Vector3.SqrMagnitude(playerPos - transform.position) <= 4f)
            {
                _attack.DoAttack();
            }
        }
        else
        {
            _timeSinceSeen += Time.deltaTime;
            if (_timeSinceSeen >= 5f)
            {
                BeginIdle();
            }
        }
    }

    private void DistractedUpdate()
    {
        _distractTimer -= Time.deltaTime;
        if (_distractTimer <= 0f)
        {
            if (ShouldGoAggressive())
            {
                BeginAttacking();
            }
            else
            {
                BeginWandering();
            }
        }
    }

    private void UpdateAnimator()
    {
        SpookerAnimator.SetFloat(_moveParamHash, _navAgent.velocity.magnitude);
    }

    private IEnumerator BeginFocusRoutine()
    {
        while (AimConstraint.weight < 1f)
        {
            AimConstraint.weight += Time.deltaTime * 2f;
            AimTwistHelper.weight += Time.deltaTime * 2f;
            yield return null;
        }

        _focusInRoutine = null;
    }

    private IEnumerator EndFocusRoutine()
    {
        while (AimConstraint.weight > 0f)
        {
            AimConstraint.weight -= Time.deltaTime * 2f;
            AimTwistHelper.weight -= Time.deltaTime * 2f;
            yield return null;
        }

        _focusOutRoutine = null;
    }

    private IEnumerator FocusRoutine()
    {
        _hasFocus = true;
        
        if (_focusOutRoutine != null)
        {
            StopCoroutine(_focusOutRoutine);
            _focusOutRoutine = null;
        }

        if (AimConstraint.weight < 1f && _focusInRoutine == null)
        {
            _focusInRoutine = StartCoroutine(BeginFocusRoutine());
        }

        while (_focusTimer > 0)
        {
            _focusTimer -= Time.deltaTime;

            if (_focusObject != null)
            {
                _focusPosition = _focusObject.position;
            }

            FocusAimer.position = _focusPosition;
            
            yield return null;
        }

        if (_focusInRoutine != null)
        {
            StopCoroutine(_focusInRoutine);
            _focusInRoutine = null;
        }

        _hasFocus = false;
        _focusRoutine = null;
        _focusOutRoutine = StartCoroutine(EndFocusRoutine());
    }

    private void BeginIdle()
    {
        CurrentState = State.Idle;
        _idleTime = 0f;
        _timeToWait = Random.Range(5f, 15f);
    }

    private void BeginWandering()
    {
        _navAgent.speed = WanderSpeed;
        
        var count = 0;
        var target = Vector3.zero;
        while (!FindRandomWanderTarget(out target))
        {
            count++;
            if (count >= 100)
            {
                Debug.LogError("Couldn't find a wander target in 100 iterations, returning to idle", this);
                BeginIdle();
                return;
            }
        }

        _navAgent.SetDestination(target);
        CurrentState = State.Wandering;
    }

    private void BeginAttacking()
    {
        CurrentState = State.Attacking;
        _navAgent.speed = ChaseSpeed;
        _navAgent.SetDestination(PlayerTag.Instance.transform.position);
        _timeSinceSeen = 0f;
    }

    private void BeginDistraction(Vector3 distractionPosition, float distractionTime)
    {
        CurrentState = State.Distracted;
        _navAgent.speed = ChaseSpeed;
        _navAgent.SetDestination(distractionPosition);
        _distractTimer = distractionTime;
        DrawFocus(distractionPosition, distractionTime);
    }

    private bool FindRandomWanderTarget(out Vector3 target)
    {
        var randomPos = transform.position + Random.insideUnitSphere * Random.Range(5f, 15f);
        if (!NavMesh.SamplePosition(randomPos, out var hit, 3f, NavMesh.AllAreas))
        {
            target = Vector3.zero;
            return false;
        }

        target = hit.position;
        return true;
    }

    private bool ShouldGoAggressive()
    {
        return IsGridActive() && HasVision();
    }
    
    private bool HasVision()
    {
        var playerPos =PlayerTag.Instance.transform.position + Vector3.up;
        var visionPoint = VisionPoint.position;
        var direction = playerPos - visionPoint;
        
        if (Vector3.Dot(transform.forward, direction) < 0)
        {
            return false;
        }
        
        var ray = new Ray
        {
            direction = direction,
            origin = visionPoint
        };
        
        var result= Physics.Raycast(ray, Mathf.Min(VisionDistance, direction.magnitude), VisionOccluders, QueryTriggerInteraction.Ignore);
        Debug.DrawLine(visionPoint, visionPoint + direction * Mathf.Min(VisionDistance, direction.magnitude),
            result ? Color.red : Color.green);        
        return !result;
    }

    private bool IsGridActive()
    {
        // todo: implement a real check
        return false;
    }

    public void Distract(Vector3 distractionPosition, float distrtactionLength)
    {
        BeginDistraction(distractionPosition, distrtactionLength);
    }
    
    public void DrawFocus(Transform target, float focusTime = 1f)
    {
        _focusObject = target;
        _focusTimer = focusTime;

        if (_focusRoutine == null)
        {
            _focusRoutine = StartCoroutine(FocusRoutine());
        }
    }

    public void DrawFocus(Vector3 position, float focusTime = 1f)
    {
        _focusObject = null;
        _focusPosition = position;
        _focusTimer = focusTime;

        if (_focusRoutine == null)
        {
            _focusRoutine = StartCoroutine(FocusRoutine());
        }
    }
}
