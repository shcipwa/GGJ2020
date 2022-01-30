using DG.Tweening;
using UnityEngine;

public class HentaiBehaviour : MonoBehaviour
{
    private const float BUFFER_ZONE_SIZE = 1f;
    
    public float Speed = 7f;
    public float RotationSize = 1080;
    public AnimationCurve MoveCurve;
    public LayerMask CollisionCheckMask;
    
    private void Start()
    {
        TweenMove();
    }

    private void TweenMove()
    {
        var timespan = Random.Range(0.75f, 1.33f);
        var offset = Random.onUnitSphere;
        var collision = Physics.SphereCast(transform.position, 0.5f, offset, out var hitInfo, Speed, CollisionCheckMask, QueryTriggerInteraction.Ignore);
        Vector3 finalPosition;
        if (collision)
        {
            finalPosition = hitInfo.point - offset * BUFFER_ZONE_SIZE;
        }
        else
        {
            finalPosition = transform.position + offset * Speed;
        }
        
        var moveHandle = transform.DOMove(finalPosition, timespan).SetEase(MoveCurve);
        moveHandle.onComplete = TweenMove;

        transform.DORotate(offset * RotationSize, timespan, RotateMode.FastBeyond360).SetEase(MoveCurve);
    }
}
