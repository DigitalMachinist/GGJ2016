
public abstract class GameState
{
    public GameManager GM { get; protected set; }
    public GMState Type { get; protected set; }

    public GameState( GameManager gm, GMState type )
    {
        GM = gm;
        Type = type;
    }

    public abstract void OnEntry();
    public abstract void OnExit();
    public abstract void Update();
}
