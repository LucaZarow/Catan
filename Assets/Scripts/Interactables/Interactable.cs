using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected GameController owner;

    protected virtual void Awake( )
    {
        owner = GameObject.Find( "GameController" ).GetComponent<GameController>( );
    }
}
