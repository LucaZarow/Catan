using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*A town has a type and a player that it belongs too. It is placed in an intersection.*/
public class Town : IntersectionPiece {

	private TownKind Type; //kind of town it is
	private PlayerController Player; //the player who owns this town
	private Intersection intersection; // the intersection that the town is on
	//renderer
	public Renderer rend;

	//create the town object, set it to local...
	/*init and toCity replaces the functions of setters... */
	//initialize a town
	public void init(PlayerController p, Intersection intersection){
		Type = TownKind.Settlement;
		this.Player = p;
		this.intersection = intersection;
        rend = GetComponent<Renderer>( );
        intersection.piece = this;

        ColorType color = p.color;
        switch( color )
        { 
            //should have 4 players
            case ColorType.red:
                rend.material.color = Color.red;
                return;
            case ColorType.white:
                rend.material.color = Color.white;
                return;
            case ColorType.yellow:
                rend.material.color = Color.yellow;
                return;
            case ColorType.blue:
                rend.material.color = Color.blue;
                return;
            default:
                return;
        }
    }
    
	//update a town
	public void toCity(){
		Type = TownKind.City;
		// change the height of the object to a city
		transform.localScale += new Vector3(0, 0.025F, 0);
	}
	/*Getters for private fields */

	//get Type
	public TownKind type {
		get{ return Type; }
		set{ Type = value; }
	}
	//get Player
	public PlayerController player {
		get{ return Player; }
		set{ Player = value; }
	}

	public Intersection Intersection {
		get {
			return this.intersection;
		}
		set {
			this.intersection = value;
		}
	}
	//converts terrainKind to resourceType
	public ResourceType converter(TerrainKind terrainKind){
		switch (terrainKind) {
		case TerrainKind.Pasture:
			return ResourceType.Wool;
		case TerrainKind.Forest:
			return ResourceType.Lumber;
		case TerrainKind.Mountain:
			return ResourceType.Ore;
		case TerrainKind.Hill:
			return ResourceType.Brick;
		case TerrainKind.Field: 
			return ResourceType.Grain;
		case TerrainKind.GoldMine:
			return ResourceType.Nothing;
		case TerrainKind.Sea:
			return ResourceType.Nothing;
		case TerrainKind.Desert: 
			return ResourceType.Nothing;
		default: 
			return ResourceType.Nothing;
		}
	}		
}

public enum TownKind {
	Settlement,
	City
};
