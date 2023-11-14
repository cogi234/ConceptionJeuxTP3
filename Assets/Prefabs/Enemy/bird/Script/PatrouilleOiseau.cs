using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class PatrouilleOiseau : MonoBehaviour
{
   
    [SerializeField] float Speed = 4;

    [SerializeField] List<Transform> destination;
    Transform joueur;

    AudioSource cri;
    Node root;

    // Start is called before the first frame update
    void Start()
    {
        cri = GameObject.FindGameObjectWithTag("TetrisBird").GetComponents<AudioSource>()[0];
       
        joueur = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void Awake()
    {

        SetupTree();

    }
    Detect teste;
    public Detect l1;
    private void SetupTree()
    {
        l1 = new Detect();
         
        Node l2 = new Retour(gameObject, Speed);
        Node Invert1 = new Inverter(new List<Node> {l2});
        Node Seq1 = new Sequence(new List<Node> { Invert1, l1 });
        Node l3 = new AlertPhase(cri);
        Node l4 = new Poursuite(joueur,gameObject.transform,Speed);
        Node Seq2 = new Sequence(new List<Node> { Seq1, l3,l4 });
        Node l5 = new Patrouille(gameObject.transform,destination,Speed);
        Node sel1 = new Selector(new List<Node>() { Seq1, l2,l5 });

        root = sel1;


    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("allo");
        root.Evaluate();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter");
        l1.detection = true; 
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
        l1.detection = false;
    }

}
