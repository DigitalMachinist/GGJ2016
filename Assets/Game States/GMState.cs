
public class GMState : State
{
    GameManager gm;
    protected GameManager GM
    {
        get
        {
            if ( gm == null )
            {
                gm = GameManager.Instance;
            }
            return gm;
        }
    }
}
