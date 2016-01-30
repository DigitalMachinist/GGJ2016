using System.Linq;
using UnityEngine;

public class TitleState : GameState
{
    public TitleState( GameManager gm, GMState type ) : base( gm, type )
    {
        // Do nothing.
    }

    public override void OnEntry()
    {
        Debug.Log( "TITLE SCREEN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = false;

        // Show title screen.
        // TODO

        // Player 1 must press Start to enter the game.
        //GM.Player1.Gamepad.StartButton.Pressed.AddListener( () => GM.ChangeState( GMState.MainMenu ) );
        GM.Player1.Gamepad.StartButton.Pressed.AddListener( () => GM.ChangeState( GMState.ColourSelect ) );
    }

    public override void OnExit()
    {
        // Hide title screen.
        // TODO

        // Pressing Start no longer has an effect.
        GM.Player1.Gamepad.StartButton.Pressed.RemoveAllListeners();
    }

    public override void Update()
    {
        // Do nothing.
    }
}
