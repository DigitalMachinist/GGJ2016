using UnityEngine;

public class NullState : GameState
{
    public NullState( GameManager gm, GMState type ) : base( gm, type )
    {

    }

    public override void OnEntry()
    {
        Time.timeScale = 0f;
    }

    public override void OnExit()
    {

    }

    public override void Update()
    {

    }
}
