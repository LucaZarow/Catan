using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameState : State {

	public GameController owner;

	protected virtual void Awake()
    {
		owner = GameObject.Find( "GameController" ).GetComponent<GameController>( );
	}

    public virtual void DrawGUI( )
    {

    }
}
