using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool IsEnabled = false;
    public int Number = 1;
    public Color Colour = Color.black;
    public Xbox360Gamepad Gamepad;
    public Node SelectedNode;

    public GameManager GM { get; private set; }

    public IEnumerable<Node> Nodes
    {
        get { return GM.Nodes.Where( node => node.Player == this ); }
    }

    void Awake()
    {
        // Player controls.
        Gamepad.LeftTrigger.Pressed.AddListener( () => SelectedNode = PrevNode() );
        Gamepad.RightTrigger.Pressed.AddListener( () => SelectedNode = NextNode() );
    }

    void Start()
    {
        GM = FindObjectOfType<GameManager>();
    }

    Node PrevNode()
    {
        var nodeList = Nodes.ToList();
        if ( SelectedNode == null )
        {
            return nodeList[ 0 ];
        }
        else
        {
            var index = nodeList.IndexOf( SelectedNode );
            var newIndex = index - 1;
            if ( newIndex < 0 )
            {
                newIndex = nodeList.Count - 1;
            }
            return nodeList[ newIndex ];
        }
    }

    Node NextNode()
    {
        var nodeList = Nodes.ToList();
        if ( SelectedNode == null )
        {
            return nodeList[ 0 ];
        }
        else
        {
            var index = nodeList.IndexOf( SelectedNode );
            var newIndex = ( index + 1 ) % nodeList.Count;
            return nodeList[ newIndex ];
        }
    }
}
