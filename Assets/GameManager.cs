using System;
using System.Collections.Generic;
using Stateless;
using UnityEngine;

public enum GMState
{
    NULL,
    Start,
    Playing,
    PlayerTurn,
    GameOver,
    ControllerDisconnected,
    Paused
}

public enum GMTrigger
{
    Ready,
    PlayBegan,
    NodeIsReady,
    PlayerTurnHasEnded,
    PlayerHasWon,
    Reset, 
    ControllerDisconnected,
    ControllerReconnected,
    Paused,
    Unpaused
}

public class GameManager : MonoBehaviour
{
    public float GameSpeed = 1f;
    public List<Player> Players;
    public List<ColourChannel> ColourChannels;
    public List<Node> Nodes;

    [Header( "Events" )]
    public FoldablePromise Ready;

    bool unpauseToPlayerTurn = false;

    public StateMachine<GMState, GMTrigger> FSM { get; private set; }
    public Queue<Node> PendingNodes { get; private set; }
    public List<Func<Player>> VictoryTestFuncs { get; private set; }

    void Awake()
    {
        ConfigureFSM();
        Ready.AddListener( () => FSM.Fire( GMTrigger.Ready ) );
    }

    void Update()
    {

    }

    void ConfigureFSM()
    {
        FSM = new StateMachine<GMState, GMTrigger>( GMState.Start );

        FSM
            .Configure( GMState.NULL )
            .Permit( GMTrigger.Ready, GMState.Start )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.Start )
            .Permit( GMTrigger.PlayBegan, GMState.Playing )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.Playing )
            .Permit( GMTrigger.NodeIsReady, GMState.PlayerTurn )
            .Permit( GMTrigger.PlayerHasWon, GMState.GameOver )
            .Permit( GMTrigger.ControllerDisconnected, GMState.ControllerDisconnected )
            .Permit( GMTrigger.Paused, GMState.Paused )
            .OnEntry( () => {
                Time.timeScale = GameSpeed;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.PlayerTurn )
            .Permit( GMTrigger.PlayerTurnHasEnded, GMState.Playing )
            .Permit( GMTrigger.ControllerDisconnected, GMState.ControllerDisconnected )
            .Permit( GMTrigger.Paused, GMState.Paused )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.GameOver )
            .Permit( GMTrigger.Reset, GMState.Start )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.ControllerDisconnected )
            .Permit( GMTrigger.ControllerReconnected, GMState.Playing )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );

        FSM
            .Configure( GMState.Paused )
            .Permit( GMTrigger.Reset, GMState.Start )
            .PermitIf( GMTrigger.Unpaused, GMState.Playing, () => !unpauseToPlayerTurn )
            .PermitIf( GMTrigger.Unpaused, GMState.PlayerTurn, () => !unpauseToPlayerTurn )
            .OnEntry( () => {
                Time.timeScale = 0f;
            } )
            .OnExit( () => {

            } );
    }
}
