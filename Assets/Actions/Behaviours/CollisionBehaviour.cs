using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollisionBehaviourType
{
    Impact,
    Deflect
}

public class CollisionBehaviour : ActionBehaviour
{
    public CollisionBehaviourType Type;

    // Args:
    // 1) The action itself.
    // 2) The physics collision object.
    // 3) Whether or not this is a simulation.
    // Returns: IEnumerator
    public Func<NodeAction, Collision, bool, IEnumerator> Behaviour;

    public CollisionBehaviour(
        CollisionBehaviourType type,
        string descriptor,
        Func<NodeAction, Collision, bool, IEnumerator> behaviour = null
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
