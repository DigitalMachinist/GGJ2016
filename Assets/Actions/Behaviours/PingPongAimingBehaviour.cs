using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongAimingBehaviour : NodeBehaviour
{
    public PingPongAimingBehaviour() : base( NodeBehaviourType.Shooting, "Hosing", PingPongAimingCoroutine ) { }

    static IEnumerator PingPongAimingCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        GameManager GM = GameManager.Instance;
        Transform visuals = action.transform.FindChild( "Visuals" );

        action.BeganWarmup.Invoke( action );
        yield return new WaitForSeconds( action.WarmupDelay );

        action.BeganDuration.Invoke( action );
        var time = 0f;
        while ( time < action.Duration )
        {
            var angle = action.Angle * ( Mathf.PingPong( time, 0.5f ) - 0.25f );
            visuals.transform.localRotation = Quaternion.Euler( 0f, angle, 0f );
            time += Time.deltaTime;
        }
        visuals.transform.localRotation = Quaternion.identity;

        action.BeganCooldown.Invoke( action );
        yield return new WaitForSeconds( action.CooldownDelay );

        action.Ended.Invoke( action );
    }
}
