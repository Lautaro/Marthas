using UnityEngine;
using System.Collections;

public class CardPickup : MonoBehaviour
{

		Vector3 goTo;
		Vector3 startPosition; 
		bool IsFleeing = false;
		float startTime; 
		ParticleSystem particles;
		float totalDistance;
		float speed = 25f; 
		bool isPickedUp = false;
		int life = 3;

		public int Life {
				get {
						return life;
				}
				set {

						if (value < life) {				
								IsFleeing = true;
								SFXMan.sfx_Shot.Play ();
						}

						life = value;
						lifeLabel.text = value.ToString ();

						if (value < 1) {
								lifeLabel.text = "";
								isPickedUp = true;
								collider.enabled = false;
								CardScene.EnergyBalls ++; 				
								particles.startSpeed = 6f;				
								particles.startLifetime = 2f;
								particles.emissionRate = 300f;
								SFXMan.sfx_Score.Play ();
								Invoke ("Die", 1f);
						}
				}
		}

		TextMesh lifeLabel;
		

 


		// Use this for initialization
		void Start ()
		{
				lifeLabel = transform.Find ("LifeLabel").GetComponent<TextMesh> ();
				startTime = Time.time;
				particles = transform.Find ("Particles").GetComponent<ParticleSystem> ();
				startPosition = transform.position;
				goTo = startPosition;
				totalDistance = 1;
				CardScene.HasPickUp = true;
		}

		
	
		// Update is called once per frame
		void Update ()
		{

				if (!isPickedUp) {
						var distanceCovered = (Time.time - startTime) * speed;
						var progress = distanceCovered / totalDistance;
					
					
						if (progress >= 1 || IsFleeing) {
								//SET NEW RANDOM POSITION
								var newX = Random.Range (7f, 70f);
								var newY = Random.Range (7f, 55f);
								speed = Random.Range (10, 50);
								if (IsFleeing) {
										speed = 50;
										IsFleeing = false;
								}
								goTo = new Vector3 (newX, newY, 3);
					
								startTime = Time.time;
								startPosition = transform.position;
								totalDistance = Vector3.Distance (transform.position, goTo);
						}			
					
						transform.position = Vector3.Lerp (startPosition, goTo, progress);

						/*	if(_TargetObject != null) {
				
				Vector3 newPos = _TargetObject.position + DistancebetweenTargetAndFollower;
				newPos = Vector3.SmoothDamp(mprevPosition,newPos,ref mcurrentVelocity, mfmaxSpeed);
				mprevPosition = newPos;
				
				transform.position = newPos;
			}*/
				}

				/*			if (Input.GetMouseButtonDown (0)) {
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit hit;
						// Casts the ray and get the first game object hit
						Physics.Raycast (ray, out hit);
						if (hit.collider.gameObject == this.gameObject) {
								PickedUp ();
						}			
						Debug.Log ("This hit at " + hit.point);
				}*/

		}

		void OnMouseDown ()
		{			
				Life--;				
		}

		void Die ()
		{
				particles.Stop (true);
				CardScene.HasPickUp = false;
				Destroy (gameObject, 4f);
		}
}
