using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
	// reference to the game
	private GameController gameController;

	// list of towns(settlements and cities) that the player owns
	private List<Town> towns = new List<Town> ();

	// list of roads that the player owns
	private List<Road> roads = new List<Road> ();
    
	//private colour of the player
	private ColorType colour; 

    [SyncVar]
    public bool gameInitialized = false;

    public bool setupOneCompleted = false;
    public bool setupTwoCompleted = false;

    // Use this for initialization
    void Awake( )
    {
        DebugConsole.Log( "PlayerController::Awake " + this.netId );
        gameController = GameObject.Find( "GameController" ).GetComponent<GameController>( );
		initCards ();
    }

    // Use this for initialization
    void Start( )
    {
        DebugConsole.Log( "PlayerController::Start " + this.netId );
        gameController.AddConnectedPlayer( this );
    }

	[Command]
	public void CmdUpgradeSettlement(Vector3 position) {
		RpcUpgradeSettlement (position);
	}

	[ClientRpc]
	public void RpcUpgradeSettlement(Vector3 position) {
		gameController.UpgradeSettlement (position);
	}

	[Command]
	public void CmdStartTrade(int a, int b)
	{
		RpcStartTrade(a, b);
	}

	[ClientRpc]
	public void RpcStartTrade(int a, int b)
	{
		gameController.startTrade(a, b, this);
	}

	[Command]
	public void CmdRoundTwoAssignResources() {
		RpcRoundTwoAssignResources ();
	}

	[ClientRpc]
	public void RpcRoundTwoAssignResources() {
		gameController.RoundTwoAssignResources (this);
	}

    [ClientRpc]
    public void RpcSetPlayerOrder( int[] playerOrder )
    {
        gameController.SetPlayerOrder( playerOrder );
    }

    [Command]
    public void CmdBuildPiece( Vector3 position, String piece )
    {
        RpcBuildPiece( position, piece );
    }

    [ClientRpc]
    public void RpcBuildPiece( Vector3 position, String piece )
    {
        gameController.BuildPiece( position, piece, this );
    }
    
    [ClientRpc]
    public void RpcStartInitializePhase( )
    {
        DebugConsole.Log( "PlayerController::RpcStartInitializePhase " + netId );
        gameController.ChangeState<InitializeGameState>( );
    }

    [Command]
    public void CmdSetInitialized( )
    {
        gameInitialized = true;
    }

    [ClientRpc]
    public void RpcStartSetupOnePhase( )
    {
        DebugConsole.Log( "PlayerController::RpcStartSetupOnePhase " + netId );
        if( gameController.IsActivePlayer( gameController.GetLocalPlayer( ) ) )
        {
            gameController.ChangeState<SetupOneActiveGameState>( );
        }
        else
        {
            gameController.ChangeState<SetupOnePassiveGameState>( );
        }
    }

    [Command]
    public void CmdSetSetupOneDone( )
    {
        RpcSetSetupOneDone( );
    }

    //"this" player object finished their setup one phase.
    [ClientRpc]
    public void RpcSetSetupOneDone( )
    {
        setupOneCompleted = true;
        
        if( gameController.AllPlayersSetupOneCompleted( ) )
        {
            if( gameController.IsActivePlayer( gameController.GetLocalPlayer( ) ) )
            {
                gameController.ChangeState<SetupTwoActiveGameState>( );
            }
            else
            {
                gameController.ChangeState<SetupTwoPassiveGameState>( );
            }
        }
        else
        {
            //Move to the next player
            gameController.incrementActivePlayerIndex( );
            gameController.PassToNextPlayer( );
        }
    }

    [Command]
    public void CmdSetSetupTwoDone( )
    {
        RpcSetSetupTwoDone( );
    }

    //"this" player object finished their setup two phase.
    [ClientRpc]
    public void RpcSetSetupTwoDone( )
    {
        setupTwoCompleted = true;
        
        if( gameController.AllPlayersSetupTwoCompleted( ) )
        {
            if( gameController.IsActivePlayer( gameController.GetLocalPlayer( ) ) )
            {
                gameController.ChangeState<ActiveGameState>( );
            }
            else
            {
                gameController.ChangeState<PassiveGameState>( );
            }
        }
        else
        {
            //Setup two requires the players place their second settlements in reverse.
            gameController.decrementActivePlayerIndex( );
            gameController.PassToNextPlayer( );
        }
    }

    [Command]
    public void CmdNextPlayer( )
    {
        RpcNextPlayer( );
    }

    [ClientRpc]
    public void RpcNextPlayer( )
    {
        gameController.incrementActivePlayerIndex( );
        gameController.PassToNextPlayer( );
    }

    [Command]
    public void CmdPrevPlayer( )
    {
        RpcPrevPlayer( );
    }

    [ClientRpc]
    public void RpcPrevPlayer( )
    {
        gameController.decrementActivePlayerIndex( );
        gameController.PassToNextPlayer( );
    }

    [Command]
    public void CmdInformDiceRoll( int red, int yellow )
    {
        RpcInformDiceRoll( red, yellow );
    }

    [ClientRpc]
    public void RpcInformDiceRoll( int red, int yellow )
    {
        gameController.FinishDiceRoll( red, yellow );
    }

	// called when a Player is instantiated by NetworkManager
	public override void OnStartClient()
	{
		base.OnStartClient ();
		DebugConsole.Log( "PlayerController::OnStartClient " + this.netId );
	}

	// called when the local player is instantiated by NetworkManager
	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();
		DebugConsole.Log( "PlayerController::OnStartLocalPlayer " + this.netId );
	}

	public override void OnStartServer( )
	{
		base.OnStartServer( );
		DebugConsole.Log( "PlayerController::OnStartServer " + this.netId );
	}

	// invoked on clients when the server has caused this object to be destroyed
	public override void OnNetworkDestroy( )
	{
		base.OnNetworkDestroy( );
		DebugConsole.Log( "PlayerController::OnNetworkDestroy " + this.netId );
        DebugConsole.Log( "Players in match: " + NetworkManager.singleton.numPlayers );
        gameController.ResetGame( );
        gameController.ChangeState<PreGameState>( );
    }

	//Dictionary for cards
	private Dictionary<Card, int> Hand = new Dictionary<Card, int> ();
	//initialize the cards in a player's hand
	public void initCards(){
		Card Brick = new Card (ResourceType.Brick);
		Card Grain = new Card (ResourceType.Grain);
		Card Lumber = new Card (ResourceType.Lumber);
		Card Ore = new Card (ResourceType.Ore);
		Card Wool = new Card (ResourceType.Wool);
		Hand.Add (Brick, 0);
		Hand.Add (Grain, 0);
		Hand.Add (Lumber, 0);
		Hand.Add (Ore, 0);
		Hand.Add (Wool, 0);
		return;
	}
	//returns all the cards in the hand
	public Dictionary<Card, int> hand{
		get{ return Hand; }
		set{ Hand = value; }
	}
	//change the number of cards in hand
	public void changeCardInHand(ResourceType t, int number){
		List<Card> keys = new List<Card> (Hand.Keys);
		foreach (Card key in keys) {
			if (key.type == t) {
				int value = Hand[key] + number;
				Hand[key] = value;
			}
		}
	}

	//get number of certain card
	public int getAmountInHand(ResourceType t){
		foreach (KeyValuePair<Card, int> pair in hand) {
			if (pair.Key.type == t) {
				return pair.Value;
			}
		}
		return 0;   //shouldn't be called
	}

	// getters and setters

	// add a town to the list of towns that the player owns
	public void AddTown(Town town) {
		towns.Add (town);
	}

	// add a road to the list of roads that the player owns
	public void AddRoad(Road road) {
		roads.Add (road);
	}

	public List<Town> Towns {
		get {
			return this.towns;
		}
		set {
			this.towns = value;
		}
	}

	//change and get color
	public ColorType color{
		get{ return colour; }
		set{ colour = value; }
	}
}

public enum ColorType{
	red, white, yellow, blue
};
	