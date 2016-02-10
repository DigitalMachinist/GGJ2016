using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event used to pass information about the state machine and the state in question.
/// </summary>
[Serializable] public class StateEvent 
    : UnityEvent<StateProvider, Animator, AnimatorStateInfo, int> { }
[Serializable] public class FoldableStateEvent 
    : FoldableEvent<StateEvent, StateProvider, Animator, AnimatorStateInfo, int> { }

/// <summary>
/// The StateMachine class is used by States to communicate to one another and supports the action 
/// of other game objects in response to events signaling state changes.
/// </summary>
[ RequireComponent( typeof( Animator ) )]
public class StateMachine : MonoBehaviour
{
    public bool IsDebugLogging = false;

    public Animator Animator { get; private set; }
    public StateProvider CurrentState { get; private set; }
    public StateProvider EnteringState { get; private set; }
    public StateProvider ExitingState { get; private set; }
    public bool IsStarted { get; private set; }
    public bool IsTransitioning { get; private set; }

    public FoldableStateEvent ControlEnter;
    public FoldableStateEvent ControlUpdate;
    public FoldableStateEvent ControlExit;
    public FoldableStateEvent StateEnter;
    public FoldableStateEvent StateUpdate;
    public FoldableStateEvent StateExit;

    public void SetCurrentState( StateProvider state )
    {
        CurrentState = state;
        IsTransitioning = true;
    }

    public void SetEnteringState( StateProvider state )
    {
        EnteringState = state;
    }

    public void SetExitingState( StateProvider state )
    {
        ExitingState = state;
        IsTransitioning = false;
    }

    public void SetIsStarted()
    {
        IsStarted = true;
    }

    void Awake()
    {
        Animator = GetComponent<Animator>();
        if ( Animator == null )
        {
            Debug.LogError( "StateMachine requires an animator component!" );
            return;
        }

        Animator
            .GetBehaviours<StateProvider>()
            .ToList()
            .ForEach( state => {
                state.ControlEnter.AddListener( ControlEnter.Invoke );
                state.ControlUpdate.AddListener( ControlUpdate.Invoke );
                state.ControlExit.AddListener( ControlExit.Invoke );
                state.StateEnter.AddListener( StateEnter.Invoke );
                state.StateUpdate.AddListener( StateUpdate.Invoke );
                state.StateExit.AddListener( StateExit.Invoke );
            } );
    }

    void OnDestroy()
    {
        if ( Animator == null )
        {
            return;
        }

        Animator
            .GetBehaviours<StateProvider>()
            .ToList()
            .ForEach( state => {
                state.ControlEnter.RemoveListener( ControlEnter.Invoke );
                state.ControlUpdate.RemoveListener( ControlUpdate.Invoke );
                state.ControlExit.RemoveListener( ControlExit.Invoke );
                state.StateEnter.RemoveListener( StateEnter.Invoke );
                state.StateUpdate.RemoveListener( StateUpdate.Invoke );
                state.StateExit.RemoveListener( StateExit.Invoke );
            } );
    }
}
