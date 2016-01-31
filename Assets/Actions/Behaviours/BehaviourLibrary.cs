//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Object = UnityEngine.Object;
//using Random = UnityEngine.Random;

//public static class BehaviourLibrary
//{
//    // CREATE NODE
//    public static NodeBehaviour CreateNodeBehaviour()
//    {
//        return new NodeBehaviour( NodeBehaviourType.Other, "Create New Node", CreateNodeCoroutine );
//    }
//    static IEnumerator CreateNodeCoroutine(
//        NodeAction action,
//        IEnumerable<Node> selectedNodes,
//        bool isSimulated
//    )
//    {
//        yield return null;
//    }

//    // GROW
//    public static NodeBehaviour GrowBehaviour()
//    {
//        return new NodeBehaviour( NodeBehaviourType.Other, "Grow", GrowCoroutine );
//    }
//    static IEnumerator GrowCoroutine(
//        NodeAction action,
//        IEnumerable<Node> selectedNodes,
//        bool isSimulated
//    )
//    {
//        yield return null;
//    }

//}
