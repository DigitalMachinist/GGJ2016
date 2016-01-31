using UnityEngine;

public class ActionState : GameState
{
    public ActionState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Debug.Log( "STOPPED FOR PLAYER TURN" );

        Time.timeScale = 0f;
        GM.Cursor.enabled = true;

        // Listen for the completion of all pending nodes' actions.
        GM.ContinuePlaying.AddListener( () => GM.ChangeState( GMState.Playing ) );

        // Listen for any further nodes that might need to take their turns now.
        GM.RequiresAction.AddListener( GM.NextPendingNode );

        // Begin actioning the first node that is about to take its turn.
        GM.NextPendingNode();
    }

    public override void OnExit()
    {
        GM.UnpauseState = Type;

        GM.ContinuePlaying.RemoveAllListeners();
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
