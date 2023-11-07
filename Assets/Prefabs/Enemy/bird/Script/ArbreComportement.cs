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

   
    public Detect() : base()
    {
    
    


    }


    public override NodeState Evaluate()
    {

        object detect = GetData("detect");
        State = NodeState.Failure;
        //if (detect.)
        //{
        //    State = NodeState.Success;
        //}

        return State;
    }
    


}
public class Retour : Node
{


    public Retour() : base()
    {




    }


    public override NodeState Evaluate()
    {


        return State;
    }



}
public class AlertPhase : Node
{


    public AlertPhase() : base()
    {




    }


    public override NodeState Evaluate()
    {


        return State;
    }



}

public class Poursuite : Node
{

   


    public Poursuite() : base()
    {
    
    }

    public override NodeState Evaluate()
    {


        return State;
    }


   
}
public class Patrouille : Node
{
    List<Transform> ListeTransform;
    int destinationIndex = 0;
    NavMeshAgent agent;
    float waittime = 0;
    float tempsEncour = 0;
    bool isWaiting = false;

    public Patrouille() : base()
    {



    }
    public override NodeState Evaluate()
    {

        State = NodeState.Running;
        if (isWaiting)
        {
            tempsEncour += Time.deltaTime;
            if (tempsEncour > waittime)
            {
                tempsEncour = 0;
                isWaiting = false;
                destinationIndex = (destinationIndex + 1) % ListeTransform.Count;
            }

        }
        else
        {
            if (!agent.SetDestination(ListeTransform[destinationIndex].position))
            {
                State = NodeState.Failure;
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                isWaiting = true;
            }


        }

        return State;
    }

}

