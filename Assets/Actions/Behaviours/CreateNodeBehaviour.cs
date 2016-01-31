using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeBehaviour : NodeBehaviour
{
    public CreateNodeBehaviour() 
        : base( NodeBehaviourType.Other, "Create Node", CreateNodeCoroutine ) { }

    static IEnumerator CreateNodeCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        Debug.Log( "Create Node action fired!" );
        yield return null;
    }

    public override string ToString()
    {
        return "Create a new node at a fixed distance this one.";
    }
}
