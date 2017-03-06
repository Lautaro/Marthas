using UnityEngine;
using System.Collections;

public class CardScene : MonoBehaviour
{

		public static CardMan cardMan;
		public static Tools tools;
		public static CardScene cardGameManager;
		//public static SFXMan sfxMan;
		public static CardGui guiMan;
		static float gravityStrength;
		public GameState gameState;
		public static bool HasPickUp = false;
		public static GameObject pickUp;
		public static bool IsGravityPaused = false;
		public static float GravityPauseDuration = 3f; 
		public static float GravityStrength {
				get {
						return gravityStrength;
				}
				set {
						gravityStrength = value;
						if (value < 0f) {
								value = 0f;				
						}

						if (value > 0f && value < 0.5f) {
								value = 0.5f;
						}

						Physics.gravity = new Vector3 (-gravityStrength, 0, 0);
						CardGui.Me.SetSpeedLabel (value);
				}
		}

		static int energyBalls;
		public static int EnergyBalls {
				get { return energyBalls;}
				set {
						if (value > energyBalls) {
								CardGui.Message ("ENERGY BALL!", Color.cyan);
						}
						energyBalls = value;

						CardGui.Me.SetEnergyBalls (value);
				}
		}

		public static float gravityAcceleration;
		public	static DangerState dangerState;
		float gameOverRange = 15F; //X Position the card should be in to cause game over.

		



		// Use this for initialization
		void Start ()
		{
				cardGameManager = this;
				gameState = GameState.StartingUp;
				EnergyBalls = 1;

				tools = Tools.Me;
				if (tools == null) {
						Debug.LogWarning ("Tools reference in GameManager is null");
				}

				guiMan = CardGui.Me;
				if (guiMan == null) {
						Debug.LogWarning ("GuiMan reference in GameManager is null");
				}

				cardMan = CardMan.Me;
				if (cardMan == null) {
						Debug.LogWarning ("CardMan reference in GameManager is null");
				}

				StartCoroutine (InitiateWave_cr ());
				
	
		}

		IEnumerator InitiateWave_cr ()
		{		


				//Present wave
				SFXMan.sfx_StartWave.Play ();
				var wave = MainGame.Wave;
				
				CardGui.Message (string.Format ("Clear {1} cards!", wave, wave * (CardMan.cardPairsPerWave * 2)), Color.white);

				yield return new WaitForSeconds (3);			

				SFXMan.PlayAsSong (SFXMan.sng_CardSong);

				dangerState = DangerState.VerySafe;

				GravityStrength = 1f;
				
				InvokeRepeating ("SlowUpdate", 0, 1F);

				CardMan.SetupWave ();

				gameState = GameState.GameOn;
		
		}
	

	
		// Update is called once per frame
		void Update ()
		{


				foreach (var card in Card.Cards) {
						if (card) {		
			
								var newX = card.transform.position.x - GravityStrength * Time.deltaTime;
								var y = card.transform.position.y;
								var z = card.transform.position.z;
								card.transform.position = new Vector3 (newX, y, z);
						}
				}

				if (Input.GetKeyDown (KeyCode.Space)) {
						if (EnergyBalls > 0 && !IsGravityPaused) {
								StartCoroutine (PauseGravity_cr ());
								EnergyBalls --;
						}
				}
		}

		IEnumerator PauseGravity_cr ()
		{
				
				IsGravityPaused = true;

				var isSlowingDown = true;
				var normalGravity = GravityStrength;
				var lerpStartTime = Time.time; 
				var timeLerping = Time.time - lerpStartTime;
				var startStopDuration = 1f; 
				var progress = timeLerping / startStopDuration;

				//SLOW DOWN
				CardGui.Message ("GRAVITY PAUSE!", Color.cyan);
				while (isSlowingDown && progress < startStopDuration) {
						
						GravityStrength = Mathf.Lerp (normalGravity, 0, progress);
						timeLerping += Time.deltaTime;
						progress = timeLerping / startStopDuration;
						yield return null;
				}
				isSlowingDown = false;
				lerpStartTime = Time.time; 
				timeLerping = Time.time - lerpStartTime;
				progress = timeLerping / startStopDuration;
				
				yield return new WaitForSeconds (GravityPauseDuration);
				
				//RESUME SPEED
				CardGui.Message ("HERE WE GO AGAIN!", Color.cyan);
				while (!isSlowingDown && progress < startStopDuration) {
				
						GravityStrength = Mathf.Lerp (0, normalGravity, progress);
						timeLerping += Time.deltaTime;						
						progress = timeLerping / startStopDuration;
						yield return null;

				}
				


				IsGravityPaused = false;
		}

		void SlowUpdate ()
		{

				if (gameState == GameState.GameOn) {
						if (!HasPickUp) {
								
								if (Random.Range (0, 7) == 0) {
										pickUp = Instantiate (Resources.Load ("Prefabs/CardGame/Pickup"))as GameObject;	

								} else {
										print ("No pickup today!");
								}
						}


						if (!IsGravityPaused) {
								// ADD GRAVITY OVER TIME
								GravityStrength += gravityAcceleration;
								if (GravityStrength > 150) {
										gravityAcceleration = 0.05f;
								} else if (GravityStrength > 250) {
										gravityAcceleration = 0.02f;
								} else {
										gravityAcceleration = 0.07f;
								}
						}
						
					
						// CHECK CARDS POSITION for game over
						var cardWidth = Card.cardDimensions.x;
						var allCards = cardMan.AllCardsByLowXPosition ();
						foreach (var card in allCards) {
								var xPos = card.Box ().center.x;
								if (xPos <= gameOverRange) {
										StartCoroutine (GameLost_cr ());
										return;
								} else
					if (xPos <= gameOverRange + cardWidth) {
										SecondWarning ();
										return;
								} else
						if (xPos <= gameOverRange + cardWidth * 2) {
										FirstWarning ();
										return;
								}
						}
					
						if (dangerState != DangerState.VerySafe) {
								dangerState = DangerState.VerySafe;
								SFXMan.sfx_Score.Play ();
								
						}
				}
				
		}



		void FirstWarning ()
		{
				if (dangerState != DangerState.FirstWarning) {
						
						dangerState = DangerState.FirstWarning;
						SFXMan.sfx_Warning.Play ();
				}
				
		}

		void SecondWarning ()
		{
				if (dangerState != DangerState.SecondWarning) {
						
						dangerState = DangerState.SecondWarning;
						SFXMan.sfx_Warning.Play ();
				}
		}

		public IEnumerator GameLost_cr ()
		{

				if (dangerState != DangerState.GameOver) {		
		
						SFXMan.StopSong ();
						gameState = GameState.GameLost;
						
						CardMan.CardCascade ();
						SFXMan.sfx_GameOver.Play ();
						dangerState = DangerState.GameOver;
						
						
						yield return new WaitForSeconds (3);
						StartCoroutine (InitiateWave_cr ());
						Application.LoadLevel ("MainMenu");

				}


		}

		public  IEnumerator GameWon_cr ()
		{
				SFXMan.StopSong ();
				SFXMan.sfx_CardWaveCleared.Play ();
				CardGui.Message ("Wave cleared! ", Color.white);
				gameState = GameState.GameWon;
				MainGame.Wave ++;
				yield return new WaitForSeconds (4);
				Application.LoadLevel ("PlatformScene");
		}

		public enum DangerState
		{
				VerySafe,
				Safe,
				FirstWarning, 
				SecondWarning,
				GameOver
		}

		public enum GameState
		{
				StartingUp,
				GameOn,
				Pause, 
				GameWon,
				GameLost
		}
}
