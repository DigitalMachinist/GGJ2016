using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PlayingState : GMState
{
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        Debug.Log( "TIME IS PROGRESSING" );

        Time.timeScale = GameManager.Instance.GameSpeed;
        GM.Cursor.enabled = true;

        //GM.RequiresAction.AddListener( () => GM.ChangeState( GMState.Action ) );
    }

    public override void OnControlEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlEnter( animator, stateInfo, layerIndex );
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateUpdate( animator, stateInfo, layerIndex );

        if ( GM.HasPendingNodes )
        {
            GM.RequiresAction.Invoke();
        }
    }

    public override void OnControlUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlUpdate( animator, stateInfo, layerIndex );
    }

    public override void OnControlExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlExit( animator, stateInfo, layerIndex );
    }

    public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateExit( animator, stateInfo, layerIndex );

        //GM.UnpauseState = Type;

        GM.RequiresAction.RemoveAllListeners();
    }
}
