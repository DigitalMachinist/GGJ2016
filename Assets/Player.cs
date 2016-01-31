using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsEnabled = false;
    public bool IsReady = false;
    public int Number = 1;
    public int Energy = 0;
    public Color Colour = Color.black;
    public Xbox360Gamepad Gamepad;
    public Node SelectedNode;
    public Node SamplingNode;

    public NodeEvent NodeSelected;
    public NodeEvent NodeDeselected;

    public GameManager GM { get; private set; }

    //public ColourBenefit ColourBenefit
    //{
    //    get { return }
    //}
    public IEnumerable<Node> Nodes
    {
        get { return GM.Nodes.Where( node => node.Player == this ); }
    }

    void Awake()
    {
        // Player controls.
        Gamepad.LeftTrigger.Pressed.AddListener( PreviousNode );
        Gamepad.RightTrigger.Pressed.AddListener( NextNode );

        // Debug controls.
        Gamepad.LeftBumper.Pressed.AddListener( CreateNode );
        Gamepad.RightBumper.Pressed.AddListener( SkipTurn );
    }

    void Start()
    {
        GM = FindObjectOfType<GameManager>();
    }

    void CreateNode()
    {
        GM.InstantiateNode( GM.NodePrefab, this, new Vector3( Random.Range( 27f, 133f ), 0f, Random.Range( 15f, 75f ) ), Quaternion.identity );
    }

    void NextNode()
    {
        if ( GM.PlayerTurn != this )
        {
            return;
        }

        var nodeList = Nodes.ToList();
        if ( SelectedNode == null )
        {
            SetSelectedNode( nodeList[ 0 ] );
        }
        else
        {
            var index = nodeList.IndexOf( SelectedNode );
            var newIndex = ( index + 1 ) % nodeList.Count;
            SetSelectedNode( nodeList[ newIndex ] );
        }
    }

    void PreviousNode()
    {
        if ( GM.PlayerTurn != this )
        {
            return;
        }

        var nodeList = Nodes.ToList();
        if ( SelectedNode == null )
        {
            SetSelectedNode( nodeList[ 0 ] );
        }
        else
        {
            var index = nodeList.IndexOf( SelectedNode );
            var newIndex = index - 1;
            if ( newIndex < 0 )
            {
                newIndex = nodeList.Count - 1;
            }
            SetSelectedNode( nodeList[ newIndex ] );
        }
    }

    void SkipTurn()
    {
        Debug.Log( "SKIP TURN: Figure this out later!" );
    }

    public void SetSelectedNode( Node node )
    {
        if ( SelectedNode != null )
        {
            NodeDeselected.Invoke( SelectedNode );
        }
        SelectedNode = node;
        NodeSelected.Invoke( node );
    }
}
