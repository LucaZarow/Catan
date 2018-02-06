using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInteractable : Interactable {
	public Intersection location;

	// when the interactable is clicked by the mouse
	void OnMouseDown( )
	{
		// call the CmdBuildPiece method
		owner.GetLocalPlayer( ).CmdUpgradeSettlement(this.transform.position);
		owner.DestroyInteractables( );
	}
}
