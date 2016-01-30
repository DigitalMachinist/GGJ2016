using System;
using System.Collections.Generic;
using System.Linq;
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

public enum ColourChannel
{
    Red,
    Green,
    Blue
}

public class GameManager : MonoBehaviour
{
    public CenterSelectedTransform Cursor;
    public float GameSpeed = 1f;
    public Player PlayerTurn;
    public List<Player> Players;
    public List<Node> Nodes;

    [Header( "Events" )]
    public FoldablePromise Ready;

    bool unpauseToPlayerTurn = false;

    public Dictionary<ColourChannel, ColourStats> ColourChannels { get; private set; }
    public StateMachine<GMState, GMTrigger> FSM { get; private set; }
    public Queue<Node> PendingNodes { get; private set; }
    public IEnumerable<Victory> Victories { get; private set; }

    public IEnumerable<Player> EnabledPlayers
    {
        get { return Players.Where( player => player.IsEnabled ); }
    }
    public IEnumerable<Victory> EnabledVictories
    {
        get { return Victories.Where( victory => victory.IsEnabled ); }
    }

    void Awake()
    {
        ConfigureFSM();
        Ready.AddListener( () => FSM.Fire( GMTrigger.Ready ) );
    }

    void Start()
    {
        // Make each of the ColourStats easily accessible.
        var channels = transform.FindChild( "Colour Channels" );
        ColourChannels = new Dictionary<ColourChannel, ColourStats>()
        {
            { ColourChannel.Red, channels.FindChild( "Red Channel" ).GetComponent<ColourStats>() },
            { ColourChannel.Green, channels.FindChild( "Green Channel" ).GetComponent<ColourStats>() },
            { ColourChannel.Blue, channels.FindChild( "Blue Channel" ).GetComponent<ColourStats>() }
        };

        // The list of pending nodes is empty for now. Eventually players will join the game and 
        // their initial turns will be queued here.
        PendingNodes = new Queue<Node>();

        // Read in the available list of victory conditions. The currently enabled victory 
        // conditions can be obtained using the EnabledVictories property.
        var victories = transform.FindChild( "Colour Channels" );
        Victories = new List<Victory>( transform.GetComponentsInChildren<Victory>() );

        // Tell the rest of the game that all of the core resources are ready!
        Ready.Resolve();
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
                unpauseToPlayerTurn = false;
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
                unpauseToPlayerTurn = true;
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
