using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private Vector3 _startPos;
    public float RotationSpeed;
    public Rigidbody RootBody;

    // Start is called before the first frame update
    void Start()
    {
        _startPos = RootBody.position;
    }

    // Update is called once per frame
    void Update()
    {
        RootBody.MovePosition(_startPos + new Vector3(Mathf.PerlinNoise(Time.time/3 + _startPos.x, Time.time/3 + _startPos.x),
            Mathf.PerlinNoise(Time.time/3 + _startPos.x, Time.time/3 + _startPos.x)));
        
        RootBody.MoveRotation(RootBody.rotation *= Quaternion.Euler(0,RotationSpeed * Mathf.PerlinNoise(Time.time + _startPos.x,0),0));
        
    }
}
