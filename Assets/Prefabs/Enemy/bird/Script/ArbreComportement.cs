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
    bool aUpdatePosition = true;
    public bool detection = false;
    float y;
    Transform bird;
    Node root;
    Vector3 birdPosition;
    public Detect(Transform birds) : base()
    {
        bird = birds;
       // y = birds.position.y;




    }

    bool DéjaDétect = false;
    public override NodeState Evaluate()
    {
        State = NodeState.Failure;
        root = parent;

        while (root.parent != null)
        {

            root = root.parent;

        }






        if (detection && y- bird.position.y <30)
        {

            if (!DéjaDétect)
            {

                birdPosition = new Vector3(bird.transform.position.x, bird.transform.position.y, bird.transform.position.z);
                root.SetData("detect", true);
            
                if (aUpdatePosition)
                {
                   
                    root.SetData("position", birdPosition);
                    aUpdatePosition = !aUpdatePosition;

                }


                DéjaDétect = !DéjaDétect;

            }
            State = NodeState.Success;
        }
        else
        {
            root.SetData("detect", false);
            root.SetData("cri", false);
            if (DéjaDétect)
            {
                DéjaDétect = !DéjaDétect;
                aUpdatePosition = !aUpdatePosition;
            }
        }

       
        return State;
    }



}
public class Retour : Node
{

    Vector3 positionPoursuite;
    GameObject ennemie;
    float speed;
    bool estEnTrainDeRetour = false;
   bool adetect =false;
    Node root;
    bool instancier= true;
    bool detect;
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
       
        State = NodeState.Failure;
         detect = (bool)root.GetData("detect");

        if (detect)
        {
            adetect = true;
          

        }
        else  {
         
          
            if (adetect && !estEnTrainDeRetour)
            {
                adetect = !adetect;
                estEnTrainDeRetour = !estEnTrainDeRetour;
                root.SetData("retour", estEnTrainDeRetour);
                
            }
               

        }
      


     
        if (estEnTrainDeRetour)
        {
            


            positionPoursuite  = (Vector3)root.GetData("position");
          

            ennemie.transform.Translate(Vector3.Normalize(positionPoursuite - ennemie.transform.position) * speed * Time.deltaTime);
                if(Vector3.Distance(ennemie.transform.position, positionPoursuite) <= 1)
                {
                Debug.Log("re");
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
    private Vector3 _direction;
    float speed;
    


    public Poursuite(Transform joueur, Transform ennemie,float speed) : base()
    {
        this.joueur = joueur;
        this.ennemie = ennemie;
        this.speed = speed;
    }

    public override NodeState Evaluate()
    {
        _direction = (joueur.position - ennemie.position).normalized;

        Vector3 gravityUp = -_direction.normalized;
        ennemie.rotation = Quaternion.FromToRotation(-ennemie.right, gravityUp) * ennemie.rotation;

       




        State = NodeState.Running;
        ennemie.transform.Translate(Vector3.right * speed * Time.deltaTime);


        return State;
    }


   
}
public class Patrouille : Node
{
    List<Transform> ListeTransform;
    Transform ennemie;
    int destinationIndex = 0;
    Transform destination;

   

    private Vector3 _direction;
    float speed = 3;

    public Patrouille(Transform ennemie, List<Transform> destination,float speed) : base()
    {
        ListeTransform = destination;
        this.ennemie = ennemie;
        this.speed = speed;
    }
    public override NodeState Evaluate()
    {
        
    State= NodeState.Running;
        destination = ListeTransform[destinationIndex];


        _direction = (destination.position - ennemie.position).normalized;

        Vector3 gravityUp = -_direction.normalized;
        ennemie.rotation = Quaternion.FromToRotation(-ennemie.right, gravityUp) * ennemie.rotation;


        //ennemie.position += -Vector3.right * Time.deltaTime;



        ennemie.transform.Translate(Vector3.right * speed*Time.deltaTime);
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

