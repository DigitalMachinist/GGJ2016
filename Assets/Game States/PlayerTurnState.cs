using UnityEngine;

public class PlayerTurnState : GameState
{
    public PlayerTurnState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "STOPPED FOR PLAYER TURN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = true;
    }

    public override void OnExit()
    {
        GM.UnpauseState = Type;
    }

    public override void Update()
    {

    }
}
