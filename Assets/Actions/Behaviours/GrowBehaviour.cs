using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowBehaviour : NodeBehaviour
{
    public GrowBehaviour() 
        : base( NodeBehaviourType.Other, "Grow", GrowCoroutine ) { }

    static IEnumerator GrowCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        Debug.Log( "Grow action fired!" );
        yield return null;
    }

    public override string ToString()
    {
        return "Increase max HP, regen rate, energy rate, and gain access to another action.";
    }
}
