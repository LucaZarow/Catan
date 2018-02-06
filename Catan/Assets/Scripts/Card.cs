using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {

	private ResourceType Type; //enum property

	public Card(ResourceType t){
		Type = t;
	}
	public ResourceType type { //getter and setter
		get{ return Type; }
		set { Type = value; }
	}
}

public enum ResourceType {
	Brick, Grain, Lumber, Ore, Wool, Nothing
};
