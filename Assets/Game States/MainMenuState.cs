using System.Linq;
using UnityEngine;

public class MainMenuState : GameState
{
    public MainMenuState( GameManager gm, GMState type ) : base( gm, type )
    {
        // Do nothing.
    }

    public override void OnEntry()
    {
        Debug.Log( "MAIN MENU SCREEN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = false;

        // Show main menu screen.
        // TODO
    }

    public override void OnExit()
    {
        // Hide main menu screen.
        // TODO
    }

    public override void Update()
    {
        
    }
}
