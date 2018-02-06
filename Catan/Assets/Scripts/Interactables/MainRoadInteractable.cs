using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoadInteractable : Interactable {
	public Edge location;

	// when the interactable is clicked by the mouse
	void OnMouseDown( )
	{
		// call the CmdBuildPiece method
		owner.GetLocalPlayer( ).CmdBuildPiece( this.transform.position, "road" );
		owner.DestroyInteractables( );
	}
}
