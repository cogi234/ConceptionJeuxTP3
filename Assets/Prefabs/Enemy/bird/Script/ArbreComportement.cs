using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.AI;
    
using static Unity.VisualScripting.Metadata;

public enum NodeState { Running, Success, Failure }


public abstract class Node
{

    Dictionary<String, object> data = new Dictionary<String, object>();

    public void SetData(string key, object value)
    {
        data[key] = value;
    }
    public object GetData(string key)
    {
        if (data.TryGetValue(key, out object value)) return value;

        if (parent != null)
        {
            return parent.GetData(key);

        }
        return null;
    }


    protected List<Node> children = new();
    protected NodeState State;
    public Node parent { get; set; }
    public Node()
    {
        parent = null;
        State = NodeState.Running;
    }
    public Node(List<Node> pChildren)
    {
        parent = null;
        State = NodeState.Running;
        foreach (Node n in pChildren)
        {
            Attach(n);
        }
    }
    protected void Attach(Node n)
    {
        children.Add(n);
        n.parent = this;
    }
    public abstract NodeState Evaluate();
    public bool RemoveData(string key)
    {

        if (data.Remove(key)) { return true; }
        if (parent != null)
        {
            return parent.RemoveData(key);
        }
        return false;


    }
}


//public class Patrol : Node { }


public class Sequence : Node
{
    public Sequence(List<Node> n) : base(n)
    {

    }
    public override NodeState Evaluate()
    {
        Debug.Log("passe sequence");

        foreach (Node n in children)
        {
            State = n.Evaluate();
            if (State != NodeState.Success)
            {

                return State;
            }
           

        }
        State = NodeState.Success;
        return NodeState.Success;


    }



}





public class Selector : Node
{
    public Selector(List<Node> n) : base(n) { }
    public override NodeState Evaluate()
    {
        Debug.Log("passe selector");
        foreach (Node n in children)
        {
            State = n.Evaluate();
            if (State != NodeState.Failure)
            {

                return State;
            }


        }
        State = NodeState.Failure;
        return NodeState.Failure;
    }


}




public class Inverter : Node
{
    public Inverter(List<Node> n) : base(n)
    {

        if (n.Count != 1)
        {
            throw new ArgumentException();
        }
    }
    public override NodeState Evaluate()
    {
        Debug.Log("passe invert");
        NodeState childState = children[0].Evaluate();

        if (childState == NodeState.Failure)
        {
            State = NodeState.Success;
        }

        else if (childState == NodeState.Success)
        {
            State = NodeState.Failure;
        }

        else
        {
            State = NodeState.Running;
        }

        return State;
    }


}



public class Detect : Node
{
    bool aUpdatePosition = false;
  public  bool detection = false;
    Node root;
    Transform birdPosition;
    public Detect() : base()
    {
        

       


    }


    public override NodeState Evaluate()
    {
        root = parent;

        while (root.parent != null)
        {

            root = root.parent;

        }
        Debug.Log(detection);
        Debug.Log("passe détect");
        bool DéjaDétect = false;
     
        State = NodeState.Failure;
       
        if (detection)
        {
            if (DéjaDétect)
            {


                root.SetData("detet", true);
                if (aUpdatePosition)
                {
                    root.SetData("position", birdPosition);
                    aUpdatePosition = !aUpdatePosition;
                    
                }

                State = NodeState.Success;
                DéjaDétect = !DéjaDétect;

            }
        }
        else
        {
            root.SetData("detet", false);
            root.SetData("cri", false);
            DéjaDétect = !DéjaDétect;
        }
        

        return State;
    }
    


}
public class Retour : Node
{

    Transform positionPoursuite;
    GameObject ennemie;
    float speed;
    bool estEnTrainDeRetour = false;
   bool adetect =false;
    Node root;
    bool instancier= true;
    public Retour( GameObject ennemies, float speeds) : base()
    {
       // positionPoursuite = positionP;
        ennemie = ennemies;
        speed = speeds;
         

    }

 
    public override NodeState Evaluate()
    {
       

        root = parent;

        while (root.parent != null)
        {

            root = root.parent;

        }
        if (instancier)
        {
            root.SetData("detect", false);
            instancier = false;
        }
        Debug.Log("passe Retour");
        State = NodeState.Failure;
        bool detect = (bool)root.GetData("detect");
        if (detect)
        {
            adetect = true;
            

        }
        else if( adetect && !estEnTrainDeRetour) {
            estEnTrainDeRetour = !estEnTrainDeRetour;
            root.SetData("retour", estEnTrainDeRetour);
          
        }


        if(estEnTrainDeRetour)
        {



            positionPoursuite  = (Transform)root.GetData("position");
         
          
                ennemie.transform.Translate(Vector3.Normalize(positionPoursuite.position - ennemie.transform.position) * speed);
                if(Vector3.Distance(ennemie.transform.position, positionPoursuite.position) <= 1)
                {
                    estEnTrainDeRetour = !estEnTrainDeRetour;
                    root.SetData("retour", estEnTrainDeRetour);
                  State = NodeState.Success;
            }
                 State = NodeState.Running;
            
        }
        return State;
    }



}
public class AlertPhase : Node
{
    float tempsPhase  = 5;
    float compteur = 0;
    Node root;
    bool cri;
    AudioSource audio;
    public AlertPhase(AudioSource source) : base()
    {
        
       
        audio = source;


    }


    public override NodeState Evaluate()
    {
        Debug.Log("passe alerte");
        root = parent;

        while (root.parent != null)
        {

            root = root.parent;

        }
        State = NodeState.Running;
      cri= (Boolean)root.GetData("cri");
       
  
        if (!cri)
        {
            compteur = 0;
            audio.Play();
            root.SetData("cri", true);
            compteur += Time.deltaTime;
        }
       else
        {
            compteur += Time.deltaTime;
            if (compteur > tempsPhase)
            {
                State = NodeState.Success;
              
            }
        }
       
       

        return State;
    }



}

public class Poursuite : Node
{
    Transform joueur;
    Transform ennemie;
    float speed;
    //


    public Poursuite(Transform joueur, Transform ennemie,float speed) : base()
    {
        this.joueur = joueur;
        this.ennemie = ennemie;
        this.speed = speed;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("passe poursuite");
        State = NodeState.Running;
        ennemie.transform.Translate(Vector3.Normalize(joueur.position - ennemie.transform.position) * speed);


        return State;
    }


   
}
public class Patrouille : Node
{
    List<Transform> ListeTransform;
    Transform ennemie;
    int destinationIndex = 0;
    Transform destination;
    
    float speed = 3;

    public Patrouille(Transform ennemie, List<Transform> destination,float speed) : base()
    {
        ListeTransform = destination;
        this.ennemie = ennemie;
        this.speed = speed;
    }
    public override NodeState Evaluate()
    {
        Debug.Log("passe Patrouille");
    State= NodeState.Running;
        destination = ListeTransform[destinationIndex];

        ennemie.transform.Translate(Vector3.Normalize(destination.position - ennemie.transform.position) * speed);
        if (Vector3.Distance(ennemie.position, destination.position) <= 10)
        {
            destinationIndex++;
            if (destinationIndex == ListeTransform.Count)
            {
                destinationIndex = 0;
            }
        }





        return State;
    }

}

