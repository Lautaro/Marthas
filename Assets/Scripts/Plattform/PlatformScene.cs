using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformScene : MonoBehaviour
{
		//public SFXMan sfxMan;
		
		public static MarthaController Martha;
		public static bool IsGameOn; 
		public bool AllowEvilStuff = true;
		public Transform zombieSpawnPoint; 
		static	int waveZombieMultiplier = 4;
		int zombiesSpawned;
		int zombiesKilled;
		

		public  int ZombiesKilled {
				get {
						return zombiesKilled;
				}
				set {
						zombiesKilled = value;
						var wave = MainGame.Wave;
						
						if (zombiesKilled == wave * waveZombieMultiplier) {
						
								StartCoroutine (WaveCleared_cr ());
						}
				}
		}

		public	static PlatformScene ps; 


		void Awake ()
		{
	
				MainGame.state = MainGameState.PlatformGame;
				ps = this;

		}

		void Start ()
		{
				StartCoroutine (InitiateWave_cr ());//	InitiateWave ();
				
		}




		IEnumerator InitiateWave_cr ()
		{
				
				//PRESENT WAVE
				var wave = MainGame.Wave;
				SFXMan.sfx_StartWave.Play ();
				string presentation = string.Format ("Wave {0}", wave);
				Hud.ShowPresentation (presentation);
				yield return new WaitForSeconds (3);

				presentation = string.Format ("Kill {0} Zombies! ", wave * 3);
				Hud.ShowPresentation (presentation);
				yield return new WaitForSeconds (1);

				
				//SETUP PLAYER
				Martha = (Instantiate (Prefabs.Martha) as GameObject).GetComponent<MarthaController> () as MarthaController;
				Martha.transform.position = new Vector3 (10, 10);
		
		
				//START EVIL PLANS
				InvokeRepeating ("DoEvilStuff", 1.0f, 2.0f);

				//START PICKUP PLAN
				InvokeRepeating ("SpawnPickups", 1.0f, 4.0f);
				
				//Initiate game variables
				IsGameOn = true;
				ZombiesKilled = 0;
				zombiesSpawned = 0;
				//	EnableAllCharacters (true);
				SFXMan.PlayAsSong (SFXMan.sng_PlatformSong);
				
		}

		void SpawnPickups ()
		{
				GameObject pickup = Instantiate (Prefabs.Pickup, new Vector2 (0, 0), Quaternion.identity) as GameObject;	
				
				SFXMan.sfx_Shot.Play ();
		}

		IEnumerator WaveCleared_cr ()
		{
				IsGameOn = false;
				//	EnableAllCharacters (false);
				SFXMan.StopSong ();		
				yield return new WaitForSeconds (0.5f);
				
				SFXMan.sfx_ZombieWaveCleared.Play ();
				Hud.ShowPresentation ("Wave " + MainGame.Wave + " Cleared! ");
				yield return new WaitForSeconds (3);
				
				Application.LoadLevel ("CardScene");
		}

	
		public static IEnumerator GameOver_cr ()
		{
				if (IsGameOn) {
						PlatformScene.IsGameOn = false;
						Martha.collider2D.enabled = false;
						SFXMan.StopSong ();
						SFXMan.sfx_MarthaDeath.Play ();
						yield return new WaitForSeconds (1);
						SFXMan.sfx_GameOver.Play ();
						Hud.ShowPresentation ("U R Looser !");
						yield return new WaitForSeconds (3);
						Application.LoadLevel ("MainMenu");
				}
		}
	
		void DoEvilStuff ()
		{			

				if (AllowEvilStuff && IsGameOn) {
						if (zombiesSpawned < MainGame.Wave * waveZombieMultiplier) {
							
								GameObject zombie = Instantiate (Prefabs.ZombieBoy, zombieSpawnPoint.transform.position, Quaternion.identity) as GameObject;
				
								var newX = Random.Range (-10, 10);
								var newY = Random.Range (3, 10);
								transform.position = new Vector3 (newX, newY, 0);
								Blip2D.BlipAtPosition (transform.position, 0.5f);
				
								// Give push to random direction
								zombie.GetComponent<ZombieBoyController> ().rigidbody2D.AddForce (new Vector2 (0, 500));
				
								zombiesSpawned ++;
						} 
				}
		}

		
	#region STATIC MEMBERS
		static List<MonoBehaviour> ActiveCharacters = new List<MonoBehaviour> ();

		public static void AddActiveCharacter (MonoBehaviour character)
		{
				ActiveCharacters.Add (character);
		}




		public static void RemoveCharacterFromActiveList (MonoBehaviour character)
		{			
				ActiveCharacters.Remove (character);
		}
}
#endregion