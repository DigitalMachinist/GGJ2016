using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class OrdersState : GMState
{
    Player controllingPlayer;

    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        
        Debug.Log( "STOPPED FOR ORDERS" );

        //Time.timeScale = 0f;
        GM.Cursor.enabled = true;

        // Listen for the completion of all pending nodes' actions.
        //GM.ContinuePlaying.AddListener( () => GM.ChangeState( GMState.Playing ) );

        // Listen for any further nodes that might need to take their turns now.
        GM.PlayerTurnBegan.AddListener( player => {
            SetNoControl( controllingPlayer );
            SetInControl( player );
        } );

        // Begin actioning the first node that is about to take its turn.
        GM.NextPendingNode();

        // Give control to the player whose node is acting.
        SetInControl( GM.PlayerTurn );
    }

    public override void OnControlEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlEnter( animator, stateInfo, layerIndex );
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateUpdate( animator, stateInfo, layerIndex );

        // 
        GM
            .ReadyPlayers
            .ToList()
            .ForEach( player => {

                if ( player.SelectedNode == null )
                {
                    return;
                }

                var x = player.Gamepad.GetAxis( Xbox360GamepadAxis.LAnalogX );
                var y = player.Gamepad.GetAxis( Xbox360GamepadAxis.LAnalogY );
                Vector3 direction = new Vector3( x, 0f, y );
                if ( direction == Vector3.zero )
                {
                    return;
                }

                // Sample the colour of the node in placement state around the player's central node.
                var nodeTransform = player.SelectedNode.transform;
                nodeTransform.LookAt( nodeTransform.position + direction, Vector3.up );

            } );
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

        // Revoke control from the player whose node is acting.
        SetNoControl( GM.PlayerTurn );

        GM.ContinuePlaying.RemoveAllListeners();
        GM.RequiresAction.RemoveAllListeners();
    }

    void SetInControl( Player player )
    {
        if ( player == null )
        {
            return;
        }

        controllingPlayer = player;

        // Pressing X runs the Create Node action.
        player.Gamepad.XButton.Pressed.RemoveAllListeners();
        player.Gamepad.XButton.Pressed.AddListener( () => {
            player.SelectedNode.ExecuteAction( "Create Node" );
        } );

        // Pressing Y runs the Grow action.
        player.Gamepad.YButton.Pressed.RemoveAllListeners();
        player.Gamepad.YButton.Pressed.AddListener( () => {
            player.SelectedNode.ExecuteAction( "Grow" );
        } );
    }

    void SetNoControl( Player player )
    {
        if ( player == null )
        {
            return;
        }

        player.Gamepad.XButton.Pressed.RemoveAllListeners();
        player.Gamepad.YButton.Pressed.RemoveAllListeners();
    }
}
