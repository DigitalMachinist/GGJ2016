using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNodeBehaviour : NodeBehaviour
{

    public CreateNodeBehaviour() : base( NodeBehaviourType.Other, "Create Node", CreateNodeCoroutine ) { }

    static IEnumerator CreateNodeCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        GameManager GM = GameManager.Instance;
        Debug.Log( "Create Node action fired!" );

        action.BeganWarmup.Invoke( action );
        yield return new WaitForSeconds( action.WarmupDelay );
        Debug.Log( "WUT" );

        action.BeganDuration.Invoke( action );
        yield return new WaitForSeconds( action.Duration );
        GM.FinalizePlaceNode( GM.StartPlaceNode( action.Node ) );
        Debug.Log( "WUT WUT WUT" );

        action.BeganCooldown.Invoke( action );
        yield return new WaitForSeconds( action.CooldownDelay );
        Debug.Log( "WUT WUT WUT WUT WUT WUT WUT WUT WUT" );

        action.Ended.Invoke( action );
    }
}
