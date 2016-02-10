using System;
using System.Collections;﻿
using System.Collections.Generic;﻿
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CreditsStateProvider : StateProvider
{
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        // TODO Show credits UI canvas
    }

    public override void OnControlEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlEnter( animator, stateInfo, layerIndex );
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateUpdate( animator, stateInfo, layerIndex );
    }

    public override void OnControlUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlUpdate( animator, stateInfo, layerIndex );
    }

    public override void OnControlExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnControlExit( animator, stateInfo, layerIndex );
    }

    public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateExit( animator, stateInfo, layerIndex );

        // TODO Hide credits UI canvas
    }
}
