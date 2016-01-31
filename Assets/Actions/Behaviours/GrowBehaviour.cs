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
        Debug.Log( "WAT" );

        action.BeganDuration.Invoke( action );
        yield return new WaitForSeconds( action.Duration );
        action.Node.Grow();
        Debug.Log( "WAT WAT WAT" );

        action.BeganCooldown.Invoke( action );
        yield return new WaitForSeconds( action.CooldownDelay );
        Debug.Log( "WAT WAT WAT WAT WAT WAT WAT WAT WAT" );

        action.Ended.Invoke( action );
    }
}
