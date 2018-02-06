using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupOneRoadInteractable : Interactable
{
    public Edge location;

    // when the interactable is clicked by the mouse
    void OnMouseDown( )
    {
        // call the CmdBuildPiece method
        owner.GetLocalPlayer( ).CmdBuildPiece( this.transform.position, "road" );
        owner.DestroyInteractables( );

        // we have to set the setupOneCompleted attribute of the player to true
        // we also have to increment the active player index and check state on all machines (done by CmdNextPlayer)
        PlayerController localPc = owner.GetLocalPlayer( );
        localPc.CmdSetSetupOneDone( );
    }
}
