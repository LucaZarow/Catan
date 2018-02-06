using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : StateMachine
{
    // *Note* //
    // currently the board is premade, so is stored as a prefab
    // reference to the board prefab
    public Board boardPrefab;

	public SetupOneSettlementInteractable settlementOneInteractablePrefab;

    public SetupTwoSettlementInteractable settlementTwoInteractablePrefab;

	public MainSettlementInteractable settlementMainInteractablePrefab;

	public MainRoadInteractable roadMainInteractablePrefab;

	public UpgradeInteractable upgradeInteractablePrefab;

    public Town townPrefab;

	public Road roadPrefab;

    // reference to the board gameobject
    private Board board;
    
    // The number of players we're expecting to join the match.
    private int matchSize = 2;
    
    private List<PlayerController> players = new List<PlayerController>( );
    private int activePlayerIndex = 0;

    private List<Interactable> currentInteractables = new List<Interactable>( );

    //Dictionary for cards
    private Dictionary<Card, int> Deck = new Dictionary<Card, int>( );

    //Dice values, for displaying the last roll.
    public int redDiceValue;
    public int yellowDiceValue;

	public Text resourceText;
	public Text diceText;

    public static List<PlayerController> GetConnectedPlayers( )
    {
        List<PlayerController> output = new List<PlayerController>( );
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag( "Player" );
         
        foreach( GameObject gameObject in gameObjects )
        {
            output.Add( gameObject.GetComponent<PlayerController>( ) );
        }

        return output;
    }

    // Locate the PlayerController object that represents this client's player
    // Returns null if it cannot be found.
    public PlayerController GetLocalPlayer( )
    {
        foreach( PlayerController player in players )
        {
            if( player.isLocalPlayer )
            {
                return player;
            }
        }
        return null;
    }

    public bool IsActivePlayer( PlayerController player )
    {
        return players[activePlayerIndex].Equals( player );
    }

    public bool AllPlayersInitialized( )
    {
        foreach( PlayerController player in players )
        {
            if( !player.gameInitialized )
            {
                return false;
            }
        }
        return true;
    }

    public bool AllPlayersSetupOneCompleted( )
    {
        foreach( PlayerController player in players )
        {
            if( !player.setupOneCompleted )
            {
                return false;
            }
        }
        return true;
    }

    public bool AllPlayersSetupTwoCompleted( )
    {
        foreach( PlayerController player in players )
        {
            if( !player.setupTwoCompleted )
            {
                return false;
            }
        }
        return true;
    }

    // Use this for initialization
    void Start ()
    {
        ChangeState<PreGameState>( );
	}

    // Draw state-specific GUI
    private void OnGUI( )
    {
        (CurrentState as BaseGameState).DrawGUI( );
    }

    public void InitializeGame( )
    {
        board = Instantiate( boardPrefab, Vector3.zero, boardPrefab.transform.rotation ) as Board;
		CreateCards ();
    }

    public void ResetGame( )
    {
        // Destroy board
        if( board != null )
        {
            Destroy( board.gameObject );
            DebugConsole.Log( "Board Destroyed" );
        }
    }

    // method for setup round one
    public void SetupOne( )
    {
        DebugConsole.Log( "Pick a location to place your settlement" );

        // instantiate interactables on the board
        // the interactable that is clicked on will respond
		List<Intersection> availableIntersections = board.GetSetupSettlementLocations();

		foreach (Intersection intersection in availableIntersections)
        {
            SetupOneSettlementInteractable interactable = Instantiate( settlementOneInteractablePrefab
                                                                     , intersection.GetWorldPosition()
                                                                     , Quaternion.identity);
            interactable.location = intersection;
            currentInteractables.Add( interactable );
		}
    }

    // method for setup round two
    public void SetupTwo( )
    {
        DebugConsole.Log( "Pick a location to place your settlement" );

        // instantiate interactables on the board
        // the interactable that is clicked on will respond
        List<Intersection> availableIntersections = board.GetSetupSettlementLocations( );

        foreach( Intersection intersection in availableIntersections )
        {
            SetupTwoSettlementInteractable interactable = Instantiate( settlementTwoInteractablePrefab
                                                                     , intersection.GetWorldPosition( )
                                                                     , Quaternion.identity );
            interactable.location = intersection;
            currentInteractables.Add( interactable );
        }
    }

	// method for instantiating settlement interactables for the main game phase
	public void SpawnSettlementInteractables() {
		List<Intersection> availableIntersections = board.GetSettlementLocations( );

		if (availableIntersections.Count == 0) {
			DebugConsole.Log ("There are no available locations to build settlement");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		if (!CheckEnoughResources ("settlement")) {
			DebugConsole.Log ("You don't have enough resources to build a settlement");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		GetLocalPlayer().changeCardInHand(ResourceType.Brick, -1);
		GetLocalPlayer().changeCardInHand(ResourceType.Lumber, -1);
		GetLocalPlayer().changeCardInHand(ResourceType.Wool, -1);
		GetLocalPlayer().changeCardInHand(ResourceType.Grain, -1);

		SetResourceText ();

		foreach( Intersection intersection in availableIntersections )
		{
			MainSettlementInteractable interactable = Instantiate( settlementMainInteractablePrefab
																	 , intersection.GetWorldPosition( )
																 	 , Quaternion.identity );
			interactable.location = intersection;
			currentInteractables.Add( interactable );
		}
	}

	// method for instantiating road interactables for the main game phase
	public void SpawnRoadInteractables() {
		List<Edge> availableEdges = board.GetRoadLocations( );

		if (availableEdges.Count == 0) {
			DebugConsole.Log ("There are no available locations to build road");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		if (!CheckEnoughResources ("road")) {
			DebugConsole.Log ("You don't have enough resources to build a road");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		GetLocalPlayer().changeCardInHand(ResourceType.Brick, -1);
		GetLocalPlayer().changeCardInHand(ResourceType.Lumber, -1);

		SetResourceText ();

		foreach( Edge edge in availableEdges )
		{
			MainRoadInteractable interactable = Instantiate( roadMainInteractablePrefab
														   , edge.GetWorldPosition( )
														   , Quaternion.identity );
			interactable.location = edge;
			currentInteractables.Add( interactable );
		}
	}

	public void SpawnUpgradeInteractables() {
		List<Intersection> intersections = board.GetLocalPlayerSettlementLocations ();

		if (intersections.Count == 0) {
			DebugConsole.Log ("You have no settlements to upgrade");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		if (!CheckEnoughResources ("upgrade")) {
			DebugConsole.Log ("You don't have enough resources to upgrade a settlement");
			(CurrentState as ActiveGameState).Action = false;
			return;
		}

		GetLocalPlayer().changeCardInHand(ResourceType.Ore, -3);
		GetLocalPlayer().changeCardInHand(ResourceType.Grain, -2);

		SetResourceText ();

		foreach( Intersection intersection in intersections )
		{
			UpgradeInteractable interactable = Instantiate( upgradeInteractablePrefab
														  , intersection.GetWorldPosition( )
														  , Quaternion.identity );
			interactable.location = intersection;
			currentInteractables.Add( interactable );
		}
	}

    public void PassToNextPlayer( )
    {

        if( IsActivePlayer( GetLocalPlayer( ) ) )
        {
            if( CurrentState is SetupOnePassiveGameState
             || CurrentState is SetupOneActiveGameState )
            {
                ChangeState<SetupOneActiveGameState>( );
            }
            else if( CurrentState is SetupTwoPassiveGameState
                  || CurrentState is SetupTwoActiveGameState )
            {
                ChangeState<SetupTwoActiveGameState>( );
            }
            else if( CurrentState is PassiveGameState
                  || CurrentState is ActiveGameState )
            {
                ChangeState<ActiveGameState>( );
            }
        }
        else
        {
            if( CurrentState is SetupOnePassiveGameState
             || CurrentState is SetupOneActiveGameState )
            {
                ChangeState<SetupOnePassiveGameState>( );
            }
            else if( CurrentState is SetupTwoPassiveGameState
                  || CurrentState is SetupTwoActiveGameState )
            {
                ChangeState<SetupTwoPassiveGameState>( );
            }
            else if( CurrentState is PassiveGameState
                  || CurrentState is ActiveGameState )
            {
                ChangeState<PassiveGameState>( );
            }
        }
    }

    // method for building a piece on the board
    public void BuildPiece( Vector3 position, string piece, PlayerController pc )
    {
        if( piece.Equals( "settlement" ) )
        {
            // if the piece to be built is a settlement, instantiate the town prefab
            Town settlement = Instantiate( townPrefab, position, Quaternion.identity );

            // add the newly built settlement to the list of towns on the player that built the settlement
            pc.AddTown( settlement );

            // add a reference to the settlement for the intersection that it was built on
            Intersection intersection = board.GetIntersectionAtPosition( position );

            if( intersection != null )
            {
                intersection.piece = settlement;
                settlement.init( pc, intersection );
            }
            else
            {
                DebugConsole.Log( "BuildPiece Error: Could not find intersection with matching position" );
            }
        }
        else if( piece.Equals( "road" ) )
        {
            // if the piece to be built is a road, instantiate the road prefab
			Road road = Instantiate( roadPrefab, position + new Vector3(0f, 0f, -0.20f), Quaternion.identity ) as Road;

            // add the newly built road to the list of roads on the player that built the road
            pc.AddRoad( road );

            // add a reference to the road for the edge that it was built on
            Edge edge = board.GetEdgeAtPosition( position );

            if( edge != null )
            {
                edge.piece = road;
                road.init( pc, edge );
            }
            else
            {
                DebugConsole.Log( "BuildPiece Error: Could not find edge with matching position" );
            }
        }

		if (pc.isLocalPlayer && (CurrentState is ActiveGameState)) {
			(CurrentState as ActiveGameState).Action = false;
		}
    }

	public void UpgradeSettlement(Vector3 position) {
		Intersection intersection = board.GetIntersectionAtPosition( position );

		if( intersection != null )
		{
			(intersection.piece as Town).toCity ();
		}
		else
		{
			DebugConsole.Log( "BuildPiece Error: Could not find intersection with matching position" );
		}

		if ((CurrentState is ActiveGameState)) {
			(CurrentState as ActiveGameState).Action = false;
		}
	}

    public void AddInteractable( Interactable interactable )
    {
        currentInteractables.Add( interactable );
    }

    public void DestroyInteractables( )
    {
        //Iterate in reverse over the interactable list, removing the item at the end and then destroying it.
        for( int i = currentInteractables.Count - 1; i >= 0; i-- )
        {
            Destroy( currentInteractables[i].gameObject );
            currentInteractables.RemoveAt( i );
        }
    }

	// method for determining which player should go first
	public int[] DeterminePlayerOrder( )
	{
        //TODO: Reorder the server's players list here

        //Order the players in simple array form
        int[] playerOrder = new int[players.Count];

        for( int i = 0; i < players.Count; i++ )
        {
            playerOrder[i] = (int)players[i].netId.Value;
        }

        //Inform all clients of the correct list order
        return playerOrder;
	}

    public void SetPlayerOrder( int[] playerOrder )
    {
        List<PlayerController> newOrder = new List<PlayerController>( );
        for( int i = 0; i < playerOrder.Length; i++ )
        {
            for( int j = 0; j < players.Count; j++ )
            {
                if( players[j].netId.Value == playerOrder[i] )
                {
                    newOrder.Add( players[j] );
                    break;
                }
            }
        }
        activePlayerIndex = 0;
        setColors( newOrder );
        players = newOrder;
    }

	public void setColors(List<PlayerController> Players){
		for (int i = 0; i < Players.Count; i++) {
			switch (i) { //should have 4 players
			case 0: 
				Players[0].color = ColorType.red;
				break;
			case 1: 
				Players[1].color = ColorType.white;
				break;
			case 2:
				Players [2].color = ColorType.yellow;
				break;
			case 3:
				Players [3].color = ColorType.blue;
				break;
			default: 
				break;
			}
		}
	
	}

    public void AddConnectedPlayer( PlayerController player )
    {
        players.Add( player );
    }

    public void RemoveConnectedPlayer( int netId )
    {
        for( int i = 0; i < players.Count; i++ )
        {
            if( players[i].netId.Value == netId )
            {
                players.RemoveAt( i );
            }
        }
    }

    public int GetMatchSize( )
    {
        return matchSize;
    }

    public void incrementActivePlayerIndex( )
    {
        activePlayerIndex = (activePlayerIndex + 1)%matchSize;
    }
    
    public void decrementActivePlayerIndex( )
    {
        activePlayerIndex = (activePlayerIndex + 1) % matchSize;
    }

	//initialize a set of cards
	public void CreateCards(){
		Card Brick = new Card (ResourceType.Brick);
		Card Grain = new Card (ResourceType.Grain);
		Card Lumber = new Card (ResourceType.Lumber);
		Card Ore = new Card (ResourceType.Ore);
		Card Wool = new Card (ResourceType.Wool);
		Deck.Add (Brick, 19);
		Deck.Add (Grain, 19);
		Deck.Add (Lumber, 19);
		Deck.Add (Ore, 19);
		Deck.Add (Wool, 19);
		return;
	}
	//returns all the cards in the deck
	public Dictionary<Card, int> deck{
		get{ return Deck; }
		set{ Deck = value; }
	}
	//change the number of cards in bank
	public void changeCardInDeck(ResourceType t, int number){
		List<Card> keys = new List<Card> (Deck.Keys);
		foreach (Card key in keys) {
			if (key.type == t) {
				int value = Deck[key] + number;
				Deck[key] = value;
			}
		}
	}

    public void RollDice( )
    {
        int red = Random.Range( 1, 6 );
        int yellow = Random.Range( 1, 6 );

        GetLocalPlayer( ).CmdInformDiceRoll( red, yellow );
    }

    public void FinishDiceRoll( int red, int yellow )
    {
        //Store values for displaying to GUI.
        redDiceValue = red;
        yellowDiceValue = yellow;
        int sum = red + yellow;

		SetDiceText ();

        List<TerrainHex> producingHexes = board.GetHexesWithNumber( sum );

        foreach( TerrainHex hex in producingHexes )
        {
            Intersection[] intersections = hex.m_intersections;
            for( int i = 0; i < 6; i++ )
            {
                Intersection intersection = intersections[i];
                if( intersection.HasSettlement( ) )
                {
                    Town town = intersection.town; //get the town
                    PlayerController p = town.player;
                    TownKind type = town.type;
                    TerrainKind terrain = hex.terrainKind;
                    ResourceType cardtype = town.converter( terrain );

                    if( type == TownKind.Settlement )
                    {
                        p.changeCardInHand( cardtype, 1 );
                    }
                    else
                    {
                        p.changeCardInHand( cardtype, 2 );
                    }
                }
            }
        }

		SetResourceText ();
    }

	public void RoundTwoAssignResources(PlayerController pc) {
		foreach (Town town in pc.Towns) {
			TerrainHex[] hexes = town.Intersection.hexes;

			foreach (TerrainHex hex in hexes) {
				if (hex != null) {
					TerrainKind terrain = hex.terrainKind;
					ResourceType cardtype = town.converter (terrain);
					TownKind type = town.type;

					if (type == TownKind.Settlement) {
						pc.changeCardInHand (cardtype, 2);
					} else {
						pc.changeCardInHand (cardtype, 2);
					}
				}
			}
		}

		SetResourceText ();
	}

	public void SetResourceText() {
		PlayerController localPlayer = GetLocalPlayer ();
		List<Card> keys = new List<Card> (localPlayer.hand.Keys);
		string text = "";
		foreach (Card key in keys) {
			text = text + key.type.ToString () + ": " + localPlayer.hand [key].ToString () + " ";
		}
		resourceText.text = text;
	}

	void SetDiceText() {
		diceText.text = "Red: " + redDiceValue.ToString () + " " + "Yellow: " + yellowDiceValue.ToString ();
	}

	bool CheckEnoughResources(string piece) {
		if (piece == "settlement") {
			if (CheckHasResource (ResourceType.Brick, 1) && CheckHasResource (ResourceType.Lumber, 1) && CheckHasResource (ResourceType.Wool, 1) && CheckHasResource (ResourceType.Grain, 1)) {
				return true;
			} else {
				return false;
			}
		} 
		else if (piece == "road") {
			if (CheckHasResource (ResourceType.Brick, 1) && CheckHasResource (ResourceType.Lumber, 1)) {
				return true;
			} else {
				return false;
			}
		} 
		else if (piece == "upgrade") {
			if (CheckHasResource (ResourceType.Ore, 3) && CheckHasResource (ResourceType.Grain, 2)) {
				return true;
			} else {
				return false;
			}
		}
		return false;
	}

	bool CheckHasResource(ResourceType type, int number) {
		PlayerController localPlayer = GetLocalPlayer ();
		List<Card> keys = new List<Card> (localPlayer.hand.Keys);
		foreach (Card key in keys) {
			if (key.type == type && localPlayer.hand [key] >= number) {
				return true;
			}
		}

		return false;
	}

	public void startTrade(int a, int b, PlayerController p){
		Dictionary<Card,Card> offer = new Dictionary<Card, Card>();
		Card aCard;
		Card bCard;
		switch (a)
		{
		case 0:
			aCard = new Card(ResourceType.Brick);
			break;
		case 1:
			aCard = new Card(ResourceType.Grain);
			break;
		case 2:
			aCard = new Card(ResourceType.Lumber);
			break;
		case 3:
			aCard = new Card(ResourceType.Ore);
			break;
		case 4:
			aCard = new Card(ResourceType.Wool);
			break;
		default:
			aCard = new Card(ResourceType.Brick);
			break;
		}

		switch (b)
		{
		case 0:
			bCard = new Card(ResourceType.Brick);
			break;
		case 1:
			bCard = new Card(ResourceType.Grain);
			break;
		case 2:
			bCard = new Card(ResourceType.Lumber);
			break;
		case 3:
			bCard = new Card(ResourceType.Ore);
			break;
		case 4:
			bCard = new Card(ResourceType.Wool);
			break;
		default:
			bCard = new Card(ResourceType.Brick);
			break;
		}
		if (p != players[activePlayerIndex])
			return;

		offer.Add(aCard, bCard);
		MaritimeTrade.Trade(offer, this, players[activePlayerIndex]);
	}

	//get number of certain card
	public int getAmountInDeck(ResourceType t){
		foreach (KeyValuePair<Card, int> pair in deck) {
			if (pair.Key.type == t) {
				return pair.Value;
			}
		}
		return 0; //shouldn't be called
	}

}