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

        // Player 1 joins automatically.
        GM.Join( GM.Player1 );

        // Press A to join as another player.
        GM
            .Players
            .Where( player => player.Number > 1 )
            .ToList()
            .ForEach( player => {
                player.Gamepad.AButton.Pressed.AddListener( () => GM.Join( player ) );
            } );
    }

    public override void OnExit()
    {
        // Hide player select screen.
        GameObject.FindGameObjectWithTag( "ColourSelectScreen" ).SetActive( true );

        // Stop Player 1 press Start to begin.
        GM.Player1.Gamepad.StartButton.Pressed.RemoveListener( GM.ChangeToPlaying );

        // Stop press A to join and press B to quit.
        GM
            .Players
            .Where( player => player == GM.Player1 )
            .ToList()
            .ForEach( player => {
                player.Gamepad.AButton.Pressed.RemoveAllListeners();
                player.Gamepad.BButton.Pressed.RemoveAllListeners();
            } );
    }

    public override void Update()
    {
        // Do nothing.
    }
}
