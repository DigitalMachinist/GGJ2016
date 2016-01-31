using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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

[Serializable] public class PlayerEvent : UnityEvent<Player> { }

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
    public NodeAction ActionPrefab;
    public GameObject ColourWheelMask;
    public Cursor Cursor;
    public float GameSpeed = 1f;
    public GMState UnpauseState = GMState.Playing;
    public Player PlayerTurn;
    public List<Player> Players;

    [Header( "Events" )]
    public FoldablePromise Ready;
    public PlayerEvent PlayerJoined;
    public PlayerEvent PlayerQuit;
    public PlayerEvent PlayerReady;
    public PlayerEvent PlayerNotReady;
    public UnityEvent RequiresAction;
    public UnityEvent ContinuePlaying;

    public ColourEffect ColourEffect { get; private set; }
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
    public bool HasPendingNodes
    {
        get { return ( PendingNodes.Count > 0 ); }
    }
    public Player Player1
    {
        get { return Players.Single( player => player.Number == 1 ); }
    }
    public Player Player2
    {
        get { return Players.Single( player => player.Number == 2 ); }
    }
    public Player Player3
    {
        get { return Players.Single( player => player.Number == 3 ); }
    }
    public Player Player4
    {
        get { return Players.Single( player => player.Number == 4 ); }
    }
    public IEnumerable<Player> ReadyPlayers
    {
        get { return Players.Where( player => player.IsReady ); }
    }
    
    public ColourBenefit GetColourBenefit( Color sampledColour )
    {
        // Use the values of each colour with respect to the sum of all colour channels (i.e. 
        // the fraction that each channel contributes to the final colour) as the input to the 
        // animation curves that compute the coefficients for colour benefit.
        var sum = sampledColour.r + sampledColour.g + sampledColour.b;
        
        var rCoeff = ColourEffect.RegenRateWrtRed.Evaluate( sampledColour.r / sum );
        var gCoeff = ColourEffect.ActionRateWrtGreen.Evaluate( sampledColour.g / sum );
        var bCoeff = ColourEffect.EnergyRateWrtBlue.Evaluate( sampledColour.b / sum );
        return new ColourBenefit( rCoeff, gCoeff, bCoeff );
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
            { GMState.Action, new ActionState( this, GMState.Action ) },
            { GMState.Victory, new VictoryState( this, GMState.Victory ) },
            { GMState.GamepadDisconnected, new GamepadDisconnectedState( this, GMState.GamepadDisconnected ) },
            { GMState.Paused, new PausedState( this, GMState.Paused ) }
        };
    }

    void Start()
    {
        // Make each of the ColourStats easily accessible.
        var channels = transform.FindChild( "Colour Channels" );
        ColourEffect = FindObjectOfType<ColourEffect>();

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
        if ( ReadyPlayers.Count() >= 2 )
        {
            ChangeState( GMState.Playing );
        }
    }

    public void JoinPlayer( Player player )
    {
        // If player has already joined, don't redo that.
        if ( ( player == null || player.IsEnabled ) )
        {
            return;
        }
        
        player.IsEnabled = true;
        PlayerJoined.Invoke( player );
    }

    public void QuitPlayer( Player player, bool force = false )
    {
        // Player 1 can't quit.
        if ( !force && ( player == null || player == Player1 ) )
        {
            return;
        }

        // Reset the player.
        player.IsEnabled = false;
        player.Colour = Color.black;

        // Clean up all of the player's nodes.
        // This should happen here instead of in an event handler because this should give me a  
        // way to allow players to quite the game at any time -- even during an ongoing game.
        player
            .Nodes
            .ToList()
            .ForEach( node => {
                Nodes.Remove( node );
                Destroy( node.gameObject );
            } );

        PlayerQuit.Invoke( player );
    }

    public void QuitAllPlayers()
    {
        // Yes, this even quits player 1. They should be rejoined in code after this.
        foreach ( var player in EnabledPlayers )
        {
            QuitPlayer( player );
        }
    }

    public void ReadyPlayer( Player player )
    {
        if ( player.SamplingNode.Colour == Color.black )
        {
            Debug.Log( "Sampled colour was black. Blocking the player from being ready..." );
            return;
        }
        player.IsReady = true;
        PlayerReady.Invoke( player );
    }

    public void NotReadyPlayer( Player player )
    {
        player.IsReady = false;
        PlayerNotReady.Invoke( player );
    }

    public Node InstantiateNode( Node nodePrefab, Player player, Vector3 position, Quaternion rotation )
    {
        var node = (Node)Instantiate( nodePrefab, position, rotation );

        // Set the new node's player equal to the creator node's player.
        node.Player = player;
        node.name = "Node (Player " + player.Number + ")";

        // This may become part of the Node's animation controller eventually.
        node.GetComponent<Renderer>().material.color = player.Colour;

        // Listen for the node's events.
        node.ReadyToAct.AddListener( readyNode => PendingNodes.Enqueue( node ) );

        return node;
    }

    public Node StartPlaceNode( Node parentNode )
    {
        // This starts the node placement process --> FinalizePlaceNode() finishes it.
        // Create the new node <distance> units forward from the creator node, giving the new node 
        // a random rotation. Parent the new node to the creator node.
        var offset = parentNode.CreateDistance * parentNode.transform.TransformDirection( Vector3.forward );
        var position = parentNode.transform.position + offset;
        //var rotation = Quaternion.Euler( 0f, Random.Range( 0f, 360f ), 0f );
        var rotation = Quaternion.identity;
        var childNode = InstantiateNode( NodePrefab, parentNode.Player, position, rotation );
        childNode.transform.parent = parentNode.transform;
        childNode.transform.localRotation = Quaternion.identity;
        return childNode;
    }

    public bool FinalizePlaceNode( Node node )
    {
        if ( !Bounds.Contains( node.transform.position ) )
        {
            // The node can't be placed out of bounds.
            return false;
        }
        
        // Parent the new node to the nodes collection now that it is a real thing. Add it to the 
        // nodes collection so it can be queried by actions and set it to be the player's currently 
        // selected node.
        var nodesContainer = GameObject.FindGameObjectWithTag( "NodesContainer" );
        node.transform.localRotation = Quaternion.Euler( 0f, Random.Range( 0f, 360f ), 0f );
        node.transform.parent = nodesContainer.transform;
        Nodes.Add( node );
        node.Player.SetSelectedNode( node );

        return true;
    }

    public void ClearAllNodes()
    {
        Nodes
            .ToList()
            .ForEach( node => {
                Nodes.Remove( node );
                Destroy( node.gameObject );
            } );

        Players
            .ForEach( player => player.SetSelectedNode( null ) );
    }

    public void PrepareGameplayArea()
    {
        // Clear all of the nodes we just created so we start fresh.
        ClearAllNodes();

        // Instantiate players' first nodes randomly so they come up in the turn order randomly.
        ReadyPlayers
            .OrderBy( player => -player.Number )
            //.OrderBy( player => Guid.NewGuid() ) // Random ordering?
            .ToList()
            .ForEach( player => {

                // Reset each player's energy.
                player.Energy = 0;

                // Instantiate first node randomly.
                var xRandom = Random.Range( Bounds.min.x, Bounds.max.x );
                var zRandom = Random.Range( Bounds.min.z, Bounds.max.z );
                FinalizePlaceNode(
                    InstantiateNode(
                        NodePrefab,
                        player,
                        new Vector3( xRandom, 0f, zRandom ),
                        Quaternion.Euler( 0f, Random.Range( 0f, 360f ), 0f )
                    )
                );

            } );
    }

    public void NextPendingNode()
    {
        var node = PendingNodes.Dequeue();
        if ( node == null )
        {
            ContinuePlaying.Invoke();
            return;
        }
        PlayerTurn = node.Player;
        PlayerTurn.SelectedNode = node;
    }
}
