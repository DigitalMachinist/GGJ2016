using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An event used to pass information about the state machine and the state in question.
/// </summary>
[Serializable] public class StateMachineEvent : UnityEvent<State, Animator, AnimatorStateInfo, int> { }

/// <summary>
/// The StateMachine class is used by States to communicate to one another and supports the action 
/// of other game objects in response to events signaling state changes.
/// </summary>
[RequireComponent( typeof( Animator ) )]
public class StateMachine : MonoBehaviour
{
    public Animator Animator { get; private set; }
    public State CurrentState { get; private set; }
    public State EnteringState { get; private set; }
    public State ExitingState { get; private set; }

    public StateMachineEvent ControlEnter;
    public StateMachineEvent ControlUpdate;
    public StateMachineEvent ControlExit;
    public StateMachineEvent StateEnter;
    public StateMachineEvent StateUpdate;
    public StateMachineEvent StateExit;

    public void SetCurrentState( State state )
    {
        CurrentState = state;
    }

    public void SetEnteringState( State state )
    {
        EnteringState = state;
    }

    public void SetExitingState( State state )
    {
        ExitingState = state;
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
            .GetBehaviours<State>()
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
            .GetBehaviours<State>()
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

/// <summary>
/// The State class is a specialized StateMachineBehaviour that provides an event interface for 
/// the messages that Unity calls. Perhaps more importantly, States communicate with the next state 
/// (when transitioning into something else) or the previous state (when transitioning from 
/// something else). This allows the end-programmer to utilize transition times without losing the 
/// reliability of events that can signal changes in control contexts (otherwise there are overlaps
/// when 2 control contexts are simultaneously valid).
/// </summary>
public abstract class State : StateMachineBehaviour
{
    public StateMachine StateMachine { get; private set; }

    public StateMachineEvent StateEnter;
    public StateMachineEvent StateUpdate;
    public StateMachineEvent StateExit;
    public StateMachineEvent ControlEnter;
    public StateMachineEvent ControlUpdate;
    public StateMachineEvent ControlExit;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // Get a ref to the StateMachine if one isn't set already.
        if ( StateMachine == null )
        {
            StateMachine = animator.GetComponent<StateMachine>();
        }

        if ( StateMachine == null )
        {
            Debug.LogError( "State requires a StateMachine to support control context callbacks! Control contest events will not be emitted and OnControlEnter(), OnControlUpdate() and OnControlExit() will not be called until a StateMachine is available." );
            return;
        }
        else
        {
            // This state is now entering.
            StateMachine.SetEnteringState( this );

            // Notify the previous state that the next state (this one) has entered.
            if ( StateMachine.CurrentState != null )
            {
                StateMachine.CurrentState.OnControlExit( animator, stateInfo, layerIndex );
            }
        }

        // Emit an event to signal this state enter to other objects.
        StateEnter.Invoke( this, animator, stateInfo, layerIndex );
    }

    /// <summary>
    /// This executes when the transition from the previous state has ended.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public virtual void OnControlEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // This method shouldn't run if no StateMachine is available.
        if ( StateMachine == null )
        {
            return;
        }

        // This state is no longer entering -- it is now the current state.
        StateMachine.SetCurrentState( this );
        StateMachine.SetEnteringState( null );

        // Emit an event to signal this state enter to other objects.
        ControlEnter.Invoke( this, animator, stateInfo, layerIndex );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // Drive the special update function below. It only executes when this state is the only 
        // active state. As soon as any transition begins it no longer runs.
        if ( StateMachine != null && StateMachine.CurrentState == this )
        {
            OnControlUpdate( animator, stateInfo, layerIndex );
        }

        // Emit an event to signal this state update to other objects.
        StateUpdate.Invoke( this, animator, stateInfo, layerIndex );
    }

    /// <summary>
    /// This executes while this is the current (and only) active state. As soon as any 
    /// transition begins it is no longer executed on update.
    /// </remarks>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public virtual void OnControlUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // This method shouldn't run if no StateMachine is available.
        if ( StateMachine == null )
        {
            return;
        }

        // Emit an event to signal this state update to other objects.
        ControlUpdate.Invoke( this, animator, stateInfo, layerIndex );
    }

    /// <summary>
    /// This executes when a transition to the next state has begun.
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public virtual void OnControlExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        // This method shouldn't run if no StateMachine is available.
        if ( StateMachine == null )
        {
            return;
        }

        // This state is no longer the current state -- it has begun exiting.
        StateMachine.SetCurrentState( null );
        StateMachine.SetExitingState( this );

        // Emit an event to signal this state exit to other objects.
        ControlExit.Invoke( this, animator, stateInfo, layerIndex );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( StateMachine != null )
        {
            // This state has now finished exiting.
            StateMachine.SetExitingState( null );

            // Notify the next state that the previous state (this one) has exited.
            if ( StateMachine.CurrentState )
            {
                StateMachine.CurrentState.OnControlEnter( animator, stateInfo, layerIndex );
            }
        }

        // Emit an event to signal this state exit to other objects.
        StateExit.Invoke( this, animator, stateInfo, layerIndex );
    }
}