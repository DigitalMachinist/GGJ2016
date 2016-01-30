using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public enum GMState
{
    NULL,
    Title,
    MainMenu, 
    ColourSelect,
    Playing,
    Action,
    Victory,
    GamepadDisconnected,
    Paused
}

public enum ColourChannel
{
    Red,
    Green,
    Blue
}

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if ( instance == null )
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public Bounds Bounds;
    public Node NodePrefab;
    public CenterSelectedTransform Cursor;
    public float GameSpeed = 1f;
    public GMState UnpauseState = GMState.Playing;
    public Player PlayerTurn;
    public List<Player> Players;

    [Header( "Events" )]
    public FoldablePromise Ready;
    
    public Dictionary<ColourChannel, ColourStats> ColourChannels { get; private set; }
    public List<Node> Nodes { get; private set; }
    public Queue<Node> PendingNodes { get; private set; }
    public GMState State { get; private set; }
    public Dictionary<GMState, GameState> StatesMap { get; private set; }
    public IEnumerable<Victory> Victories { get; private set; }

    public IEnumerable<Player> EnabledPlayers
    {
        get { return Players.Where( player => player.IsEnabled ); }
    }
    public IEnumerable<Victory> EnabledVictories
    {
        get { return Victories.Where( victory => victory.IsEnabled ); }
    }
    public Player Player1
    {
        get { return Players.Single( player => player.Number == 1 ); }
    }

    void Awake()
    {
        Nodes = new List<Node>();

        // Set up a map of GMState enum values to state objects.
        State = GMState.NULL;
        StatesMap = new Dictionary<GMState, GameState>()
        {
            { GMState.NULL, new NullState( this, GMState.NULL ) },
            { GMState.Title, new TitleState( this, GMState.Title ) },
            { GMState.MainMenu, new MainMenuState( this, GMState.MainMenu ) },
            { GMState.ColourSelect, new ColourSelectState( this, GMState.ColourSelect ) },
            { GMState.Playing, new PlayingState( this, GMState.Playing ) },
            { GMState.Action, new PlayerTurnState( this, GMState.Action ) },
            { GMState.Victory, new VictoryState( this, GMState.Victory ) },
            { GMState.GamepadDisconnected, new GamepadDisconnectedState( this, GMState.GamepadDisconnected ) },
            { GMState.Paused, new PausedState( this, GMState.Paused ) }
        };
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

        // Start up and tell the rest of the game that all of the GM resources are ready!
        ChangeState( GMState.Title );
        Ready.Resolve();
    }

    void Update()
    {
        // Run the current state's update function.
        StatesMap[ State ].Update();
    }

    public void ChangeState( GMState newState )
    {
        if ( newState == State )
        {
            return;
        }

        StatesMap[ State ].OnExit();
        State = newState;
        StatesMap[ State ].OnEntry();
    }

    public void ChangeToMainMenu()
    {
        ChangeState( GMState.MainMenu );
    }

    public void ChangeToPlaying()
    {
        // The game can only be played with 2 or more players.
        if ( EnabledPlayers.Count() > 1 )
        {
            ChangeState( GMState.Playing );
        }
    }

    public void Join( Player player )
    {
        // If player has already joined, don't redo that.
        if ( ( player == null || player.IsEnabled ) )
        {
            return;
        }

        // Set up the player.
        player.IsEnabled = true;
        player.Colour = new Color( Random.Range( 0f, 1f ), Random.Range( 0f, 1f ), Random.Range( 0f, 1f ) );
        var xRandom = Random.Range( Bounds.min.x, Bounds.max.x );
        var zRandom = Random.Range( Bounds.min.z, Bounds.max.z );
        SpawnNode( player, new Vector3( xRandom, 0f, zRandom ) );

        // Pressing A no longer joins, but pressing B quits (but not if they are Player 1).
        if ( player != Player1 )
        {
            player.Gamepad.AButton.Pressed.RemoveAllListeners();
            player.Gamepad.BButton.Pressed.AddListener( () => Quit( player ) );
        }
    }

    public void Quit( Player player, bool force = false )
    {
        // Player 1 can't quit.
        if ( !force && ( player == null || player == Player1 ) )
        {
            return;
        }

        // Reset the player and clean up their first node.
        player.IsEnabled = false;
        player.Colour = Color.black;
        foreach ( var node in player.Nodes )
        {
            Nodes.Remove( node );
        }
        player
            .Nodes
            .ToList()
            .ForEach( node => Destroy( node.gameObject ) );

        // Allow player to rejoin by pressing A, as before.
        player.Gamepad.AButton.Pressed.AddListener( () => Join( player ) );
        player.Gamepad.BButton.Pressed.RemoveAllListeners();
    }

    public void QuitAll()
    {
        foreach ( var player in EnabledPlayers )
        {
            Quit( player );
        }
    }

    public bool SpawnNode( Player player, Vector3 position, bool simulate = false )
    {
        if ( !Bounds.Contains( position ) )
        {
            return false;
        }

        if ( !simulate )
        {
            var nodesContainer = GameObject.FindGameObjectWithTag( "NodesContainer" );
            var node = (Node)Instantiate( NodePrefab, position, Quaternion.identity );
            node.transform.parent = nodesContainer.transform;
            node.Player = player;
            node.name = "Node (Player " + player.Number + ")";
            node.GetComponent<Renderer>().material.color = player.Colour;

            Nodes.Add( node );
            player.SelectedNode = node;
        }

        return true;
    }
}
