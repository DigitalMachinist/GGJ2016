using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeBehaviourType
{
    Aiming,
    Shooting,
    Support,
    Other
}

public class NodeBehaviour : ActionBehaviour
{
    public NodeBehaviourType Type;

    // Args:
    // 1) The action itself.
    // 2) A list of selected nodes.
    // 3) The duration of the action (including delays and duration).
    // 4) Whether or not this is a simulation.
    // Returns: IEnumerator
    public Func<NodeAction, IEnumerable<Node>, bool, IEnumerator> Behaviour;

    public NodeBehaviour( 
        NodeBehaviourType type, 
        string descriptor, 
        Func<NodeAction, IEnumerable<Node>, bool, IEnumerator> behaviour = null
    )
    {
        GetsFinalWord = false;
        UsesSelection = false;
        UsesProjectiles = false;

        Type = type;
        Descriptor = descriptor;
        Behaviour = behaviour;
    }
}
