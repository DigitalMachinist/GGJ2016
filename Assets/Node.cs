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


    public List<NodeAction> Actions { get; private set; }
    public GameManager GM { get; private set; }
    public ColorSampler Sampler { get; private set; }
    public IEnumerable<Node> Selection { get; private set; }

    public Color Colour
    {
        get { return Sampler.SampledColor; }
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
        Sampler = GetComponent<ColorSampler>();
        Selection = new List<Node>();
        Created.Invoke( this );
    }

    void Start()
    {
        GM = FindObjectOfType<GameManager>();

        // The list of default actions includes both Grow and Create Node
        Actions = new List<NodeAction>()
        {
            NodeActionFactory.Grow(), 
            NodeActionFactory.CreateNode()
        };

        foreach ( var action in Actions )
        {
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
        Actions.Add( action );
        action.transform.parent = transform;
        action.transform.localPosition = Vector3.zero;
        action.transform.localRotation = Quaternion.identity;
        action.name = action.ToString();
    }

    public void ClearActions()
    {
        SelectedAction = null;
        Actions
            .ToList()
            .ForEach( action => {
                Actions.Remove( action );
                Destroy( action );
            } );

    }

    void SetSelectedAction( NodeAction action )
    {
        if ( SelectedAction != null )
        {
            ActionDeselected.Invoke( SelectedAction );
        }
        SelectedAction = action;

        // Get a new selection of nodes.
        Selection = GM.Nodes;
        foreach ( var filter in action.SelectionFilters )
        {
            Selection = Selection.Where( filter.Test );
        }

        ActionSelected.Invoke( action );
    }

    public void ExecuteAction( NodeAction action, bool isSimulating, bool force = false )
    {
        if ( !force && !Actions.Contains( action ) )
        {
            return;
        }

        // Begin executing all of the node behaviour coroutines for this action concurrently.
        action
            .NodeBehaviours
            .ForEach( behaviour => {
                StartCoroutine( behaviour.Behaviour( action, Selection, isSimulating ) );
            } );

        // Continue looping if simulation mode is on.
        action.Ended.RemoveListener( RepeatSimulatedAction );
        IsSimulating = isSimulating;
        if ( isSimulating )
        {
            GM.NextPendingNode();
            action.Ended.AddListener( RepeatSimulatedAction );
        }
    }

    void RepeatSimulatedAction( NodeAction action )
    {
        ExecuteAction( action, true );
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
