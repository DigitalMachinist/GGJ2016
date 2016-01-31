using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamageBehaviour : CollisionBehaviour
{
    public CollisionDamageBehaviour() : base( CollisionBehaviourType.Impact, "", CollisionDamageCoroutine )
    {
        
    }

    static IEnumerator CollisionDamageCoroutine( NodeAction action, Collision collision, bool isSimulated )
    {
        GameManager GM = GameManager.Instance;

        var health = collision.transform.GetComponent<Health>();
        if ( health != null )
        {
            health.ApplyDamage( 10 );
        }

        action.Ended.Invoke( action );

        // Just to satisfy the damn coroutine.
        yield return null;
    }
}
