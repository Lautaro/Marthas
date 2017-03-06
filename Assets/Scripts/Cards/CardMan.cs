using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardMan : MonoBehaviour
{


	#region
		public static CardMan Me;
	#endregion

		public static List<Texture> AllTextures = new List<Texture> ();
		public Card SelectedCard;
		
		public static bool canCardBeFlipped = true;
		public static int cardMargin = 1; //meters
		public static int cardStartingZ = 0;
		bool unselectCardsNextClick = false;
		

		//Wave 
		public static float cardStartingX = 60;
		public static float topRowY = 39; 
		public static float bottomRowY = 24; 
		public static int cardPairsPerWave = 6;


		void Awake ()
		{
				Me = this;
		}
			

		static public void SetupWave ()
		{
				
				Card.Cards = new List<Card> ();

				canCardBeFlipped = true;
				/* LOAD TEXTURES */
				var objects = Resources.LoadAll ("Cards/");
				foreach (var o in objects) {
						AllTextures.Add (o as Texture);
				}
				/* SETUP CARD WAVES */
				for (int i = 0; i < MainGame.Wave; i++) {
						var wave = new CardGrid (i);
						// creates a wave of cards in pairs with amount cardsPerWave;
				}
		}

		public static Card CreateNewCardPrefab ()
		{
				var prefab = Instantiate (Resources.Load ("Prefabs/CardGame/Card")) as GameObject;
				var card = prefab.GetComponent<Card> ();
				return card;
		}

		public static Texture GetRandomTexture ()
		{
				var texture = (AllTextures [Random.Range (0, AllTextures.Count)]);
				//randomly set cards image
				return texture;
		}



		public void OnCardClicked (Card sender)
		{
				if (canCardBeFlipped) {
						if (unselectCardsNextClick || sender.CurrentSide == Card.FlipsSide.Front) {
								UnselectCards ();
								unselectCardsNextClick = false;
						} else if (sender.CurrentSide == Card.FlipsSide.Back) {
								StartCoroutine (MatchCards_cr (sender));
				
						} 	
				}
		}


		/// <summary>
		/// Checks if both cards flipped are a match and removes them if so.
		/// </summary>
		/// <returns>The cards_cr.</returns>
		/// <param name="cardClicked">Card clicked.</param>
		IEnumerator MatchCards_cr (Card cardClicked)
		{						
				
				canCardBeFlipped = false;
										
				StartCoroutine (cardClicked.FlipToFront_cr ());
										
				if (SelectedCard == null) {
						SelectedCard = cardClicked;

							
				} else {
						if (SelectedCard != null && SelectedCard != cardClicked) {
								
								yield return new WaitForSeconds (0.5F);
								if (SelectedCard.cardName == cardClicked.cardName) {
								
										//REPORT SCORE AND DESTROY CARDS
										SelectedCard.RemoveCardCardAfterDelay ();
										cardClicked.RemoveCardCardAfterDelay ();
										SelectedCard = null;
										CardGui.Me.Score = 100;
										CardScene.GravityStrength -= 0.3f; //Slow speed
										CardGui.Message ("Speed -30", Color.yellow);

																								
										// REALIGN CARDS
										yield return StartCoroutine (RealignCards_cr ());
			
										//Check For Game Win
										if (Card.Cards.Count < 1 && CardScene.cardGameManager.gameState == CardScene.GameState.GameOn) {
												StartCoroutine (CardScene.cardGameManager.GameWon_cr ());
										}
			
								} else {
										SFXMan.sfx_Error.Play ();
										CardGui.Message ("Speed +10", Color.red);
										CardScene.GravityStrength += 0.1f;
										unselectCardsNextClick = true;
								}
						} 
				}
				canCardBeFlipped = true;				
		}

		IEnumerator RealignCards_cr ()
		{
				var allTheCards = AllCardsByHighXPosition ();// Make sure list iterates from right to left
				var topRowCards = new List<Card> ();
				var bottomRowCards = new List<Card> ();

				foreach (var card in allTheCards) {
						if (card.IsTopRow) {
								topRowCards.Add (card);
						} else {
								bottomRowCards.Add (card);
						}
				}

				//A LIST WITH LIST<CARD>. SO BIZZARE!
				var cardLists = new List<List<Card>> ();
				cardLists.Add (topRowCards);
				cardLists.Add (bottomRowCards);

				foreach (var cardList in cardLists) {			

						//Start with seconde card. i=1;
						for (int i = 1; i < cardList.Count; i++) {
								var card = cardList [i];
								var neighbourCard = cardList [i - 1];
								var rightEdge = card.RightEdge ();
								var previousCardLeftEdge = neighbourCard.LeftEdge ();
								var distance = previousCardLeftEdge - rightEdge;
								

								if (distance > cardMargin * 2) {//double cardMargin just to be sure. Sometimes the margin can vary just a little bit.
										//Move card to neighbour									
										card.BlipSelf ();
										
										yield return StartCoroutine (MoveCardToNeighbour_cr (card, neighbourCard));

								}
								var y = card.transform.position.y;
								var z = card.transform.position.z - 10;
								var minX = rightEdge;
								var maxX = previousCardLeftEdge;
								Debug.DrawLine (new Vector3 (minX, y, z), new Vector3 (maxX, y, z), Color.green, 3f, true);

						}
				}
		}

		IEnumerator MoveCardToNeighbour_cr (Card card, Card neighbour)
		{
				var durationSeconds = 0.3F;
				
				var t = Time.deltaTime;

				SFXMan.sfx_Realign.Play ();

				while (t < durationSeconds) {
		
						var cardX = card.transform.position.x;
						var targetX = neighbour.transform.position.x - card.Box ().width - cardMargin; //Distance to position next to neighbour
						var newX = Mathf.Lerp (cardX, targetX, t / durationSeconds);			
						var newY = neighbour.transform.position.y;
						var newZ = neighbour.transform.position.z;
						card.transform.position = new Vector3 (newX, newY, newZ);
						yield return null;
			
						t += Time.deltaTime; // add time for each cycle
				}
				//IMPORTANT! Force the target value cause Slerp will never get 
				var finalX = neighbour.transform.position.x - card.Box ().width - cardMargin; //Distance to position next to neighbour				
				var finalY = neighbour.transform.position.y;
				var finalZ = neighbour.transform.position.z;
				card.transform.position = new Vector3 (finalX, finalY, finalZ); 

		}

		void UnselectCards ()
		{
				// NO MATCH
				SFXMan.sfx_FlipBack.Play ();
				foreach (var card in Card.Cards) {
						if (card.CurrentSide == Card.FlipsSide.Front) {
								StartCoroutine (card.FlipToBack_cr ());
						}						
				}
				SelectedCard = null;
		}

		/// <summary>
		/// Get list sorted by cards X position. Low X = low index. 
		/// </summary>
		public List<Card> AllCardsByLowXPosition ()
		{
				return Card.Cards.OrderBy (c => c.LeftEdge ()).ToList ();	
		}

		/// <summary>
		/// Get list sorted by cards X position. High X = lower index. Top Row cards first then bottom row cards. 
		/// </summary>
		public List<Card> AllCardsByHighXPosition ()
		{
				var allCards = Card.Cards.OrderBy (c => c.LeftEdge ()).ToList ();
				allCards.Reverse ();	
				return allCards;
		}

		public IEnumerator BlipAllCards_cr ()
		{
				foreach (var card in Card.Cards) {
						card.BlipSelf ();
						yield return new WaitForSeconds (0.2f);
				}
		}

		public static void CardCascade ()
		{
				/*InvalidOperationException: Collection was modified; enumeration operation may not execute.
System.Collections.Generic.List`1+Enumerator[Card].VerifyState () (at /Users/builduser/buildslave/monoAndRuntimeClassLibs/build/mcs/class/corlib/System.Collections.Generic/List.cs:778)
System.Collections.Generic.List`1+Enumerator[Card].MoveNext () (at /Users/builduser/buildslave/monoAndRuntimeClassLibs/build/mcs/class/corlib/System.Collections.Generic/List.cs:784)
CardMan.CardCascade () (at Assets/Scripts/Cards/CardMan.cs:245)
CardGameMan.GameOver () (at Assets/Scripts/Cards/CardGameMan.cs:143)
CardGameMan.SlowUpdate () (at Assets/Scripts/Cards/CardGameMan.cs:94) */
				foreach (var card in Card.Cards) {
						card.Cascade ();					
				}
		}

	

}
