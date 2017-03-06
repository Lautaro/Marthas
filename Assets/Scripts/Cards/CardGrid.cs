using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardGrid
{
		public List<Card> cards;
		public int waveIndex;
		int amountOfCards;


		// Use this for initialization
		public CardGrid (int waveIndex)
		{
				this.waveIndex = waveIndex;
				
				var amountOfCards = CardMan.cardPairsPerWave * 2;
				cards = new List<Card> (amountOfCards);

				// Get a list of random textures as big as half the amount of cards in wave. So a wave with 8 cards will have 4 textures and created 4 pairs. 
				List<Texture> textures = new List<Texture> (amountOfCards / 2);

				for (int i = 0; i < amountOfCards / 2; i++) {
						var randomTexture = CardMan.GetRandomTexture ();
						
						//ADD EACH TEXTURE TWICE TO ENSURE PAIRS			
						textures.Add (randomTexture);
						textures.Add (randomTexture);
				}

				for (int i = 0; i < amountOfCards; i++) {
						//create card 
						var card = CardMan.CreateNewCardPrefab ();			


						//IS CARD IN TOP OR BOTTOM ROW?
						card.IsTopRow = i < (amountOfCards / 2) ? true : false;

						//Get one of the texture index left in list
						var textureIndex = Random.Range (0, textures.Count);

						//Set the texture to the card
						card.SetImage (textures [textureIndex]);

						//REMOVE TEXTURE SO THAT IT DOES NOT GET USED AGAIN. THIS ASSURES THAT THERE IS ALWAYS EVEN PAIRS IN EACH WAVE.
						textures.RemoveAt (textureIndex);

						//position cards on top or bottom row
						int xWaveMultiplier = (amountOfCards / 2) * (waveIndex); //add x for each prior wave of cards. Each wave is amountOfCard/2 cards wide.
						float cardWidth = card.Box ().width;
						int margin = CardMan.cardMargin;
						float width = cardWidth + margin;
						float x = CardMan.cardStartingX; 
						float columnIndex; // column position of card relative to all cards in all waves
						float y; 
						
						if (card.IsTopRow) {
								//CARD IS IN TOP ROW
								columnIndex = xWaveMultiplier + i;

								x += columnIndex * width;
								y = CardMan.topRowY;


						} else {
								// CARD IS ON BOTTOM ROW

								columnIndex = xWaveMultiplier + (i - amountOfCards / 2);//starts on bottom row

								x += columnIndex * width;
								y = CardMan.bottomRowY;

						}					

						card.transform.position = new Vector3 (x, y, CardMan.cardStartingZ);

						cards.Add (card);

				}



	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
