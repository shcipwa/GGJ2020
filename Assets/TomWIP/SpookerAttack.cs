using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpookerAttack : MonoBehaviour
{
    public Transform LeftHandIKTarget;
    public Transform RightHandIKTarget;
    public ChainIKConstraint LeftHandIKConstraint;
    public ChainIKConstraint RightHandIKConstraint;
    public Transform HandBone;
    public LayerMask AttackCollision;

    public float AttackSpeed = 2f;
    public float AttackCooldown = 1f;
    
    private bool _attacking;

    public void DoAttack()
    {
        if (_attacking)
        {
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _attacking = true;

        // fade in arm IK
        while (LeftHandIKConstraint.weight < 1f)
        {
            var deltaTime = Time.deltaTime;
            var playerPos = PlayerTag.Instance.transform.position + Vector3.up;
            LeftHandIKConstraint.weight += deltaTime * AttackSpeed;
            RightHandIKConstraint.weight += deltaTime * AttackSpeed;
            LeftHandIKTarget.transform.position = playerPos;
            RightHandIKTarget.transform.position = playerPos;
            yield return null;
        }
        
        // attack hit check
        if (Physics.CheckSphere(HandBone.position, 0.33f, AttackCollision, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("HIT PLAYER", this);
            PlayerTag.Health.TakeDamage();
        }
        
        // fade out arm IK
        while (LeftHandIKConstraint.weight > 0f)
        {
            var deltaTime = Time.deltaTime;
            var playerPos = PlayerTag.Instance.transform.position + Vector3.up;
            LeftHandIKConstraint.weight -= deltaTime * AttackSpeed;
            RightHandIKConstraint.weight -= deltaTime * AttackSpeed;
            LeftHandIKTarget.transform.position = playerPos;
            RightHandIKTarget.transform.position = playerPos;
            yield return null;
        }

        // attack cooldown
        var timer = Time.deltaTime;
        while (timer < AttackCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _attacking = false;
    }
}
