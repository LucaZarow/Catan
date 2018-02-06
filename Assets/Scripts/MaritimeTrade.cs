using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaritimeTrade : MonoBehaviour{

	//public GameController game; //the game
	//public PlayerController player; //the current player
	//list of card types to trade, with 4 card for 1 card
	//Dictionary<Card,Card> cards = new Dictionary<Card,Card>();
	// Use this for initialization (need to fit GUI)
	void Start () {
		//by GUI, tomorrow...
	}
	//missing an add to cards function for the player to trade from GUI

	//trading function
	public static void Trade(Dictionary<Card,Card> hand, GameController game, PlayerController player){
		foreach(KeyValuePair<Card,Card> exchange in hand){
			Card with = exchange.Key; //with 4 kind of this card type
			Card to = exchange.Value; //exchange to 1 kind of this card type
			ResourceType toExchange = with.type; //the resourcetype we have
			ResourceType toGet = to.type; //the resourcetype we want
			//return card we have to bank
			int handAmount = player.getAmountInHand(toExchange);
			int deckAmount = game.getAmountInDeck(toGet);
			if(handAmount >= 4 && deckAmount >= 1){
				player.changeCardInHand(toExchange, -4); //reduce card we have by 4.
				game.changeCardInDeck(toExchange, 4); //increment card in bank by 4.
				//get the card we want from bank
				player.changeCardInHand(toGet, 1); //increment card we want by 1.
				game.changeCardInDeck(toGet, -1); //decrement card in bank by 1.

				game.SetResourceText ();
			}
		}
	}

}
