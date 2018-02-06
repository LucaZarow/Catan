using System.Collections.Generic;
using UnityEngine;

public class Edge
{
	//Piece occupying the edge
	private EdgePiece m_piece = null;

    //Adjacent intersections
    public static readonly int MAX_ADJ_INTERSECTIONS = 2;
    private Intersection[] m_intersections = new Intersection[MAX_ADJ_INTERSECTIONS];

    //Adjacent hexes; 
    //Be sure to check for null, an edge can have 1-2 adjacent hexes.
    public static readonly int MAX_ADJ_HEXES = 2;
    private TerrainHex[] m_hexes = new TerrainHex[MAX_ADJ_HEXES];

    public List<Intersection> GetAdjacentIntersections( )
    {
        return new List<Intersection>( m_intersections );
    }

	// method to check if a road can be placed on this edge by the local player
	public bool CanPlaceRoad() {
		if (m_piece != null) {
			return false;
		} 
		else {
			foreach (Intersection intersection in m_intersections) {
				if (intersection.piece != null && (intersection.piece is Town) && intersection.town.player.isLocalPlayer) {
					return true;
				}
			}

			List<Edge> adjEdges = GetAdjacentEdges ();

			foreach (Edge edge in adjEdges) {
				if (edge.piece != null && (edge.piece is Road) && edge.road.player.isLocalPlayer) {
					Intersection intersection = GetSharedIntersection (edge);
					if (intersection.piece == null || !(intersection.piece is Town)) {
						return true;
					}
				}
			}
		}
		return false;
	}
	
	// method to get the intersection shared between this edge and the given edge
	// returns null if the no shared intersection
	public Intersection GetSharedIntersection(Edge edge) {
		foreach (Intersection intersection1 in this.m_intersections) {
			foreach (Intersection intersection2 in edge.m_intersections) {
				if (intersection1 == intersection2) {
					return intersection1;
				}
			}
		}
		return null;
	}

	// method that returns a list of edges adjacent to this edge
	public List<Edge> GetAdjacentEdges() {
		List<Edge> adjEdges = new List<Edge> ();

		foreach (Intersection intersection in this.m_intersections) {
			foreach (Edge edge in intersection.edges) {
				if (edge != null && edge != this) {
					adjEdges.Add (edge);
				}
			}
		}

		return adjEdges;
	}

    //Get the world position of the center of the edge.
    public Vector3 GetWorldPosition( )
    {
        Vector3 output = new Vector3( );

        for( int i = 0; i < MAX_ADJ_HEXES; i++ )
        {
            if( m_hexes[i] != null )
            {
                output = m_hexes[i].GetEdgeWorldPosition( this );
				break;
            }
        }

        return output;
    }

    //Attach an intersection object.
    public void AttachIntersection( Intersection intersection )
    {
        for( int i = 0; i < MAX_ADJ_INTERSECTIONS; i++ )
        {
            if( ( m_intersections[i] != null )
             && ( m_intersections[i].Equals( intersection ) ) )
            {
                //intersection already attached
                break;
            }
            if( m_intersections[i] == null )
            {
                m_intersections[i] = intersection;
                break;
            }
        }
    }

    //Attach a hex object.
    //Returns false if there are already 2 attached hexes.
    public void AttachHex( TerrainHex hex )
    {
        for( int i = 0; i < MAX_ADJ_HEXES; i++ )
        {
            if( ( m_hexes[i] != null )
             && ( m_hexes[i].Equals( hex ) ) )
            {
                //hex already attached
                break;
            }
            if( m_hexes[i] == null )
            {
                m_hexes[i] = hex;
                break;
            }
        }
    }

	public EdgePiece piece {
		get {
			return this.m_piece;
		}
		set {
			this.m_piece = value;
		}
	}
    
	public Road road
    {
        get
        {
            return (Road)this.m_piece;
        }
        set
        {
            this.m_piece = value;
        }
    }

	public Intersection[] intersections{
		get{ return m_intersections; }
		set{ m_intersections = value; }
	}
}
