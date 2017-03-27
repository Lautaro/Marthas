using UnityEngine;
using System.Collections;

public class Blip2D : MonoBehaviour
{

	#region Static Methods

		static string blip2DprefabPath = "Prefabs/Misc/2DBlip";

		/// <summary>
		/// Blip at position untill destroyed.
		/// </summary>
		/// <param name="position">Position.</param>
		static public Blip2D BlipAtPosition (Vector2 position)
		{
				GameObject blip = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;
				blip.transform.position = position;
				var blipController = blip.GetComponent<Blip2D> ();
				blipController.followsTransform = false;
				return blipController;

		}

		/// <summary>
		/// Blip at position untill destroyed. For limited duration set lifeSpan to higher than zero. Otherwise blips until destroyed.
		/// </summary>
		/// <param name="position">Position.</param>
		static public Blip2D BlipAtPosition (Vector2 position, float lifeSpanInSeconds)
		{
				GameObject blip = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;
				blip.transform.position = position;
				var blipController = blip.GetComponent<Blip2D> ();
				blipController.followsTransform = false;
				blipController.lifeSpanInSeconds = lifeSpanInSeconds; 	
				return blipController;
		}

		/// <summary>
		/// Blips a static position. For limited duration set lifeSpan to higher than zero. Otherwise blips until destroyed. 
		/// </summary>
		/// <param name="position">static position.</param>
		/// <param name="lifeSpanInSeconds">Life span in seconds.</param>
		/// <param name="blipSize">Blips max size.</param>
		/// <param name="blipSpeed">How fast the blip blinks</param>
		static public Blip2D BlipAtPosition (Vector2 position, float lifeSpanInSeconds, float blipSize, float blipSpeed)
		{
				GameObject blip = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;
				blip.transform.position = position;
				var blipController = blip.GetComponent<Blip2D> ();
				blipController.followsTransform = false;
				
				blipController.lifeSpanInSeconds = lifeSpanInSeconds; 
				blipController.lerpDuration = blipSpeed;
				blipController.targetSize = blipSize;
				return blipController;
		}

		static public Blip2D BlipFollow (Transform follow, float lifeSpanInSeconds, float blipSize, float blipSpeed)
		{
				GameObject blip = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;					
				var blipController = blip.GetComponent<Blip2D> ();
				blipController.followsTransform = true;
				blipController.lifeSpanInSeconds = lifeSpanInSeconds; 
				blipController.lerpDuration = blipSpeed;
				blipController.targetSize = blipSize;
				blipController.followTransform = follow;
				return blipController;
		}

		static public Blip2D BlipFollow (Transform follow, float lifeSpanInSeconds)
		{
				GameObject blip = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;					
				var blipController = blip.GetComponent<Blip2D> ();
				blipController.followsTransform = true;
				blipController.lifeSpanInSeconds = lifeSpanInSeconds; 
				blipController.followTransform = follow;
				return blipController;
		}

		/// <summary>
		/// Blips the transform untill destroyed. 
		/// </summary>
		/// <param name="follow">Follow.</param>
		static public Blip2D BlipFollow (Transform follow)
		{
				GameObject circle = Instantiate (Resources.Load (blip2DprefabPath)) as GameObject;					
				var circleController = circle.GetComponent<Blip2D> ();
				circleController.followTransform = follow;
				return circleController;
		}

	#endregion

		public float lifeSpanInSeconds = 0.0f;
		float existenceTimer; 
		float targetSize = 0.2f;
		float lerpDuration = 0.3F; 
		Transform followTransform; 
		bool followsTransform = false; 

		// Use this for initialization
		void Start ()
		{

				existenceTimer = Time.time;
				transform.localScale = new Vector3 (0, 0, 0);
				StartCoroutine (DoBlip_cr ());	
				if (lifeSpanInSeconds > 0) {
						Invoke ("DestroySelf", lifeSpanInSeconds);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (followTransform == null && followsTransform == true) {
						//If blip is set to follow a transform but there is no transform to follow -> Commmit suicide. 					
						DestroySelf ();

				} else {
						if (followTransform) {
								var newPos = followTransform.position;
								newPos.z --; //makes sure blip is between player and camera;
								transform.position = newPos;			
						} 					
				}
		}



		IEnumerator DoBlip_cr ()
		{
				while (true) {					
								
						//	print ("Starting lerp up");
						yield return StartCoroutine (LerpSize (targetSize, lerpDuration)); 
						//	print ("Finished lerp up. Starting Lerp down");
						yield return  StartCoroutine (LerpSize (0, lerpDuration)); 
						//	print ("Finished Lerp down");			
				} 
		}

		IEnumerator LerpSize (float targetSize, float durationSeconds)
		{				
				var alpha = GetComponent<Renderer>().material.color.a;
				var t = Time.deltaTime;
				var material = GetComponent<Renderer>().material;
				var startScale = transform.localScale.x;	

				while (t < durationSeconds) {

						var add = Mathf.Lerp (startScale, targetSize, t / durationSeconds);
					
						//regulates size per cycle
						transform.localScale = new Vector3 (add, add, add);
						
						var newAlpha = durationSeconds / t / 2;
						material.color = new Color (1F, 1F, 1F, newAlpha);
						// t / durationSeconds;
						yield return null;
						t += Time.deltaTime;
						// add time for each cycle
				}
		}

		void DestroySelf ()
		{
				//print ("Im killing myself after this amount of time: " + lifeSpanInSeconds);
				DestroyImmediate (gameObject);
		}
}
