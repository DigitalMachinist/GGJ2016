using UnityEngine;

public class VictoryState : GameState
{
    public VictoryState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "A PLAYER HAS WON THE GAME" );

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
