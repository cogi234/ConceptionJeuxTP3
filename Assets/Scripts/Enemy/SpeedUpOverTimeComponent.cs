using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SpeedUpOverTimeComponent : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] float timeBeforeUpgrade = 3;
    [SerializeField] float speedUpgrade = 0.1f;
    [SerializeField] float accelerationUpgrade = 0.5f;
    [SerializeField] float maxSpeed;

    float time = 0;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //The enemy accelerates over time, up to a maximum speed
        if (agent.speed < maxSpeed)
        {
            time += Time.deltaTime;

            if (time >= timeBeforeUpgrade)
            {
                time -= timeBeforeUpgrade;
                agent.speed += speedUpgrade;
                agent.acceleration += accelerationUpgrade;
            }
        } else if (agent.speed > maxSpeed)
            agent.speed = maxSpeed;
    }
}
