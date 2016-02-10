using System;
using System.Collections;﻿
using System.Collections.Generic;﻿
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PlayingStateSubscriber : GameStateSubscriber<PlayingStateProvider>
{
    public override void OnStateEnter()
    {
        Debug.Log( "TIME IS PROGRESSING" );

        Time.timeScale = GameManager.Instance.GameSpeed;
        //GM.Cursor.enabled = true;

        //GM.RequiresAction.AddListener( () => GM.ChangeState( GMState.Action ) );
    }
    
    public override void OnStateUpdate()
    {
        if ( GM.HasPendingNodes )
        {
            GM.RequiresAction.Invoke();
        }
    }
    
    public override void OnStateExit()
    {
        //GM.UnpauseState = Type;

        GM.RequiresAction.RemoveAllListeners();
    }
}
