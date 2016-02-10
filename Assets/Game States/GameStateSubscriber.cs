using UnityEngine;

public abstract class GameStateSubscriber<T> : StateSubscriber<T> where T : StateProvider
{
    public Animator Animator;
    public Transform CameraTarget;
    public Cursor Cursor;

    GameManager gm;
    public GameManager GM
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

    protected override void Start()
    {
        base.Start();

        if ( Animator == null )
        {
            Animator = GetComponent<Animator>();
            if ( Animator == null )
            {
                Debug.LogError( "No Animator was found!", this );
            }
        }

        if ( CameraTarget == null )
        {
            CameraTarget = transform.FindChild( "Camera Target" );
            if ( CameraTarget == null )
            {
                Debug.LogError( "No CameraTarget was found!", this );
            }
        }

        if ( Cursor == null )
        {
            Cursor = FindObjectOfType<Cursor>();
            if ( Cursor == null )
            {
                Debug.LogError( "No Cursor was found!", this );
            }
        }
    }

    #region UI Controls

    public void ActivateScreen( bool moveCursor = false )
    {
        SetScreenActive( true );
        if ( moveCursor )
        {
            MoveCursorToCameraTarget();
        }
    }

    public void DeactivateScreen()
    {
        SetScreenActive( false );
    }

    public void SetScreenActive( bool value )
    {
        Animator.SetBool( "IsActive", value );
    }

    #endregion

    #region Camera Controls

    public void MoveCursorToCameraTarget()
    {
        Cursor.transform.position = CameraTarget.transform.position;
    }

    #endregion
}
