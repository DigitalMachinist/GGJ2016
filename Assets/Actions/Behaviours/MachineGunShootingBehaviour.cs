using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class MachineGunShootingBehaviour : NodeBehaviour
{
    public MachineGunShootingBehaviour() : base( NodeBehaviourType.Shooting, "Machine Gun", MachineGunShootingCoroutine )
    {
        GetsFinalWord = true;
    }

    static IEnumerator MachineGunShootingCoroutine( NodeAction action, IEnumerable<Node> selectedNodes, bool isSimulated )
    {
        GameManager GM = GameManager.Instance;
        Transform visuals = action.transform.FindChild( "Visuals" );

        action.BeganWarmup.Invoke( action );
        yield return new WaitForSeconds( action.WarmupDelay );

        action.BeganDuration.Invoke( action );
        var timeShot = 0.075f;
        var numIterations = Mathf.FloorToInt( action.Duration / timeShot );
        for ( var i = 0; i < numIterations; i++ )
        {
            var projectile = (Projectile)Object.Instantiate( GM.BulletPrefab );
            projectile.transform.parent = action.transform;
            projectile.transform.localPosition = Vector3.zero;
            projectile.transform.localRotation = Quaternion.identity;
            projectile.transform.parent = null;
            projectile.GetComponent<Rigidbody>().velocity = 10f * Vector3.forward;
        }

        action.BeganCooldown.Invoke( action );
        yield return new WaitForSeconds( action.CooldownDelay );

        action.Ended.Invoke( action );
    }
}
