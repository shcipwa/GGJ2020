using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class BigManBehaviour : MonoBehaviour
{
    public MultiAimConstraint AimConstraint;
    public MultiRotationConstraint AimTwistHelper;
    public Transform StartPosition;
    public Transform EndPosition;
    public Transform BigManRoot;
    public Animator BigManAnimator;
    public Animator FistAnimator;

    private float _smashTime = -1;

    private TweenerCore<Vector3, Vector3, VectorOptions> _mover;

    public void WakeUp(float moveTime)
    {
        BigManRoot.gameObject.SetActive(true);
        BigManRoot.transform.position = StartPosition.position;
        
        AimConstraint.weight = 1f;
        AimTwistHelper.weight = 1f;
        _mover = BigManRoot.DOMove(EndPosition.position, moveTime);
        _mover.onComplete = SmashGrid;
        
        BigManAnimator.SetFloat("MoveSpeed", 1f);
    }

    private void SmashGrid()
    {
        BigManAnimator.SetFloat("MoveSpeed", 0f);
        FistAnimator.Play("Clench");
        BigManAnimator.Play("Smash", 1);

        _smashTime = Time.time + .75f;
    }

    public void Update()
    {
        if (_smashTime > 0 && _smashTime < Time.time)
        {
            GridManager.Instance.TriggerNearest(transform.position,GridEventType.DestroyGrid);
        }
    }

    public void SleepAndReset()
    {
        AimConstraint.weight = 0f;
        AimTwistHelper.weight = 0f;

        if (_mover != null)
        {
            _mover.Kill();
            _mover = null;
        }

        BigManRoot.position = StartPosition.position;
        BigManRoot.gameObject.SetActive(false);
        Debug.Log($"BigMan {BigManRoot.name} went to sleep", BigManRoot);
        
        FistAnimator.Play("Relax");
        BigManAnimator.SetFloat("MoveSpeed", 0f);
    }
}
