using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeAction : MonoBehaviour
{
    public Node Node;
    public float WarmupDelay;
    public float Duration;
    public float CooldownDelay;
    public float Angle;
    public int EnergyCost = 0;
    public int HPCost = 0;

    public List<CollisionBehaviour> CollisionDeflectBehaviours { get; private set; }
    public List<CollisionBehaviour> CollisionImpactBehaviours { get; private set; }
    public List<NodeBehaviour> NodeBehaviours { get; private set; }
    public List<SelectionFilter> SelectionFilters { get; private set; }

    void Awake()
    {
        CollisionDeflectBehaviours = new List<CollisionBehaviour>();
        CollisionImpactBehaviours = new List<CollisionBehaviour>();
        NodeBehaviours = new List<NodeBehaviour>();
        SelectionFilters = new List<SelectionFilter>();
    }
}
