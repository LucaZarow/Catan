using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
	//Dictionary to keep track of all terrain hexes on the board.
	private Dictionary<Tuple<int, int>, TerrainHex> m_terrainHexes = new Dictionary<Tuple<int,int>,TerrainHex>( );

    //List of all intersections on the board, in no guaranteed order.
    private List<Intersection> m_intersections = new List<Intersection>( );

	//List of all edges on the board, in no guaranteed order.
    private List<Edge> m_edges = new List<Edge>( );

	// Use this for initialization
	void Start( )
    {
        makeBoard( );

        foreach( KeyValuePair<Tuple<int, int>, TerrainHex> kvPair in m_terrainHexes )
        {
            foreach( Edge edge in kvPair.Value.m_edges )
            {

            }

            foreach( Intersection intersection in kvPair.Value.m_intersections )
            {

            }
        }
	}

	public List<Intersection> GetLocalPlayerSettlementLocations() {
		List<Intersection> output = new List<Intersection>();
		foreach (Intersection intersection in m_intersections) 
		{
			if (intersection.piece != null && (intersection.piece is Town) && intersection.town.player.isLocalPlayer) {
				output.Add (intersection);
			}
		}

		return output;
	}

    //This method returns all of the intersections that obey the distance rule.
    //It does NOT check if the intersections are on any trade route networks.
    public List<Intersection> GetSetupSettlementLocations()
    {
        List<Intersection> output = new List<Intersection>();

        foreach( Intersection intersection in m_intersections )
        {
            if( intersection.CanPlaceSetupSettlement( ) )
            {
                output.Add( intersection );
            }
        }

        return output;
    }

	// method that returns all intersections where a settlement can be built
	// checks for distance rule and the intersection is next to at least one of the player's own roads
	public List<Intersection> GetSettlementLocations()
	{
		List<Intersection> output = new List<Intersection>();

		foreach( Intersection intersection in m_intersections )
		{
			if( intersection.CanPlaceSettlement( ) )
			{
				output.Add( intersection );
			}
		}

		return output;
	}

	// method that returns all edges where a road can be built
	public List<Edge> GetRoadLocations() 
	{
		List<Edge> output = new List<Edge> ();

		foreach (Edge edge in m_edges) {
			if (edge.CanPlaceRoad ()) {
				output.Add (edge);
			}
		}

		return output;
	}

	// method that returns the intersection which has world position equal to the given position 
	// returns null if the given position does not match the world position of any intersection
	public Intersection GetIntersectionAtPosition(Vector3 position) {
		foreach (Intersection intersection in m_intersections) {
			if (intersection.GetWorldPosition() == position) {
				return intersection;
			}
		}
		return null;
	}

	// method that returns the edge which has world position equal to the given position
	// returns null if the given position does not match the world position of any intersection
	public Edge GetEdgeAtPosition(Vector3 position) {
		foreach (Edge edge in m_edges) {
			if (edge.GetWorldPosition () == position) {
				return edge;
			}
		}
		return null;
	}

    private void makeBoard( )
    {
        //The goal of this function is to iterate over the existing TerrainHex
        //objects that make up the board, and create the appropriate edge and 
        //intersection objects between them.

        // Iterate over all the children of the board (the terrain hexes)
        foreach( Transform child in transform )
        {
            //Get the TerrainHex object that is attached to the GameObject.
            TerrainHex currentHex = child.gameObject.GetComponent<TerrainHex>( );

            //Add the hex to the dictionary of hexes as supplied
            Tuple<int, int> hexPosition = new Tuple<int, int>( currentHex.m_CoordinateX
                                                             , currentHex.m_CoordinateY );
            m_terrainHexes.Add( hexPosition
                              , currentHex );

            for( int directionIndex = 0; directionIndex < TerrainHex.Directions.Count; directionIndex++ )
            {
                //Attempt to find an already initialized hex in the direction.
                Tuple<int, int> direction = TerrainHex.Directions[directionIndex];
                Tuple<int, int> adjHexPosition = new Tuple<int, int>( hexPosition.Item1 + direction.Item1
                                                                    , hexPosition.Item2 + direction.Item2 );

                if( m_terrainHexes.ContainsKey(adjHexPosition) )
                {
                    TerrainHex adjHex = m_terrainHexes[adjHexPosition];

                    //We found an adjacent hex in our dictionary.
                    //This hex must have already made edges and vertices that
                    //will end up between it and currentHex. We need to get
                    //references to these objects and attach them.

                    //These indices will point to the intersections we want from the adjHex.
                    int revDirectionIndex = (directionIndex + 3) % 6;
                    int revCcwDirectionIndex = TerrainHex.GetCcwDirectionIndex( revDirectionIndex );

                    //Naming scheme is their direction in relation to direction.
                    Intersection ccwIntersection = adjHex.GetIntersection( revDirectionIndex );
                    Edge edge = adjHex.GetEdge( revDirectionIndex );
                    Intersection cwIntersection = adjHex.GetIntersection( revCcwDirectionIndex );

                    //Add associations
                    currentHex.AttachIntersection( TerrainHex.GetCcwDirectionIndex( directionIndex ), ccwIntersection );
                    currentHex.AttachEdge( directionIndex, edge );
                    currentHex.AttachIntersection( directionIndex, cwIntersection );

                    ccwIntersection.AttachHex( currentHex );
                    edge.AttachHex( currentHex );
                    cwIntersection.AttachHex( currentHex );
                }
                else
                {
                    //We did not find an adjacent hex in our dictionary.
                    //We must create a new edge, but other hexes adjacent to
                    //currentHex may have made intersections. We attempt to
                    //find these intersections first, and then create them if
                    //we cannot find them. Finally, attach them all to
                    //currentHex.

                    //Create the new edge that for this side of the hex.
                    Edge newEdge = new Edge( );

                    //Keep track of all created edges.
                    m_edges.Add( newEdge );

                    //Create the association between the edge and this hex.
                    currentHex.AttachEdge( directionIndex, newEdge );
                    newEdge.AttachHex( currentHex );

                    //Check for the existence of the intersection CCW to this edge.
                    //
                    //We already know the hex at adjHexPosition doesn't exist, 
                    //so it can't hold a reference to the intersection we're looking for
                    //
                    //However, we also need to check the hex located in the ccwDirection
                    //to see if it has a reference to the intersection we're looking for.
                    int ccwDirectionIndex = TerrainHex.GetCcwDirectionIndex( directionIndex );
                    Intersection ccwIntersection = currentHex.GetIntersection( ccwDirectionIndex );

                    if( ccwIntersection == null )
                    {
                        //We don't find it on our hex, check the hex in the ccwDirection.
                        Tuple<int, int> ccwDirection = TerrainHex.Directions[ccwDirectionIndex];
                        Tuple<int, int> ccwHexPosition = new Tuple<int, int>( hexPosition.Item1 + ccwDirection.Item1
                                                                            , hexPosition.Item2 + ccwDirection.Item2 );
                        if( m_terrainHexes.ContainsKey( ccwHexPosition ) )
                        {
                            //The CCW hex exists in our map. Since it exists in the map, its intersections array is complete.
                            //Get the intersection we want from it.
                            TerrainHex ccwHex = m_terrainHexes[ccwHexPosition];

                            //We can get the index of the intersection we want on ccwHex by 
                            //reversing the direction index, and then moving one position CCW.
                            int intersectionIndex = TerrainHex.GetCcwDirectionIndex( (ccwDirectionIndex + 3) % 6 );
                            ccwIntersection = ccwHex.GetIntersection( intersectionIndex );
                        }
                        else
                        {
                            //We could not find hexes that would be adajacent to this intersection.
                            //Therefore, we are the first hex to need the intersection, and must create it.
                            //TODO: Somehow get the harbour information for this intersection.
                            ccwIntersection = new Intersection( );

                            //Keep track of all created intersections
                            m_intersections.Add( ccwIntersection );
                        }

                        //Create the association between the intersection and this hex.
                        currentHex.AttachIntersection( ccwDirectionIndex, ccwIntersection );
                        ccwIntersection.AttachHex( currentHex );
                    }

                    //Create the associations between the intersection and the new edge.
                    newEdge.AttachIntersection( ccwIntersection );
                    ccwIntersection.AttachEdge( newEdge );

                    //Check for the existence of the intersection CW to this edge.
                    //
                    //We already know the hex at adjHexPosition doesn't exist, 
                    //so it can't hold a reference to the intersection we're looking for
                    //
                    //However, we also need to check the hex located in the cwDirection
                    //to see if it has a reference to the intersection we're looking for.
                    Intersection cwIntersection = currentHex.GetIntersection( directionIndex );

                    if( cwIntersection == null )
                    {
                        //We don't find it on our hex, check the hex in the cwDirection.
                        int cwDirectionIndex = TerrainHex.GetCwDirectionIndex( directionIndex );
                        Tuple<int, int> cwDirection = TerrainHex.Directions[cwDirectionIndex];
                        Tuple<int, int> cwHexPosition = new Tuple<int, int>( hexPosition.Item1 + cwDirection.Item1
                                                                           , hexPosition.Item2 + cwDirection.Item2 );
                        if( m_terrainHexes.ContainsKey( cwHexPosition ) )
                        {
                            //The CW hex exists in our map. Since it exists in the map, its intersections array is complete.
                            //Get the intersection we want from it.
                            TerrainHex cwHex = m_terrainHexes[cwHexPosition];

                            //We can get the index of the intersection we want on cwHex by 
                            //reversing the direction index.
                            int intersectionIndex = (cwDirectionIndex + 3) % 6;
                            cwIntersection = cwHex.GetIntersection( intersectionIndex );
                        }
                        else
                        {
                            //We could not find hexes that would be adajacent to this intersection.
                            //Therefore, we are the first hex to need the intersection, and must create it.
                            //TODO: Somehow get the harbour information for this intersection.
                            cwIntersection = new Intersection( );

                            //Keep track of all created intersections
                            m_intersections.Add( cwIntersection );
                        }

                        //Create the association between the intersection and this hex.
                        currentHex.AttachIntersection( directionIndex, cwIntersection );
                        cwIntersection.AttachHex( currentHex );
                    }

                    //Create the associations between the intersection and the new edge.
                    newEdge.AttachIntersection( cwIntersection );
                    cwIntersection.AttachEdge( newEdge );
                }
            }
        }
    }

    public List<TerrainHex> GetHexesWithNumber( int number )
    {
        List<TerrainHex> output = new List<TerrainHex>( );
        foreach( KeyValuePair<Tuple<int,int>,TerrainHex> kvPair in m_terrainHexes )
        {
            if( kvPair.Value.diceNumber == number )
            {
                output.Add( kvPair.Value );
            }
        }
        return output;
    }
}

