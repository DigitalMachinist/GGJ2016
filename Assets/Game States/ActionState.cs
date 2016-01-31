using System.Linq;
using UnityEngine;

public class ActionState : GameState
{
    Player controllingPlayer;

    public ActionState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "STOPPED FOR PLAYER TURN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = true;

        // Listen for the completion of all pending nodes' actions.
        GM.ContinuePlaying.AddListener( () => GM.ChangeState( GMState.Playing ) );

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

    public override void OnExit()
    {
        GM.UnpauseState = Type;

        // Revoke control from the player whose node is acting.
        SetNoControl( GM.PlayerTurn );

        GM.ContinuePlaying.RemoveAllListeners();
        GM.RequiresAction.RemoveAllListeners();
    }

    public override void Update()
    {
        // 
        GM
            .ReadyPlayers
            .ToList()
            .ForEach( player => {

                if ( player.SelectedNode == null )
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
                    return;
                }

                // Sample the colour of the node in placement state around the player's central node.
                var nodeTransform = player.SelectedNode.transform;
                nodeTransform.LookAt( nodeTransform.position + direction, Vector3.up );

            } );
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
