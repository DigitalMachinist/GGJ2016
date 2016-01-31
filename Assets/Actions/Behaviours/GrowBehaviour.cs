using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowBehaviour : NodeBehaviour
{
    public GrowBehaviour() : base( NodeBehaviourType.Other, "Grow", GrowCoroutine ) { }

    static IEnumerator GrowCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        Debug.Log( "Grow action fired!" );

        action.BeganWarmup.Invoke( action );
        yield return new WaitForSeconds( action.WarmupDelay );

        action.BeganDuration.Invoke( action );
        yield return new WaitForSeconds( action.Duration );

        action.Node.Grow();

        action.BeganCooldown.Invoke( action );
        yield return new WaitForSeconds( action.CooldownDelay );

        action.Ended.Invoke( action );
    }
}
