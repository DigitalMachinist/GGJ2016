using System;
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

    public NodeEvent Created;
    public NodeEvent Placed;
    public NodeEvent Destroyed;
    public NodeCollisionEvent Collision; // TODO
    public NodeEvent ReadyToAct; // TODO
    public NodeEvent Grew;
    public ActionEvent ActionSelected;
    public ActionEvent ActionDeselected;


    public List<NodeAction> Actions { get; private set; }
    public ColorSampler Sampler { get; private set; }

    public Color Colour
    {
        get { return Sampler.SampledColor; }
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
        Created.Invoke( this );
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
        ActionSelected.Invoke( action );
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
