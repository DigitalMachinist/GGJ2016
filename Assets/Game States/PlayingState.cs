using UnityEngine;

public class PlayingState : GameState
{
    public PlayingState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "TIME IS PROGRESSING" );

        Time.timeScale = GameManager.Instance.GameSpeed;
        GM.Cursor.enabled = true;

        // Set the bounds of the play area.
        GM.Bounds = new Bounds( new Vector3( 80f, 0f, 47f ), new Vector3( 106f, 0f, 60f ) );
    }

    public override void OnExit()
    {
        GM.UnpauseState = Type;
    }

    public override void Update()
    {

    }
}
