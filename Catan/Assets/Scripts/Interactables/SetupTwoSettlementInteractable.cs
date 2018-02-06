﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupTwoSettlementInteractable : Interactable
{
    public Intersection location = null;
    public SetupTwoRoadInteractable roadInteractablePrefab;

    // when the interactable is clicked by the mouse
    void OnMouseDown( )
    {
        // call the CmdBuildPiece method
        owner.GetLocalPlayer( ).CmdBuildPiece( this.transform.position, "settlement" );
        owner.DestroyInteractables( );

		// assign resources
		owner.GetLocalPlayer().CmdRoundTwoAssignResources();

        // if the current phase is setup round one and the player who built the settlement is the local player
        // we have to continue setup round one for the player by instantiating the road interactables
        Edge[] availableEdges = location.edges;
        foreach( Edge edge in availableEdges )
        {
            if( (edge != null) && (edge.piece == null) )
            {
                SetupTwoRoadInteractable interactable = Instantiate( roadInteractablePrefab
																   , edge.GetWorldPosition( )
                                                                   , Quaternion.identity );
                interactable.location = edge;
                owner.AddInteractable( interactable );
            }
        }
    }
}
