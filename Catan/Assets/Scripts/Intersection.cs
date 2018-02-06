using System.Collections.Generic;
using UnityEngine;

public class Intersection
{
	//Piece occupying the intersection.
	private IntersectionPiece m_piece = null;

	//Adjacent edges; 
	//Be sure to check for null, an intersection can have 2-3 adjacent edges.
	public static readonly int MAX_ADJ_EDGES = 3;
	private Edge[] m_edges = new Edge[MAX_ADJ_EDGES];

	//Adjacent hexes;
	//Be sure to check for null, an intersection can have 1-3 adjacent hexes.
	public static readonly int MAX_ADJ_HEXES = 3;
	private TerrainHex[] m_hexes = new TerrainHex[MAX_ADJ_HEXES];

	//Get the world position of the center of the intersection
	public Vector3 GetWorldPosition( )
	{
		Vector3 output = new Vector3();

		for( int i = 0; i < MAX_ADJ_HEXES; i++ )
		{
			if( m_hexes[i] != null )
			{
				output = m_hexes[i].GetIntersectionWorldPosition( this );
				break;
			}
		}

		return output;
	}

	public bool HasSettlement( )
	{
		bool output = false;

		if( m_piece is Town )
		{
			output = true;
		}

		return output;
	}

	//This method checks the adjacent intersections for settlements.
	//If any have a settlement, then you cannot place a settlement on this intersection
	//See the Settlers of Catan distance rule.
	public bool CanPlaceSetupSettlement( )
	{
		bool output = true;

		if( m_piece != null )
		{
			output = false;
		}
		else
		{
			List<Intersection> adjIntersections = GetAdjacentIntersections( );

			foreach( Intersection intersection in adjIntersections )
			{
				if( intersection.HasSettlement( ) )
				{
					output = false;
					break;
				}
			}
		}

		return output;            
	}

	//This method checks if a settlement can be placed on this intersection by the local player
	public bool CanPlaceSettlement( )
	{

		if( m_piece != null )
		{
			return false;
		}
		else
		{
			List<Intersection> adjIntersections = GetAdjacentIntersections( );

			foreach( Intersection intersection in adjIntersections )
			{
				if( intersection.HasSettlement( ) )
				{
					return false;
				}
			}

			foreach (Edge edge in m_edges) 
			{
				if (edge != null) 
				{
					if (edge.piece != null && (edge.piece is Road) && edge.road.player.isLocalPlayer) 
					{
						return true;
					}
				}
			}
		}

		return false; 
	}

	public List<Intersection> GetAdjacentIntersections( )
	{
		List<Intersection> adjIntersections = new List<Intersection>( );

		foreach( Edge edge in m_edges )
		{
			if (edge == null) {
				continue;
			}
			List<Intersection> edgeIntersections = edge.GetAdjacentIntersections( );
			foreach( Intersection edgeIntersection in edgeIntersections )
			{
				//The edge will have 2 intersections adjacent to it. One of them is this,
				//we want to return the other one as adjacent to this one.
				if( !edgeIntersection.Equals( this ) )
				{
					adjIntersections.Add( edgeIntersection );
				}
			}
		}

		return adjIntersections;
	}

	//Attach an edge object.
	//Returns false if there are already 3 attached edges.
	public void AttachEdge( Edge edge )
	{
		for( int i = 0; i < MAX_ADJ_EDGES; i++ )
		{
			if( ( m_edges[i] != null ) 
				&& ( m_edges[i].Equals( edge ) ) )
			{
				//Edge is already attached.
				break;
			}
			else if( m_edges[i] == null )
			{
				//There is space to attach the edge.
				m_edges[i] = edge;
				break;
			}
		}
	}

	//Attach a hex object.
	//Returns false if there are already 3 attached hexes.
	public void AttachHex( TerrainHex hex )
	{
		for( int i = 0; i < MAX_ADJ_HEXES; i++ )
		{
			if( ( m_hexes[i] != null ) 
				&& ( m_hexes[i].Equals( hex ) ) )
			{
				//Hex is already attached.
				break;
			}
			else if( m_hexes[i] == null )
			{
				m_hexes[i] = hex;
				break;
			}
		}
	}

	public IntersectionPiece piece {
		get {
			return this.m_piece;
		}
		set {
			this.m_piece = value;
		}
	}

	public Edge[] edges {
		get {
			return this.m_edges;
		}
		set {
			this.m_edges = value;
		}
	}

	public Town town {
		get {
			return (Town)this.m_piece;
		}
		set {
			this.m_piece = value;
		}
	}

	public TerrainHex[] hexes {
		get {
			return this.m_hexes;
		}
		set {
			this.m_hexes = value;
		}
	}
}
