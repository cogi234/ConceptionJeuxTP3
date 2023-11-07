using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class PatrouilleOiseau : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float detectionRange;
    NavMeshAgent agent;
    [SerializeField] List<Transform> destination;
    [SerializeField] float Waitime = 2;
    Node root;

    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SetupTree();

    }

    public Node l1;
    private void SetupTree()
    {
        l1 = new Detect();
        Node l2 = new Retour();
        Node Invert1 = new Inverter(new List<Node> {l2});
        Node Seq1 = new Sequence(new List<Node> { Invert1, l1 });
        Node l3 = new AlertPhase();
        Node l4 = new Poursuite();
        Node Seq2 = new Sequence(new List<Node> { Seq1, l3,l4 });
        Node l5 = new Patrouille();
        Node sel1 = new Selector(new List<Node>() { Seq1, l2,l5 });

        root = sel1;


    }
    // Update is called once per frame
    void Update()
    {
        root.Evaluate();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }

}
