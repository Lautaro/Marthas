using UnityEngine;
using System.Collections;

public class CardGui : MonoBehaviour
{
	#region
		public static CardGui Me;
	#endregion
	
		UILabel scoreLabel;
		UILabel speedLabel;
		UILabel energyBallsLabel;
		static UILabel wavePresentation;
		static int score;

		public int Score {
				get {
						return score;
				}
				set {
						score += value;
						SFXMan.sfx_Score.Play ();
						scoreLabel.text = "Score: " + score;
				}
		}

		/// <summary>
		/// This is set from gravity property setter. Dont set manually.
		/// </summary>
		/// <param name="speed">Speed.</param>
		public void SetSpeedLabel (float speed)
		{
				speedLabel.text = "Speed: " + (speed * 100).ToString ("N0");
		}

		public void SetEnergyBalls (int amount)
		{
				energyBallsLabel.text = "Energy Balls: " + amount;
		}

		void Awake ()
		{
				Me = this;
				scoreLabel = transform.Find ("ScoreLabel").GetComponent<UILabel> ();
				speedLabel = transform.Find ("SpeedLabel").GetComponent<UILabel> ();
				wavePresentation = transform.Find ("WavePresentation").GetComponent<UILabel> ();
				energyBallsLabel = transform.Find ("EnergyBallsLabel").GetComponent<UILabel> ();
				wavePresentation.enabled = false;
		}



		public static void Message (string message, Color color)
		{
				wavePresentation.text = message;				
				wavePresentation.color = color;
				wavePresentation.enabled = true;

				var components = wavePresentation.GetComponents<UITweener> ();
		
				foreach (var tween in components) {

						tween.Reset ();						
						tween.PlayForward ();		
				}
		}

		public void PresentationFinished ()
		{

		}


	

}
