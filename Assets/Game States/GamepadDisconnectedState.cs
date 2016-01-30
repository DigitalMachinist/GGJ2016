using UnityEngine;

public class GamepadDisconnectedState : GameState
{
    public GamepadDisconnectedState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "A CONTROLLER HAS DISCONNECTED" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = false;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}
