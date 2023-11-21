using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BirdPatrol : MonoBehaviour
{
    [SerializeField] float Speed = 4;
    [SerializeField] List<Transform> waypoints;

    Transform player;
    AudioSource scream;
    Node root;

    private void Awake()
    {
        scream = GameObject.FindGameObjectWithTag("TetrisBird").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetupTree();
    }

    public Detect detect;
    private void SetupTree()
    {
        detect = new Detect(transform);

        Node @return = new Return(transform, Speed);
        Node Invert1 = new Inverter(new List<Node> { @return });
        Node Seq1 = new Sequence(new List<Node> { Invert1, detect });
        Node alert = new AlertPhase(scream);
        Node pursuit = new Pursuit(player, transform, Speed);
        Node Seq2 = new Sequence(new List<Node> { Seq1, alert, pursuit });
        Node patrol = new Patrol(transform, waypoints, Speed);
        Node sel1 = new Selector(new List<Node>() { Seq2, @return, patrol });

        root = sel1;
    }

    void Update()
    {
        root.Evaluate();
    }
}
