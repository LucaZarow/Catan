using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class representing a hex on the board
public class TerrainHex : MonoBehaviour
{
    // A visualization of the hexagonal coordinate system.
    // Note that the "axes" pass through the center of edges.
    //
    //      +y direction
    //         /---\
    //        / 0,1 \
    //   /---\\     //---\
    //  /-1,0 \\---// 1,1 \
    //  \     //---\\     /
    //   \---// 0,0 \\---/
    //   /---\\     //---\
    //  /-1,-1\\---// 1,0 \
    //  \     //---\\     /
    //   \---// 0,-1\\---/ +x direction
    //        \     /
    //         \---/

    // The six basic hexagonal directions
    public static readonly Tuple<int, int> DirectionN = new Tuple<int, int>( 0, 1 );
    public static readonly Tuple<int, int> DirectionNE = new Tuple<int, int>( 1, 1 );
    public static readonly Tuple<int, int> DirectionSE = new Tuple<int, int>( 1, 0 );
    public static readonly Tuple<int, int> DirectionS = new Tuple<int, int>( 0, -1 );
    public static readonly Tuple<int, int> DirectionSW = new Tuple<int, int>( -1, -1 );
    public static readonly Tuple<int, int> DirectionNW = new Tuple<int, int>( -1, 0 );

    // An iterable, ordered list of the directions.
    public static readonly List<Tuple<int, int>> Directions = new List<Tuple<int, int>> { DirectionN
                                                                                        , DirectionNE
                                                                                        , DirectionSE
                                                                                        , DirectionS
                                                                                        , DirectionSW
                                                                                        , DirectionNW };

    // Gets the direction that is counter clockwise to the direction indicated 
    //by the supplied index. For example, supplying a 1 (which indicates the
    //direction NE) will return 0 (which indicates the direction N).
    //Note that direction indexes range from 0 to 5.
    public static int GetCcwDirectionIndex( int hexDirectionIndex )
    {
        int output = hexDirectionIndex - 1;

        if( output < 0 )
        {
            output = 5;
        }

        return output;
    }

    public static int GetCwDirectionIndex( int hexDirectionIndex )
    {
        int output = hexDirectionIndex + 1;

        if( output > 5 )
        {
            output = 0;
        }

        return output;
    }

    // set the outer radius of hexagon to any value
    public const float outerRadius = 1.0f;

    // inner radius of hexagon can be calculated from the outer radius
    public const float innerRadius = outerRadius * 0.866025404f;

    //These are public ints, instead of a Tuple<int,int>, so that we can edit their values in the editor for the board prefab.
    //We may want to change this to a Tuple<int,int> in the future when we are generating boards during runtime.
    public int m_CoordinateX;
    public int m_CoordinateY;

    // A visualization of the stored intersection indexes.
    //        0
    //    5 /---\ 1
    //     /     \
    //     \     /
    //    4 \---/ 2
    //        3
    public static readonly int MAX_ADJ_INTERSECTIONS = 6;
    public Intersection[] m_intersections = new Intersection[MAX_ADJ_INTERSECTIONS];

    // A visualization of the stored intersection indexes.
    //      5   0
    //      /---\ 
    //    4/     \1
    //     \     /
    //      \---/ 
    //      3   2
    public static readonly int MAX_ADJ_EDGES = 6;
    public Edge[] m_edges = new Edge[MAX_ADJ_EDGES];

	// the kind of terrain that the hex has, indicated by an enum
	public TerrainKind terrainKind;

	// the number on the hex
	public int diceNumber;

    //Returns the world position of the center of the terrain hex
    public Vector3 GetWorldPosition( )
    {
        //TODO: Confirm that this is the center of the hexagon
        return this.gameObject.transform.position;
    }

    //Returns the world position of the passed adjacent intersection
    //Returns the TerrainHex's center position if the intersection passed is
    //not adjacent to the TerrainHex.
    public Vector3 GetIntersectionWorldPosition( Intersection intersection )
    {
        Vector3 output = GetWorldPosition( );

        //We iterate over our stored intersections and try to find the passed
        //intersection object. We can then determine what angle the intersection
        //is at, given that 0 degrees represents the unit vector (0,1).
        //Since there are 60 degrees between intersections in a hexagon, we can
        //determine the angle of a given intersection index by the following formula:
        //  angle = 30 + 60 * index
        for( int i = 0; i < MAX_ADJ_INTERSECTIONS; i++ )
        {
            if( m_intersections[i].Equals( intersection ) )
            {
                int angle = 30 + 60 * i;
                Vector3 distanceToIntersection = Vector3.up * TerrainHex.outerRadius;
                //We want a quaternion that will rotate our Vector3 angle degrees about the z-axis.
                //Vector3.back is going out the screen, 
                //and rotation follows the left hand rule,
                //so it looks like a CCW rotation as you look at the screen.
                Quaternion rotation = Quaternion.AngleAxis( angle, Vector3.back );
                output += rotation * distanceToIntersection;
                break;
            }
        }

        return output;
    }

    //Returns the world position of the passed adjacent intersection
    //Returns the TerrainHex's center position if the intersection passed is
    //not adjacent to the TerrainHex.
    public Vector3 GetEdgeWorldPosition( Edge edge )
    {
        Vector3 output = GetWorldPosition( );

        //We iterate over our stored edges and try to find the passed edge
        //object. We can then determine what angle the edge is at, given that
        //0 degrees represents the unit vector (0,1). Since there are 60
        //degrees between intersections in a hexagon, we can determine the
        //angle of a given edge index by the following formula:
        //  angle = 60 * index
        for( int i = 0; i < MAX_ADJ_EDGES; i++ )
        {
            if( m_edges[i].Equals( edge ) )
            {
                int angle = 60 * i;
                Vector3 distanceToIntersection = Vector3.up * TerrainHex.innerRadius;
                //We want a quaternion that will rotate our Vector3 angle degrees about the z-axis.
                //Vector3.back is going out the screen, 
                //and rotation follows the left hand rule,
                //so it looks like a CCW rotation as you look at the screen.
                Quaternion rotation = Quaternion.AngleAxis( angle, Vector3.back );
                output += rotation * distanceToIntersection;
                break;
            }
        }

        return output;
    }

    //Get the intersection in the direction specified by the supplied index.
    public Intersection GetIntersection( int hexDirectionIndex )
    {
        return m_intersections[hexDirectionIndex];
    }

    //Get the edge in the direction specified by the supplied index.
    public Edge GetEdge( int hexDirectionIndex )
    {
        return m_edges[hexDirectionIndex];
    }

    //Attach the supplied intersection in the direction specified by the supplied index.
    public void AttachIntersection( int hexDirectionIndex, Intersection intersection )
    {
        m_intersections[hexDirectionIndex] = intersection;
    }

    //Attach the supplied edge in the direction specified by the supplied index.
    public void AttachEdge( int hexDirectionIndex, Edge edge )
    {
        m_edges[hexDirectionIndex] = edge;
    }
}

// enumeration for the kind of terrain the terrainhex is
public enum TerrainKind {
	Pasture,
	Forest,
	Mountain,
	Hill,
	Field,
	GoldMine,
	Sea,
	Desert
};
