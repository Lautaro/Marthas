using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieBoyController : EnemyBase
{

	#region ZOMBIE BOY MANAGEMENT
		//STATIC MANAGEMENT FUNCTIONS
		public static List<ZombieBoyController> Zombies = new List<ZombieBoyController> ();
	#endregion

		float mutationStrength = 0.3f;
		bool isTargetToTheRight ;
		bool arrivedAtTarget; 
		public int kickDeathForceX = 1;
		public int	kickDeathForceY = 1;
		int hitPoint = 3;

		public int HitPoint {
				get {
						return hitPoint;
				}
				set {
						hitPoint = value;
						if (value < 1) {
								Die ();								
						}
				}
		}		

		protected override void Awake ()
		{
				Zombies.Add (this);
				base.Awake ();
		
				MaxSpeed = .5f;			
				
		}
	
		
		void Start ()
		{
				MutateZombie ();
				//rigidbody2D.AddForce (new Vector2 (kickDeathForceX, kickDeathForceY));

		}

		void MutateZombie ()
		{

				var currentScale = transform.localScale;
				var min = 1 - mutationStrength;
				var max = 1 + mutationStrength;
				var xScale = currentScale.x * Random.Range (min, max);
				var yScale = currentScale.y * Random.Range (min, max);
				transform.localScale = new Vector3 (xScale, yScale);
				MaxSpeed *= Random.Range (min, max);
				
		}
	
		// Update is called once per frame
		protected override void Update ()
		{	
				base.Update ();
						
		}

		protected override void FixedUpdate ()
		{

				base.FixedUpdate ();
				var bounds = GetComponent<Renderer>().bounds;
				Vector2 frontCheckPoint;
					

				if (target.x > transform.position.x) {
						frontCheckPoint.x = bounds.max.x;
				} else {
						frontCheckPoint.x = bounds.min.x;
				} 

				frontCheckPoint.y = bounds.center.y;
				//	Tools.CircleBlip (frontCheckPoint);
				Collider2D[] frontHits = Physics2D.OverlapPointAll (frontCheckPoint);
				
				if (frontHits.Length > 0) {
						foreach (Collider2D collider in frontHits) {
								if (collider.name == "Martha") {
										//	print ("Hurting Martha");
								}
						}
				}

		}


		void OnTriggerEnter2D (Collider2D other)
		{
				switch (other.name) {
				case "EnergyBall":
						var energyBall = other.GetComponent<EnergyBallController> ();
						energyBall.DeathByImpact ();
						Die ();
						AddForceAtPosition (energyBall.transform.position, 600f);

						break;

				default:
						break;
				}

		}


		/// <summary>
		/// EVENT MUST BE NAMED 2D. DO NOT CONFUSE WITH THE 3D VERSION !!!
		/// </summary>
		/// <param name="collision">Collision.</param>
		void OnCollisionEnter2D (Collision2D  collision)
		{				
				
			
		}

		/// <summary>
		/// Only sets the movement speed and passes it to the animator. Inherited method MoveHorizontaly handles the movement itself
		/// </summary>
		protected override void SetMovementSpeed ()
		{	
				if (!IsDead && PlatformScene.Me.IsGameOn && !IsConfused && !IsHurt) {
						if (target.x > transform.position.x) {
								movement = 1.0F;
						} else if (target.x < transform.position.x) {
								movement = -1.0F;
						}					
				} else {
						movement = 0;
						IsWalking = false;
				}		
						
				if (movement != 0) {
						IsWalking = true;				
				} 
				animator.SetFloat ("Speed", Mathf.Abs (movement));
		}

		public void HitByKick (Vector2 placeOfImpact)
		{				
				Confused (5f);
				AddForceAtPosition (placeOfImpact, 300);				
		}

		public void SlashedBySword (Vector2 damagePoint)
		{

				Hurt (0.5f, damagePoint, 150f);
				HitPoint --;
		}		

		public void Die ()
		{
				if (!IsDead) {			
						IsDead = true;				
						SFXMan.sfx_ZombieJumped.Play ();
						IsDead = true;
						var deathEffect = GetComponentInChildren<ParticleSystem> ();
						deathEffect.Play ();
						animator.SetTrigger ("Dying");	
						PlatformScene.Me.ZombiesKilled ++;
						base.Die (2.5f);
				}
		}
}
