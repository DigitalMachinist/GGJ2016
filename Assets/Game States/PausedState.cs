using UnityEngine;

public class PausedState : GameState
{
    public PausedState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "PAUSED" );

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
