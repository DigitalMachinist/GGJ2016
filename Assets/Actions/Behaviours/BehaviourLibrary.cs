using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class BehaviourLibrary
{
    // CREATE NODE
    public static NodeBehaviour CreateNodeBehaviour()
    {
        return new NodeBehaviour( NodeBehaviourType.Other, "Create New Node", BehaviourLibrary.CreateNodeCoroutine );
    }
    static IEnumerator CreateNodeCoroutine(
        NodeAction action,
        IEnumerable<Node> selectedNodes,
        float duration,
        bool isSimulated
    )
    {
        yield return null;
    }


}
