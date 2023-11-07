using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MoveToPlayerComponent : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        agent.destination = player.transform.position;
        //Animation stuff
        animator.SetBool("Grounded", !agent.isOnOffMeshLink); //If we're on an offmeshlink, we're not grounded
        animator.SetFloat("X", (transform.worldToLocalMatrix * agent.velocity).x);
        animator.SetFloat("Z", (transform.worldToLocalMatrix * agent.velocity).z);
        Debug.Log((transform.worldToLocalMatrix * agent.velocity).z);
    }
}
