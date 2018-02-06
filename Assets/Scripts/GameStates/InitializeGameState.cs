using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// class for handling the Initialize game phase
public class InitializeGameState : BaseGameState
{
    public override void Enter( )
    {
        base.Enter( );
        DebugConsole.Log( "Enter Game State Initialize" );
        owner.InitializeGame( );
        owner.GetLocalPlayer( ).CmdSetInitialized( );
    }

    public override void DrawGUI( )
    {
        base.DrawGUI( );

        //The host (server) triggers the setup phase once they decide initialization is done.
        if( NetworkServer.active )
        {
            if( owner.AllPlayersInitialized( ) )
            {
                if( GUI.Button( new Rect( 0, 0, 200, 20 ), "Start Match" ) )
                {
                    //If the server is the last client to initialize, signal to all
                    //of the PlayerControllers to move to the next game phase.
                    int[] playerOrder = owner.DeterminePlayerOrder( );
                    owner.GetLocalPlayer( ).RpcSetPlayerOrder( playerOrder );
                    owner.GetLocalPlayer( ).RpcStartSetupOnePhase( );
                }
            }
        }
    }
}
