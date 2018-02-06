using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PreGameState : BaseGameState
{
    public override void Enter( )
    {
        base.Enter( );

        // Match has been created, waiting for players to join.
    }

    public override void DrawGUI( )
    {
        base.DrawGUI( );

        //The host (server) triggers the initialization phase when the match is full.
        if( NetworkServer.active )
        {
            if( NetworkManager.singleton.numPlayers == owner.GetMatchSize( ) )
            {
                if( GUI.Button( new Rect( 20, 20, 200, 20 ), "Initialize Game" ) )
                {
                    //If the server is the last client to initialize, signal to all
                    //of the PlayerControllers to move to the next game phase.
                    owner.GetLocalPlayer( ).RpcStartInitializePhase( );
                }
            }
            else
            {
                GUI.Label( new Rect( 10, 10, 200, 20 ), "Waiting for Full Match" );
            }
        }
    }
}
