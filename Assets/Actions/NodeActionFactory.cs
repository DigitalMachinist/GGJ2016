﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class NodeActionFactory
{
    public static NodeAction GetAction(
        NodeAction nodeActionPrefab,
        float warmup,
        float duration,
        float cooldown,
        float angle,
        int energyCost,
        int hpCost, 
        List<NodeBehaviour> nodeBehaviour,
        List<CollisionBehaviour> collisionImpactBehaviours = null,
        List<CollisionBehaviour> collisionDeflectBehaviours = null,
        List<SelectionFilter> selectionFilters = null
    )
    {
        var action = (NodeAction)Object.Instantiate( nodeActionPrefab );
        action.WarmupDelay = warmup;
        action.Duration = duration;
        action.CooldownDelay = cooldown;
        action.Angle = angle;
        action.EnergyCost = energyCost;
        action.HPCost = hpCost;

        if ( nodeBehaviour == null )
        {
            nodeBehaviour = new List<NodeBehaviour>();
        }
        if ( collisionImpactBehaviours == null )
        {
            collisionImpactBehaviours = new List<CollisionBehaviour>();
        }
        if ( collisionDeflectBehaviours == null )
        {
            collisionDeflectBehaviours = new List<CollisionBehaviour>();
        }
        if ( selectionFilters == null )
        {
            selectionFilters = new List<SelectionFilter>();
        }

        nodeBehaviour
            .ForEach( behaviour => action.NodeBehaviours.Add( behaviour ) );
        collisionImpactBehaviours
            .ForEach( behaviour => action.CollisionImpactBehaviours.Add( behaviour ) );
        collisionDeflectBehaviours
            .ForEach( behaviour => action.CollisionDeflectBehaviours.Add( behaviour ) );
        selectionFilters
            .ForEach( behaviour => action.SelectionFilters.Add( behaviour ) );

        action.name = action.ToString();

        return action;
    }

    public static NodeAction GetRandomAction()
    {
        throw new NotImplementedException();
    }

    public static CreateNodeAction CreateNode()
    {
        return (CreateNodeAction)Object.Instantiate( GameManager.Instance.CreateNodeActionPrefab );
    }

    public static GrowAction Grow()
    {
        return (GrowAction)Object.Instantiate( GameManager.Instance.GrowActionPrefab );
    }

    public static NodeAction MachineGun()
    {
        GameManager GM = GameManager.Instance;

        return GetAction(
            GM.BaseActionPrefab,
            1f,
            3f,
            1f,
            10f,
            50,
            0,
            new List<NodeBehaviour>()
            {
                new MachineGunShootingBehaviour(),
                new PingPongAimingBehaviour()
            },
            new List<CollisionBehaviour>()
            {
                new CollisionDamageBehaviour()
            }
        );
    }
}
