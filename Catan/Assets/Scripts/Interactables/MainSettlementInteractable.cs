using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettlementInteractable : Interactable {
	public Intersection location;

	// when the interactable is clicked by the mouse
	void OnMouseDown( )
	{
		// call the CmdBuildPiece method
		owner.GetLocalPlayer( ).CmdBuildPiece( this.transform.position, "settlement" );
		owner.DestroyInteractables( );
	}
}
