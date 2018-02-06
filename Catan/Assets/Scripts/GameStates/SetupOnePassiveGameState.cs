using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for handling the local player off turn
public class SetupOnePassiveGameState : BaseGameState
{
    public override void Enter( )
    {
        base.Enter( );
        DebugConsole.Log( "Enter Game State Passive" );
        DebugConsole.Log( "Off Turn" );
    }

    public override void DrawGUI( )
    {
        base.DrawGUI( );

        //The lcoal player is not the active player.
        GUI.Label( new Rect( 0, 0, 200, 20 ), "Waiting for other player actions" );
    }
}
