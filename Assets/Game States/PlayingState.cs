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

        GM.RequiresAction.AddListener( () => GM.ChangeState( GMState.Action ) );
    }

    public override void OnExit()
    {
        GM.UnpauseState = Type;

        GM.RequiresAction.RemoveAllListeners();
    }

    public override void Update()
    {
        if ( GM.HasPendingNodes )
        {
            GM.RequiresAction.Invoke();
        }
    }
}
