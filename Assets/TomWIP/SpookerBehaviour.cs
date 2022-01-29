using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SpookerAttack))]
[RequireComponent(typeof(NavMeshAgent))]
public class SpookerBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Wandering,
        Attacking
    }

    private NavMeshAgent _navAgent;
    private SpookerAttack _attack;

    public Animator SpookerAnimator;
    public Transform VisionPoint;
    public LayerMask VisionOccluders;
    public float VisionDistance = 20;
    public float WanderSpeed = 1f;
    public float ChaseSpeed = 2.8f;
    
    public State CurrentState;

    private float _idleTime;
    private float _timeToWait;
    private float _timeSinceSeen;

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
        }
        
        UpdateAnimator();
    }

    private void IdleUpdate()
    {
        if (HasVision())
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
        if (HasVision())
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

    private void UpdateAnimator()
    {
        SpookerAnimator.SetFloat(_moveParamHash, _navAgent.velocity.magnitude);
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
}
