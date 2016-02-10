using UnityEngine;

public abstract class StateSubscriber<T> : MonoBehaviour where T : StateProvider
{
    public T State;
    public StateMachine StateMachine;

    protected virtual void Start()
    {
        if ( StateMachine == null )
        {
            StateMachine = GetComponentInParent<StateMachine>();
            if ( StateMachine == null )
            {
                Debug.LogError( "No StateMachine was found!", this );
                return;
            }
        }

        if ( State == null )
        {
            State = StateMachine.Animator.GetBehaviour<T>();
            if ( State == null )
            {
                Debug.LogError( "No State was found!", this );
                return;
            }
        }

        // Listen to events to proxy these to other objects in the scene.
        State.ControlEnter.AddListener( ( a, b, c, d ) => OnControlEnter() );
        State.ControlUpdate.AddListener( ( a, b, c, d ) => OnControlUpdate() );
        State.ControlExit.AddListener( ( a, b, c, d ) => OnControlExit() );
        State.StateEnter.AddListener( ( a, b, c, d ) => OnStateEnter() );
        State.StateUpdate.AddListener( ( a, b, c, d ) => OnStateUpdate() );
        State.StateExit.AddListener( ( a, b, c, d ) => OnStateExit() );
    }

    public virtual void OnControlEnter() { }
    public virtual void OnControlUpdate() { }
    public virtual void OnControlExit() { }
    public virtual void OnStateEnter() { }
    public virtual void OnStateUpdate() { }
    public virtual void OnStateExit() { }
}
