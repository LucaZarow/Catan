using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : EdgePiece {
	private PlayerController Player; // player who owns the road
	private Edge edge; // the edge that the road is on
	//renderer
	private Renderer rend ;

    public void init( PlayerController p, Edge e )
    {
        this.Player = p;
        this.edge = e;
        rend = GetComponent<Renderer>( );
        p.AddRoad( this );
        e.piece = this;

        Vector3 init = e.intersections[0].GetWorldPosition( );
        Vector3 to = e.intersections[1].GetWorldPosition( );

        //if the absolute difference of angle in y is not big -> angle 180.
        if( Mathf.Abs( init.y - to.y ) < 0.2 )
        {
            transform.eulerAngles = new Vector3( 0, 0, 90 );
        }
        else
        {
            //if the angle is 60 degrees. i.e. incr x + y OR decr x + y
            if( (init.x < to.x && init.y < to.y) || (init.x > to.x && init.y > to.y) )
            {
                transform.eulerAngles = new Vector3( 0, 0, -30 );
            }
            //if angle is 120 degrees. decr x + incr y OR incr x or decr y
            else
            {
                transform.eulerAngles = new Vector3( 0, 0, 30 );
            }
        }

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
    
	public PlayerController player {
		get{ return Player; }
		set{ Player = value; }
	}


}


