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

    Detect test;
    public Detect l1;
    private void SetupTree()
    {
        l1 = new Detect(gameObject.transform);

        Node l2 = new Return(gameObject, Speed);
        Node Invert1 = new Inverter(new List<Node> { l2 });
        Node Seq1 = new Sequence(new List<Node> { Invert1, l1 });
        Node l3 = new AlertPhase(scream);
        Node l4 = new Pursuit(player, gameObject.transform, Speed);
        Node Seq2 = new Sequence(new List<Node> { Seq1, l3, l4 });
        Node l5 = new Patrol(gameObject.transform, waypoints, Speed);
        Node sel1 = new Selector(new List<Node>() { Seq2, l2, l5 });

        root = sel1;
    }

    void Update()
    {
        root.Evaluate();
    }
}
