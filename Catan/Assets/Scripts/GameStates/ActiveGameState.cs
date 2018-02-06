using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class for handling the local player's turn
public class ActiveGameState : BaseGameState
{
    private bool diceRolled = false;
	private bool action = false;
	bool trade = false;
	int offer = 0;
	int request = 0;

	bool giveLumber = false;
	bool giveOre = false;
	bool giveGrain = false;
	bool giveBrick = false;
	bool giveWool = false;
	bool giveGold = false;

	bool receiveLumber = false;
	bool receiveOre = false;
	bool receiveGrain = false;
	bool receiveBrick = false;
	bool receiveWool = false;

    public override void Enter( )
    {
        base.Enter( );
        // if it is currently the setup round one phase, call the corresponding method
        DebugConsole.Log( "Enter Game State Active" );
        DebugConsole.Log( "Your Turn" );
        //Main game phase? Anything to do without user input?
    }

    public override void DrawGUI( )
    {
        base.DrawGUI( );
        //Main game phase GUI?

        
		if (!diceRolled) {
			if (GUI.Button (new Rect (20, 20, 200, 20), "Roll dice")) {
				diceRolled = true;
				//dice roll 
				owner.RollDice ();
			}
		} else if (!action && !trade) {
			if (GUI.Button (new Rect (20, 20, 200, 20), "end Turn")) {
				diceRolled = false;
				action = false;
				//end turn
				owner.GetLocalPlayer ().CmdNextPlayer ();
			}
			if (GUI.Button (new Rect (20, 120, 200, 20), "build settlement")) {
				action = true;
				//builds settlement
				owner.SpawnSettlementInteractables ();
			}
			if (GUI.Button (new Rect (20, 140, 200, 20), "upgrade settlement")) {
				action = true;
				//upgrades settlement
				owner.SpawnUpgradeInteractables();
			}
			if (GUI.Button (new Rect (20, 200, 200, 20), "trade")) {
				//trade?
				trade = true;
			}
			if (GUI.Button (new Rect (20, 180, 200, 20), "build road")) {
				action = true;
				//build road
				owner.SpawnRoadInteractables ();
			}
		} else if (!action && trade) {

			if (GUI.Button(new Rect(20, 20, 200, 20), "cancel"))
			{
				trade = false;
			}

			giveLumber = GUI.Toggle(new Rect(20, 60, 100, 20), giveLumber, "Lumber");
			giveOre = GUI.Toggle(new Rect(20, 80, 100, 20), giveOre, "Ore");
			giveWool = GUI.Toggle(new Rect(20, 100, 100, 20), giveWool, "Wool");
			giveGrain = GUI.Toggle(new Rect(20, 120, 100, 20), giveGrain, "Grain");
			giveBrick = GUI.Toggle(new Rect(20, 140, 100, 20), giveBrick, "Brick");
			giveGold = GUI.Toggle(new Rect(20, 160, 100, 20), giveGold, "Gold");

			receiveLumber = GUI.Toggle(new Rect(130, 60, 100, 20), receiveLumber, "Lumber");
			receiveOre = GUI.Toggle(new Rect(130, 80, 100, 20), receiveOre, "Ore");
			receiveWool = GUI.Toggle(new Rect(130, 100, 100, 20), receiveWool, "Wool");
			receiveGrain = GUI.Toggle(new Rect(130, 120, 100, 20), receiveGrain, "Grain");
			receiveBrick = GUI.Toggle(new Rect(130, 140, 100, 20), receiveBrick, "Brick");
			//giveGold = GUI.Toggle(new Rect(20, 160, 100, 20), giveGold, "Gold");

			if (GUI.Button(new Rect(20, 180, 200, 20), "confirm trade"))
			{
				trade = false;

				if (giveLumber) {
					offer = 2;
				}
				else if (giveOre){
					offer = 3;
				}
				else if (giveWool)
				{
					offer = 4;
				}
				else if (giveGrain)
				{
					offer = 1;
				}
				else if (giveBrick)
				{
					offer = 0;
				}
				else if (giveGold)
				{
					offer = 5;
				}


				if (receiveLumber)
				{
					request = 2;
				}
				else if (receiveOre)
				{
					request = 3;
				}
				else if (receiveWool)
				{
					request = 4;
				}
				else if (receiveGrain)
				{
					request = 1;
				}
				else if (receiveBrick)
				{
					request = 0;
				}

				owner.GetLocalPlayer().CmdStartTrade(offer, request);
			}
		} else {
			GUI.Label( new Rect( 10, 10, 200, 20 ), "Performing action" );
		}

    }

	public bool Action {
		get {
			return this.action;
		}
		set {
			this.action = value;
		}
	}
}
