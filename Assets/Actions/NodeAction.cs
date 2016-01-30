using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public ActionEvent BeganWarmup;
    public ActionEvent BeganDuration;
    public ActionEvent BeganCooldown;
    public ActionEvent Ended;

    public List<CollisionBehaviour> CollisionDeflectBehaviours { get; private set; }
    public List<CollisionBehaviour> CollisionImpactBehaviours { get; private set; }
    public List<NodeBehaviour> NodeBehaviours { get; private set; }
    public List<SelectionFilter> SelectionFilters { get; private set; }

    public bool IsSelected
    {
        get { return ( Node == null ) ? false : ( Node.SelectedAction == this ); }
    }

    void Awake()
    {
        CollisionDeflectBehaviours = new List<CollisionBehaviour>();
        CollisionImpactBehaviours = new List<CollisionBehaviour>();
        NodeBehaviours = new List<NodeBehaviour>();
        SelectionFilters = new List<SelectionFilter>();
    }

    void Update()
    {

    }
}
