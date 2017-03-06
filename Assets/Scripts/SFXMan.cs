using UnityEngine;
using System.Collections;

public class SFXMan : MonoBehaviour
{
		//	static public  SFXMan Me;
		static public  AudioListener listener;
		static public	AudioSource player; 
		static public AudioSource musicPlayer;
		static	string path = "SFX/";
		static	public SFX sfx_CardsMatched;
		static	public SFX sfx_Error;
		static	public SFX sfx_FlipBack;
		static	public SFX sfx_FlipFront;
		static	public SFX sfx_GameOver;
		static	public SFX sfx_Score;
		static	public SFX sfx_Realign;	
		static	public SFX sfx_Warning;	
		static	public SFX sfx_Confirm ;	
		static	public SFX sfx_Shot ;	
		static	public SFX sfx_BadThing ;	
		static	public SFX sfx_Confirm2 ;
		static	public SFX sfx_Malfunction1 ;	
		static	public SFX sfx_Malfunction2;	
		static	public SFX sfx_Explosion ;
		static	public SFX sfx_Pling ;
		static	public SFX sfx_EnergyCharge ;
		static	public SFX sfx_MarthaDeath ;
		static	public SFX sfx_CardWaveCleared ;
		static	public SFX sfx_ZombieWaveCleared ;
		static	public SFX sfx_StartWave ;
		static	public SFX sfx_MainMenuStart;
		static	public SFX sfx_StartGame;
		static	public SFX sfx_MarthaPain ;
		static	public SFX sfx_Kick ;
		static	public SFX sfx_ZombieJumped ;
		static	public SFX sfx_SwordHit;
		static	public SFX sfx_ZombieKilled;
		static	public SFX sfx_SwordWhip;
		static	public SFX sng_PlatformSong;
		static	public SFX sng_CardSong;
		static	public SFX sng_StartMenuSong;
		static  float sfxVolume = 1;

		public static float SfxVolume {
				get {
						return sfxVolume;
				}
				set {
						sfxVolume = value;
				}
		}

		static  float musicVolume = 1;

		public static float MusicVolume {
				get {
						return musicVolume;
				}
				set {
						musicVolume = value;						
				}
		}

		void Awake ()
		{				
				listener = gameObject.AddComponent<AudioListener> ();
				

				player = gameObject.AddComponent<AudioSource> ();
				musicPlayer = gameObject.AddComponent<AudioSource> ();
				
				sfx_CardsMatched = new SFX (path + "CardsMatched", 1F);
				sfx_Error = new SFX (path + "Error", 1F);		
				sfx_FlipBack = new SFX (path + "FlipBack", 1F);		
				sfx_FlipFront = new SFX (path + "FlipFront", 1F);	
				sfx_GameOver = new SFX (path + "GameOver", 0.2F);	
				sfx_Score = new SFX (path + "Score", 0.4F);	
				sfx_Realign = new SFX (path + "Realign", 0.3F);	
				sfx_Warning = new SFX (path + "Warning", 1F);	

				sfx_Confirm = new SFX (path + "Confirm", 1F);	
				sfx_Shot = new SFX (path + "Shot", 0.5F);	
				sfx_BadThing = new SFX (path + "BadThing", 1F);	
				sfx_Confirm2 = new SFX (path + "Confirm2", 1F);
				sfx_Error = new SFX (path + "Error", 0.5F);	
				sfx_Malfunction1 = new SFX (path + "Malfunction1", 1F);	
				sfx_Malfunction2 = new SFX (path + "Malfunction2", 1F);	
				sfx_Explosion = new SFX (path + "Explosion", 0.5F);
				sfx_Pling = new SFX (path + "Pling", 1F);	
				sfx_EnergyCharge = new SFX (path + "EnergyCharge", 0.7F);

				sfx_MarthaDeath = new SFX (path + "MarthaDeath", 1F);	
				sfx_CardWaveCleared = new SFX (path + "CardWaveCleared", 0.5F);
				sfx_ZombieWaveCleared = new SFX (path + "ZombieWaveCleared", 0.8F);	
				sfx_StartWave = new SFX (path + "StartWave", 1F);
				sfx_MainMenuStart = new SFX (path + "MainMenuStart", 0.5F);
				sfx_StartGame = new SFX (path + "StartGame", 0.7F);

	
				sfx_MarthaPain = new SFX (path + "MarthaPain", 0.4F);	
				sfx_Kick = new SFX (path + "Kick", 0.5F);
				sfx_ZombieJumped = new SFX (path + "ZombieJumped", 0.7F);	
				sfx_SwordHit = new SFX (path + "SwordHit", 0.5F);
				sfx_SwordWhip = new SFX (path + "SwordWhip", 0.3F);
				sfx_ZombieKilled = new SFX (path + "ZombieKilled", 1F);

				sng_PlatformSong = new SFX (path + "PlatformSong", 1F);
				sng_CardSong = new SFX (path + "CardSong", 1F);	
				sng_StartMenuSong = new SFX (path + "StartMenuSong", 1F);	
		}
	

		public static void PlaySfx (AudioClip  sfx)
		{				
				player.PlayOneShot (sfx);
		}

		public static void PlayAsSong (SFX song)
		{
				if (MusicVolume > 0) {

						musicPlayer.clip = song.audioClip;
						musicPlayer.volume = MusicVolume;
						musicPlayer.Play ();
						
				}
		}

		public static void StopSong ()
		{
				musicPlayer.Stop ();
		}
}

public class SFX
{
		public AudioClip audioClip;
		float volume;
		public SFX (string pathToFile, float volume)
		{
				audioClip = Resources.Load (pathToFile) as AudioClip;
				this.volume = volume;
		}
 
		public void Play ()
		{
				if (SFXMan.SfxVolume > 0) {							
						SFXMan.player.volume = volume * SFXMan.SfxVolume;
						SFXMan.PlaySfx (audioClip);
				}
		}
}


