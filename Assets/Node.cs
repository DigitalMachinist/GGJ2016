using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class NodeEvent : UnityEvent<Node> { }
[Serializable] public class NodeCollisionEvent : UnityEvent<Node, Collision> { }

public class Node : MonoBehaviour
{
    public Player Player;
    public int HP;
    public int Level;
    public NodeAction SelectedAction;
    public bool IsSimulating;

    public NodeEvent Created;
    public NodeEvent Placed;
    public NodeEvent Destroyed;
    public NodeCollisionEvent Collision; // TODO
    public NodeEvent ReadyToAct;
    public NodeEvent Grew;
    public ActionEvent ActionBegun;
    public ActionEvent ActionSelected;
    public ActionEvent ActionDeselected;


    public Dictionary<string, NodeAction> Actions { get; private set; }
    public GameManager GM { get; private set; }
    public ColorPicker ColourPicker { get; private set; }
    public IEnumerable<Node> Selection { get; private set; }

    public Color Colour
    {
        get { return ColourPicker.Color; }
    }
    public bool HasSelectedAction
    {
        get { return ( SelectedAction == null ); }
    }
    public bool IsSelected
    {
        get { return ( Player == null ) ? false : ( Player.SelectedNode == this ); }
    }
    public float CreateDistance
    {
        get { return 8f; }
    }

    void Awake()
    {
        ColourPicker = GetComponent<ColorPicker>();
        Selection = new List<Node>();
        Created.Invoke( this );
    }

    void Start()
    {
        GM = FindObjectOfType<GameManager>();

        // The list of default actions includes both Grow and Create Node.
        Actions = new Dictionary<string, NodeAction>();
        AddAction( NodeActionFactory.CreateNode() );
        AddAction( NodeActionFactory.Grow() );

        foreach ( var action in Actions )
        {
            Debug.Log( "Node actions:" );
            Debug.Log( action );
        }

        ReadyToAct.Invoke( this );
    }

    void Update()
    {

    }

    void OnDestroy()
    {
        Destroyed.Invoke( this );
    }

    public void AddAction( NodeAction action )
    {
        Actions.Add( action.name, action );
        action.transform.parent = transform;
        action.transform.localPosition = Vector3.zero;
        action.transform.localRotation = Quaternion.identity;
        action.name = action.ToString();
        action.Node = this;
        action.BeganWarmup.AddListener( ActionBegun.Invoke );
    }

    public void ClearActions()
    {
        SelectedAction = null;
        Actions
            .Keys
            .ToList()
            .ForEach( key => {
                Actions[ key ].BeganWarmup.RemoveAllListeners();
                Actions[ key ].Ended.RemoveAllListeners();
                Destroy( Actions[ key ] );
                Actions.Remove( key );
            } );

    }

    void SetSelectedAction( NodeAction action )
    {
        // Show/hide the stuff that ought to be.
        if ( SelectedAction != null )
        {
            SelectedAction.transform.FindChild( "Visuals" ).gameObject.SetActive( false );
            ActionDeselected.Invoke( SelectedAction );
        }
        SelectedAction = action;
        SelectedAction.transform.FindChild( "Visuals" ).gameObject.SetActive( true );

        // Get a new selection of nodes.
        Selection = GM.Nodes;
        foreach ( var filter in action.SelectionFilters )
        {
            Selection = Selection.Where( filter.Test );
        }

        ActionSelected.Invoke( action );
    }

    public void ExecuteAction()
    {
        ExecuteAction( SelectedAction.name );
    }
    public void ExecuteAction( string key, bool isSimulating = false, bool force = false )
    {
        if ( !force && !Actions.ContainsKey( key ) )
        {
            return;
        }

        // Begin executing all of the node behaviour coroutines for this action concurrently.
        var action = Actions[ key ];
        action
            .NodeBehaviours
            .ForEach( behaviour => {
                StartCoroutine( behaviour.Behaviour( action, Selection, isSimulating ) );
            } );

        // Continue looping if simulation mode is on.
        IsSimulating = isSimulating;
        action.Ended.RemoveListener( RepeatSimulatedAction );
        if ( isSimulating )
        {
            action.Ended.AddListener( RepeatSimulatedAction );
        }
        else
        {
            action.Ended.AddListener( endedAction => ReadyToAct.Invoke( this ) );
            GM.NextPendingNode();
        }
    }

    void RepeatSimulatedAction( NodeAction action )
    {
        ExecuteAction( action.name, true );
    }

    public void Grow()
    {
        Level++;
        Grew.Invoke( this );
    }

    public void UpdateMaterialColour()
    {
        GetComponent<Renderer>().material.color = Colour;
    }
}
