using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for handling the local player's turn
public class SetupTwoActiveGameState : BaseGameState
{
    public override void Enter( )
    {
        base.Enter( );
        owner.SetupTwo( );
    }

    public override void DrawGUI( )
    {
        base.DrawGUI( );
        GUI.Label( new Rect( 0, 0, 200, 20 ), "Complete Setup Phase Two" );
    }
}
