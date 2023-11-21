using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            return parent.GetData(key);

        return null;
    }

    protected List<Node> children = new();
    protected NodeState State;
    public Node parent;

    protected Node GetRoot()
    {
        Node n = parent;
        while (n.parent != null)
            n = n.parent;

        return n;
    }
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
            return parent.RemoveData(key);
        return false;
    }
}

public class Sequence : Node
{
    public Sequence(List<Node> n) : base(n) { }
    public override NodeState Evaluate()
    {
        foreach (Node n in children)
        {
            State = n.Evaluate();
            if (State != NodeState.Success)
                return State;
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
                return State;
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
            State = NodeState.Success;
        else if (childState == NodeState.Success)
            State = NodeState.Failure;
        else
            State = NodeState.Running;

        return State;
    }
}

public class Detect : Node
{
    bool aUpdatePosition = true;
    bool alreadyDetected = false;
    public bool detection = false;
    float initialHeight;
    Transform bird;

    public Detect(Transform birds) : base()
    {
        bird = birds;
        initialHeight = birds.position.y;
    }

    public override NodeState Evaluate()
    {
        State = NodeState.Failure;
        Node root = GetRoot();

        if (detection && initialHeight - bird.position.y < 30)
        {
            if (!alreadyDetected)
            {
                root.SetData("detect", true);

                if (aUpdatePosition)
                {
                    root.SetData("position", bird.position);
                    aUpdatePosition = !aUpdatePosition;
                }

                alreadyDetected = !alreadyDetected;
            }
            State = NodeState.Success;
        }
        else
        {
            root.SetData("detect", false);
            root.SetData("cri", false);
            if (alreadyDetected)
            {
                alreadyDetected = !alreadyDetected;
                aUpdatePosition = !aUpdatePosition;
            }
        }

        return State;
    }
}

public class Return : Node
{
    Vector3 targetPosition;
    Transform enemy;
    float speed;
    bool isReturning = false;
    bool hasDetected = false;
    Node root;
    bool instancier = true;
    bool detect;
    public Return(Transform enemy, float speeds) : base()
    {
        this.enemy = enemy;
        speed = speeds;
    }

    public override NodeState Evaluate()
    {
        root = GetRoot();

        if (instancier)
        {
            root.SetData("detect", false);
            instancier = false;

        }

        State = NodeState.Failure;
        detect = (bool)root.GetData("detect");

        if (detect)
        {
            hasDetected = true;
        }
        else
        {
            if (hasDetected && !isReturning)
            {
                hasDetected = !hasDetected;
                isReturning = !isReturning;
                root.SetData("retour", isReturning);
            }
        }

        if (isReturning)
        {
            targetPosition = (Vector3)root.GetData("position");
            enemy.LookAt(targetPosition, Vector3.up);
            enemy.Translate(Vector3.Normalize(targetPosition - enemy.position) * speed * Time.deltaTime, Space.World);
            if (Vector3.Distance(enemy.position, targetPosition) <= 1)
            {
                Debug.Log("re");
                isReturning = !isReturning;
                root.SetData("retour", isReturning);
                State = NodeState.Success;
            }
            State = NodeState.Running;
        }

        return State;
    }
}

public class AlertPhase : Node
{
    float phaseDuration = 5;
    float timer = 0;
    Node root;
    bool scream;
    AudioSource audio;
    public AlertPhase(AudioSource source) : base()
    {
        scream = true;
        audio = source;
    }

    public override NodeState Evaluate()
    {
        root = GetRoot();
        State = NodeState.Running;
        scream = (Boolean)root.GetData("cri");

        if (!scream)
        {
            timer = 0;
            audio.Play();
            root.SetData("cri", true);
            timer += Time.deltaTime;
        }
        else
        {
            timer += Time.deltaTime;
            if (timer > phaseDuration)
                State = NodeState.Success;
        }
        return State;
    }
}

public class Pursuit : Node
{
    Transform target;
    Transform me;
    private Vector3 direction;
    float speed;

    public Pursuit(Transform target, Transform me, float speed) : base()
    {
        this.target = target;
        this.me = me;
        this.speed = speed;
    }

    public override NodeState Evaluate()
    {
        State = NodeState.Running;

        //_direction = (target.position - me.position).normalized;

        //me.rotation = Quaternion.FromToRotation(me.forward, direction) * me.rotation;

        //me.rotation.SetLookRotation(direction, new Vector3(0, 1, 0));
        me.Translate(Vector3.Normalize(target.position - me.position) * speed * Time.deltaTime, Space.World);
        me.LookAt(target, Vector3.up);

        //me.transform.Translate(Vector3.forward * speed * Time.deltaTime);

        return State;
    }
}

public class Patrol : Node
{
    List<Transform> waypoints;
    Transform me;
    int destinationIndex = 0;
    Transform destination;
    private Vector3 direction;
    float speed = 3;

    public Patrol(Transform me, List<Transform> waypoints, float speed) : base()
    {
        this.waypoints = waypoints;
        this.me = me;
        this.speed = speed;
    }
    public override NodeState Evaluate()
    {
        State = NodeState.Running;
        destination = waypoints[destinationIndex];
        //direction = (destination.position - me.position).normalized;

        //me.rotation = Quaternion.FromToRotation(me.forward, direction) * me.rotation;

        me.transform.Translate(Vector3.Normalize(destination.position - me.position) * speed * Time.deltaTime, Space.World);
        me.LookAt(destination, Vector3.up);

        if (Vector3.Distance(me.position, destination.position) <= 10)
            destinationIndex = (destinationIndex + 1) % waypoints.Count;

        return State;
    }
}