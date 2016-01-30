using System.Linq;
using UnityEngine;

public class ColourSelectState : GameState
{
    public ColourSelectState( GameManager gm, GMState type ) : base( gm, type )
    {
        // Do nothing.
    }

    public override void OnEntry()
    {
        Debug.Log( "COLOUR SELECT SCREEN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = false;

        // Show player select screen and move the camera to the special colour select play area.
        GameObject.FindGameObjectWithTag( "ColourSelectScreen" ).SetActive( true );
        var center = new Vector3( -80f, 0f, 47f );
        GM.Bounds = new Bounds( center, new Vector3( 0f, 0f, 0f ) );
        GM.Cursor.transform.position = center;

        // Players 1 and 2 join automatically.
        // This will result in their control context being set by OnPlayerJoined.
        GM
            .Players
            .Where( player => player.Number <= 2 )
            .ToList()
            .ForEach( GM.JoinPlayer );

        // Players 3 and 4 can join the game by pressing A.
        GM
            .Players
            .Where( player => player.Number > 2 )
            .ToList()
            .ForEach( SetControlsNotJoined );

        // Listen for all player join, quit, ready and not ready events.
        GM.PlayerJoined.AddListener( OnPlayerJoined );
        GM.PlayerQuit.AddListener( OnPlayerQuit );
        GM.PlayerJoined.AddListener( OnPlayerReady );
        GM.PlayerQuit.AddListener( OnPlayerNotReady );
    }

    public override void OnExit()
    {
        // Hide player select screen.
        GameObject.FindGameObjectWithTag( "ColourSelectScreen" ).SetActive( true );

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
        GM.PlayerJoined.RemoveAllListeners();
        GM.PlayerQuit.RemoveAllListeners();
    }

    public override void Update()
    {
        // Allow enabled joined players (that aren't yet ready) to rotate their center nodes to 
        // pick colour.
        GM
            .EnabledPlayers
            .Where( player => !player.IsReady )
            .ToList()
            .ForEach( player => {

                Vector3 rAnalogDirection = new Vector3(
                    player.Gamepad.GetAxis( Xbox360GamepadAxis.RAnalogX ),
                    0f,
                    player.Gamepad.GetAxis( Xbox360GamepadAxis.RAnalogY )
                );
                var nodeTransform = player.SelectedNode.transform;
                nodeTransform.LookAt( nodeTransform.position + rAnalogDirection, Vector3.up );
                player.Colour = nodeTransform.GetComponent<ColorSampler>().SampledColor;

            } );
    }

    void OnPlayerJoined( Player player )
    {
        SetControlsJoined( player );

        GM.PlayerQuit.RemoveListener( OnPlayerJoined );
        GM.PlayerQuit.AddListener( OnPlayerQuit );

        // Create this player's node at the center of the play area with a random rotation.
        var position = GM.Bounds.center;
        var rotation = Quaternion.Euler( 0f, Random.Range( 0f, 360f ), 0f );
        var node = GM.InstantiateNode( GM.NodePrefab, player, position, rotation );

        // Parent the node to the nodes container, add it to the node list, and select it. The 
        // nodes will be cleaned up as soon as play starts anyway, so these will disappear.
        var nodesContainer = GameObject.FindGameObjectWithTag( "NodesContainer" );
        node.transform.parent = nodesContainer.transform;
        GM.Nodes.Add( node );
        node.Player.SelectedNode = node;

        // Make sure this node ONLY has the Create Node action. Select that action now so that 
        // it will be the one controller by the owning player.
        node.Actions.Clear();
        var createNodeAction = new CreateNodeAction();
        node.Actions.Add( createNodeAction );
        node.SelectedAction = createNodeAction;
    }

    void OnPlayerQuit( Player player )
    {
        // Node cleanup is handled by GM here. No need to do anything special!

        SetControlsNotJoined( player );

        GM.PlayerJoined.AddListener( OnPlayerJoined );
        GM.PlayerQuit.RemoveListener( OnPlayerQuit );
    }

    void OnPlayerReady( Player player )
    {
        SetControlsReady( player );

        GM.PlayerReady.RemoveListener( OnPlayerReady );
        GM.PlayerNotReady.AddListener( OnPlayerNotReady );
    }

    void OnPlayerNotReady( Player player )
    {
        SetControlsJoined( player );

        GM.PlayerReady.AddListener( OnPlayerReady );
        GM.PlayerNotReady.RemoveListener( OnPlayerNotReady );
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

        // Pressing B indicates that the player is no longer ready and wants to choose colour again.
        player.Gamepad.BButton.Pressed.RemoveAllListeners();
        player.Gamepad.BButton.Pressed.AddListener( () => GM.NotReadyPlayer( player ) );
    }
}
