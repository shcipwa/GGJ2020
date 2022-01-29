using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookerMoveTest : MonoBehaviour
{
    [Range(min:0f,max:10f)]
    public float MoveSpeed = 0f;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.Translate(transform.forward * MoveSpeed * Time.deltaTime);
        _animator.SetFloat("MoveSpeed", MoveSpeed);
    }
}
