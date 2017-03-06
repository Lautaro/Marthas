using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour
{
		static UILabel lifeCounter; 
		static UILabel wavePresentation;
		static UILabel energyBallCounter; 
		// Use this for initialization
		void Awake ()
		{				
				lifeCounter = transform.Find ("LifeCounter").GetComponent<UILabel> ();	
				wavePresentation = transform.Find ("WavePresentation").GetComponent<UILabel> ();	
				energyBallCounter = transform.Find ("EnergyBallCounter").GetComponent<UILabel> ();

				wavePresentation.enabled = false;
		}
	
		public static void SetLifeCounter (int life)
		{
				lifeCounter.text = "Life: " + life;

		}

		public static void SetEnergyBallCounter (int amount)
		{
				energyBallCounter.text = "Energy Balls: " + amount;

		}

		public static void ShowPresentation (string presentation)
		{
				wavePresentation.text = presentation;
				wavePresentation.enabled = true;

				var components = wavePresentation.GetComponents<UITweener> ();

				foreach (var tween in components) {
						tween.Reset ();
						tween.PlayForward ();

				}	
		}

		public static void DisablePresentation ()
		{
				wavePresentation.enabled = false;
		}

		public void PresentationFinished ()
		{
				wavePresentation.enabled = false;
		}
}
