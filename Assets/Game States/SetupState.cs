using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class SetupState : GMState
{
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        Debug.Log( "COLOUR SELECT SCREEN" );
        Debug.Log( stateInfo );
        Debug.Log( animator.GetNextAnimatorStateInfo( layerIndex ) );

        Time.timeScale = 0f;
        GM.Cursor.enabled = false;

        // TODO Show colour select UI canvas

        // Change the gameplay area's background
        Object.FindObjectOfType<GameplayBackground>().RandomMaterial();

        // Show the colour select UI and move the camera to the colour select area.
        GameObject.FindGameObjectWithTag( "ColourSelectScreen" ).SetActive( true );
        var center = new Vector3( -80f, 0f, 45f );
        GM.Bounds = new Bounds( center, new Vector3( 0f, 0f, 0f ) );
        GM.Cursor.transform.position = center;

        // Listen for all player join, quit, ready and not ready events.
        // Note: This needs to come before auto-joining players 1 and 2.
        GM.PlayerJoined.AddListener( OnPlayerJoined );
        GM.PlayerQuit.AddListener( OnPlayerQuit );
        GM.PlayerReady.AddListener( OnPlayerReady );
        GM.PlayerNotReady.AddListener( OnPlayerNotReady );

        // Players 1 and 2 join automatically.
        // This will result in their control context being set by OnPlayerJoined.
        GM
            .Players
            .Where( player => player.Number <= 2 )
            .ToList()
            .ForEach( player => {
                SetControlsJoined( player );
                GM.JoinPlayer( player );
            } );

        // Players 3 and 4 can join the game by pressing A.
        GM
            .Players
            .Where( player => player.Number > 2 )
            .ToList()
            .ForEach( player => {
                SetControlsNotJoined( player );
            } );
    }

    public override void OnControlEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlEnter( animator, stateInfo, layerIndex );
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateUpdate( animator, stateInfo, layerIndex );

        // Allow enabled joined players (that aren't yet ready) to rotate their center nodes to 
        // pick colour.
        GM
            .EnabledPlayers
            .Where( player => !player.IsReady )
            .ToList()
            .ForEach( player => {

                if ( player.SelectedNode == null || player.SamplingNode == null )
                {
                    return;
                }

                Vector3 direction = new Vector3(
                    player.Gamepad.GetAxis( Xbox360GamepadAxis.LAnalogX ),
                    0f,
                    player.Gamepad.GetAxis( Xbox360GamepadAxis.LAnalogY )
                );
                if ( direction == Vector3.zero )
                {
                    // If the player doesn't pick a colour, randomize their direction input.
                    //direction = new Vector3( Random.Range( -1f, 1f ), 0f, Random.Range( -1f, 1f ) ).normalized;
                    direction = Vector3.forward;
                }

                // Sample the colour of the node in placement state around the player's central node.
                var nodeTransform = player.SelectedNode.transform;
                nodeTransform.LookAt( nodeTransform.position + direction, Vector3.up );

                // Sample the colour wheel and use that to set the player's colour.
                var samplingNode = player.SamplingNode;
                player.Colour = samplingNode.GetComponent<ColorSampler>().SampledColor;
                samplingNode.UpdateMaterialColour();

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

        // TODO Hide colour select UI canvas

        // Hide the colour select UI.
        GameObject.FindGameObjectWithTag( "ColourSelectScreen" ).SetActive( false );

        // Show the gameplay UI and move the camera to the gameplay area.
        GameObject.FindGameObjectWithTag( "GameplayScreen" ).SetActive( true );
        var center = new Vector3( 80f, 0f, 45f );
        GM.Bounds = new Bounds( center, new Vector3( 106f, 0f, 60f ) );
        GM.Cursor.transform.position = center;

        // Prepare the gameplay area for the battle!
        GM.PrepareGameplayArea();

        // Clear all A, B and Start button controls for all players.
        GM
            .Players
            .ForEach( player => {
                player.Gamepad.AButton.Pressed.RemoveAllListeners();
                player.Gamepad.BButton.Pressed.RemoveAllListeners();
                player.Gamepad.StartButton.Pressed.RemoveAllListeners();
            } );

        // Stop listening for all player events.
        GM.PlayerJoined.RemoveAllListeners();
        GM.PlayerQuit.RemoveAllListeners();
        GM.PlayerReady.RemoveAllListeners();
        GM.PlayerNotReady.RemoveAllListeners();
    }

    void OnPlayerJoined( Player player )
    {
        SetControlsJoined( player );

        // Create this player's node at the center of the play area with a random rotation.
        var position = GM.Bounds.center;
        var rotation = Quaternion.Euler( 0f, Random.Range( 0f, 360f ), 0f );
        var centralNode = GM.InstantiateNode( GM.NodePrefab, player, position, rotation );

        // Parent the node to the nodes container, add it to the node list, and select it. The 
        // nodes will be cleaned up as soon as play starts anyway, so these will disappear.
        var nodesContainer = GameObject.FindGameObjectWithTag( "NodesContainer" );
        centralNode.transform.parent = nodesContainer.transform;
        centralNode.Player.SetSelectedNode( centralNode );
        GM.Nodes.Add( centralNode );

        // Begin the process of creating a node that will sample the colour wheel.
        player.SamplingNode = GM.StartPlaceNode( centralNode );
    }

    void OnPlayerQuit( Player player )
    {
        // Node cleanup is handled by GM here. No need to do anything special!

        SetControlsNotJoined( player );
    }

    void OnPlayerReady( Player player )
    {
        SetControlsReady( player );

        // Place 
        var nodeTransform = player.SamplingNode.transform;
        var mask = Object.Instantiate( GM.ColourWheelMask );
        mask.transform.parent = nodeTransform;
        mask.transform.localPosition = Vector3.zero;
        mask.transform.localRotation = Quaternion.identity;
    }

    void OnPlayerNotReady( Player player )
    {
        SetControlsJoined( player );
    }

    void SetControlsNotJoined( Player player )
    {
        // Allow player to rejoin by pressing A, as before.
        player.Gamepad.AButton.Pressed.RemoveAllListeners();
        player.Gamepad.AButton.Pressed.AddListener( () => GM.JoinPlayer( player ) );

        // Pressing B does nothing.
        player.Gamepad.BButton.Pressed.RemoveAllListeners();
    }

    void SetControlsJoined( Player player )
    {
        // Pressing A indicates this player is ready to play.
        player.Gamepad.AButton.Pressed.RemoveAllListeners();
        player.Gamepad.AButton.Pressed.AddListener( () => GM.ReadyPlayer( player ) );

        // Pressing B now quits (but not if they are Players 1 or 2).
        player.Gamepad.BButton.Pressed.RemoveAllListeners();
        if ( player.Number > 2 )
        {
            player.Gamepad.BButton.Pressed.AddListener( () => GM.QuitPlayer( player ) );
        }
    }

    void SetControlsReady( Player player )
    {
        // Pressing A does nothing.
        player.Gamepad.AButton.Pressed.RemoveAllListeners();
        if ( player == GM.Player1 )
        {
            player.Gamepad.AButton.Pressed.AddListener( GM.ChangeToPlaying );
        }

        // Pressing B indicates that the player is no longer ready and wants to choose colour again.
        player.Gamepad.BButton.Pressed.RemoveAllListeners();
        player.Gamepad.BButton.Pressed.AddListener( () => GM.NotReadyPlayer( player ) );
    }
}
