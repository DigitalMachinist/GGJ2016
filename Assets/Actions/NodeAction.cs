using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class ActionEvent : UnityEvent<NodeAction> { }

public class NodeAction : MonoBehaviour
{
    public Node Node;
    public Projectile ProjectilePrefab;
    public float WarmupDelay;
    public float Duration;
    public float CooldownDelay;
    public float Angle;
    public int EnergyCost = 0;
    public int HPCost = 0;

    public ActionEvent BeganWarmup; // TODO
    public ActionEvent BeganDuration; // TODO
    public ActionEvent BeganCooldown; // TODO
    public ActionEvent Ended; // TODO
    public ProjectileEvent ProjectileEmitted; // TODO

    public List<CollisionBehaviour> CollisionDeflectBehaviours { get; protected set; }
    public List<CollisionBehaviour> CollisionImpactBehaviours { get; protected set; }
    public List<NodeBehaviour> NodeBehaviours { get; protected set; }
    public List<SelectionFilter> SelectionFilters { get; protected set; }

    public bool IsSelected
    {
        get { return ( Node == null ) ? false : ( Node.SelectedAction == this ); }
    }

    protected virtual void Awake()
    {
        CollisionDeflectBehaviours = new List<CollisionBehaviour>();
        CollisionImpactBehaviours = new List<CollisionBehaviour>();
        NodeBehaviours = new List<NodeBehaviour>();
        SelectionFilters = new List<SelectionFilter>();
    }

    void Update()
    {

    }

    public override string ToString()
    {
        return
            NodeBehaviours
                .Where( behaviour => !behaviour.GetsFinalWord )
                .Concat( NodeBehaviours.Where( behaviour => behaviour.GetsFinalWord ) )
                .Select( behaviour => behaviour.Descriptor )
                .Aggregate( "", ( phrase, descriptor ) => phrase + " " + descriptor );
    }
}
