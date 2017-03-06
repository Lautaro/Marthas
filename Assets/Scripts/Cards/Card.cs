using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour
{

	#region STATIC
		public static List<Card> Cards = new List<Card> ();	
		static int nextId = 0;

		/// <summary>
		/// Standard dimensions of cards. Sets at start. X is width of card, Y is height of card. 
		/// </summary>
		public	static Vector2 cardDimensions;
	#endregion

		#region Fields and Properties
		FlipsSide currentFlipSide ;
		public FlipsSide CurrentSide {
				get {
						return currentFlipSide;
				}
				set {
						currentFlipSide = value;
				}
		}


		public static float speed = 1f;
		public int id	;
		public string cardName = "not found";
		ParticleSystem starBurst; 
		Transform cardSides;
		GameObject front;
		GameObject back;
		bool  isTopRow;
		bool isMatched = false;

		public bool IsTopRow {
				get {
						return isTopRow;
				}
				set {
						isTopRow = value;
				}
		}

 
		public int Id {
				get {
						return id;
				}				
		}

		/// <summary>
		/// Returns a 2D box of the card in world space. 
		/// </summary>
		public Rect Box ()
		{
				var bounds = front.renderer.bounds;
				var box = new Rect ();
				box.xMin = bounds.min.x;
				box.xMax = bounds.max.x;
				box.yMin = bounds.min.y;
				box.yMax = bounds.max.y;
				return box;

		}
		

		/// <summary>
		/// The cards 2D left edge in world space
		/// </summary>
		public float LeftEdge ()
		{
				return Box ().xMin;
		}

		/// <summary>
		/// The cards 2D right edge in world space
		/// </summary>
		public float RightEdge ()
		{
				return Box ().xMax;
		}

	#endregion

		void Awake ()
		{	
				rigidbody.isKinematic = true;

				id = nextId++;
				cardSides = transform.FindChild ("CardSides");		
				starBurst = GetComponent<ParticleSystem> ();
				front = cardSides.FindChild ("Front").gameObject;
				back = cardSides.FindChild ("Back").gameObject;
				cardSides.rotation = Quaternion.Euler (0, 180, 0);
				CurrentSide = FlipsSide.Back;	
				cardDimensions = new Vector2 (Box ().width, Box ().height);
				Cards.Add (this);										
		}

		public void SetImage (Texture texture)
		{	
				front.renderer.material.mainTexture = texture;

				cardName = texture.name;
		}


		#region Card Flip Logic
		void OnMouseUp ()
		{			
				CardMan.Me.OnCardClicked (this);	
				
		}

		public IEnumerator FlipToFront_cr ()
		{
				
				if (CurrentSide == FlipsSide.Back) {
						yield return StartCoroutine (LerpRotationY (Card.FlipsSide.Front));
				}
			 	
		}

		public IEnumerator FlipToBack_cr ()
		{				
				if (CurrentSide == FlipsSide.Front) {

						yield return StartCoroutine (LerpRotationY (Card.FlipsSide.Back));
				} else {
						Debug.LogWarning ("Card could not flip to back because it is not set as front flipped");
				}
		}

		IEnumerator LerpRotationY (FlipsSide flipSide)
		{
				if (flipSide == FlipsSide.Front) {
						SFXMan.sfx_FlipFront.Play ();
				} 

				//CONVERT THE FLIPSIDE TO CORRECT ROTATION ANGLE. 0 FOR FRONT AND 180 FOR BACK
				int targetAngle = flipSide == FlipsSide.Front ? 0 : 180;
							
				var durationSeconds = 0.3F; 
				var flipRotation = Quaternion.Euler (0, targetAngle, 0);
				var t = Time.deltaTime;
				var startingRotation = cardSides.rotation;		// store starting rotation:
				var timePassed = Time.time;
							
				while (t < durationSeconds) {

						cardSides.rotation = Quaternion.Slerp (startingRotation, flipRotation, t / durationSeconds);					
										
						yield return null;

						t += Time.deltaTime;
				}

				//force destination rotation
				cardSides.rotation = flipRotation;
						
				if (cardSides.rotation.eulerAngles.y == 0) {
						CurrentSide = FlipsSide.Front;						
				} else {
						CurrentSide = FlipsSide.Back;
				}	
				
		}
		#endregion

		#region Misc
		/// <summary>
		/// Removes the card card after random 0 -0.f sec with animations and effects. 
		/// </summary>
		/// <param name="delay">Delay.</param>
		public void RemoveCardCardAfterDelay ()
		{
				Cards.Remove (this);
				Invoke ("RemoveCard", Random.Range (0, 0));
		}

		void RemoveCard ()
		{	
				collider.enabled = false;
				isMatched = true;
				var torque = new Vector3 (Random.Range (-500, 500), Random.Range (-500, 500), Random.Range (-500, 500));
				var force = new Vector3 (Random.Range (-30, 30), Random.Range (-30, 30), 0);


				transform.position += new Vector3 (0, 0, -10);
				rigidbody.isKinematic = false;
				rigidbody.AddForce (force, ForceMode.Impulse);
				rigidbody.AddTorque (torque, ForceMode.Impulse);
				
				starBurst.Play ();
				StartCoroutine (FadeOut_cr ());
				Destroy (gameObject, 5f);
		}

		IEnumerator FadeOut_cr ()
		{
				var durationSeconds = 2f;

				//Scale
				var t = Time.deltaTime;
				var startScale = transform.localScale; 
				var targetValue = new Vector3 (startScale.x / 1, startScale.y / 1, startScale.z);
			
				//Alpha
				var startColor = front.renderer.material.color;
				var targetColor = new Color (1, 1, 1, 0);

				while (t < durationSeconds) {

						transform.localScale = Vector3.Slerp (startScale, targetValue, t / durationSeconds);		

						var newColor = Color.Lerp (startColor, targetColor, t / durationSeconds);
						
						front.renderer.material.color = newColor;
						back.renderer.material.color = newColor;
						
						yield return null;
			
						t += Time.deltaTime; // add time for each cycle
				}
				//IMPORTANT! Force the target value cause Slerp will never get 
				transform.localScale = targetValue; 

		}

		/// <summary>
		/// Cascade the specified forceDownwards.
		/// </summary>
		/// <param name="forceDownwards">If set to <c>true</c> adds force inwards Z and  downwards Y to get card out of screen.</param>
		public void Cascade ()
		{
				isMatched = true;
				print ("Cascading card: " + cardName);
				rigidbody.isKinematic = false;

				float range = 100F;

				var pos = transform.position;

				transform.position = new Vector3 (pos.x, pos.y, 5);
		
				var x = Random.Range (-range, range);
				var y = Random.Range (-range, range);	
				var Z = Random.Range (-range, range);	
				var force = new Vector3 (x, y, Z);
			
				rigidbody.AddForce (force, ForceMode.Impulse);
				rigidbody.AddTorque (force, ForceMode.Impulse);
				
				Destroy (gameObject, 5f);
		}

		public void BlipSelfWhereClicked ()
		{
				Tools.Me.SphereBlip (Camera.main.ScreenToWorldPoint (Input.mousePosition));
		}

		public void BlipRightEdgeOfCard ()
		{
				Tools.Me.SphereBlip (new Vector2 (RightEdge (), Box ().center.y));
		}

		public void BlipSelf ()
		{
				//	Tools.Me.SphereBlip (Box ().center);
		}
	
	
		public enum FlipsSide
		{
				Front, 
				Back
		}
	#endregion
}
