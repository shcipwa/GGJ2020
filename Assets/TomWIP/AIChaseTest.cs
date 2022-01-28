using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChaseTest : MonoBehaviour
{
    public GameObject Player;
    public NavMeshAgent ThisAgent;

    private void Update()
    {
        ThisAgent.SetDestination(Player.transform.position);
    }
}
