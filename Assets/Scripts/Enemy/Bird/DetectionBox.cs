using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectionbox : MonoBehaviour
{
    GameObject bird;
    BirdPatrol birdPatrol;

    void Start()
    {
        bird = GameObject.FindGameObjectWithTag("TetrisBird");
        birdPatrol = GameObject.FindGameObjectWithTag("TetrisBird").GetComponent<BirdPatrol>();
        transform.position = bird.transform.position;
    }
    private void Update()
    {
        transform.position = bird.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            birdPatrol.l1.detection = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
            birdPatrol.l1.detection = false;
    }
}