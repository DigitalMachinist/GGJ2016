using System;
using System.Collections;﻿
using System.Collections.Generic;﻿
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class TitleStateSubscriber : GameStateSubscriber<TitleStateProvider>
{
    UnityAction triggerContinue;

    public override void OnStateEnter()
    {
        Time.timeScale = 0f;
        Cursor.enabled = false;

        ActivateScreen( true );
    }

    public override void OnControlEnter()
    {
        if ( GM.Player1.Gamepad != null )
        {
            // Player 1 must press Start to enter the game.
            // Note: If the triggerContinue function isn't set, assign it now.
            triggerContinue = triggerContinue ?? ( () => StateMachine.Animator.SetTrigger( "Continue" ) );
            GM.Player1.Gamepad.StartButton.Pressed.AddListener( triggerContinue );
        }
    }

    public override void OnControlUpdate()
    {
        if ( Input.GetMouseButtonDown( 0 ) )
        {
            StateMachine.Animator.SetTrigger( "Continue" );
        }
    }

    public override void OnControlExit()
    {
        if ( GM.Player1.Gamepad != null )
        {
            // Player 1 pressing Start no longer has any effect.
            GM.Player1.Gamepad.StartButton.Pressed.RemoveListener( triggerContinue );
        }

        DeactivateScreen();
    }
}
