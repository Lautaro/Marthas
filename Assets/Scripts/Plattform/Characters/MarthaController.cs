using UnityEngine;
using System.Collections;

public class MarthaController : CharacterBase
{
		//Life 
		int life;
		public int Life {
				get {
						return life;
				}
				set {
						life = value;
						Hud.SetLifeCounter (value);
						if (life < 1) {
								StartCoroutine (PlatformScene.GameOver_cr ());
						}
				}
		}

		int energyBalls;
		public int EnergyBalls {
				get {
						return energyBalls;
				}
				set {
						energyBalls = value;
						Hud.SetEnergyBallCounter (value);						
				}
		} 

		int energyBallsCollected = 0;

		public int EnergyBallsCollected {
				get {
						return energyBallsCollected;
				}
				set {
						energyBallsCollected = value;
						
								EnergyBalls ++;
								SFXMan.sfx_Score.Play ();
								Hud.ShowPresentation ("ENERGY BALL");
								energyBallsCollected = 0;
						
				}
		}

		GameObject weapon;
		public Attacks attack = Attacks.NotAttacking;

		//Keyboard Controls
		KeyCode WalkLeftKey = KeyCode.A;
		KeyCode WalkRightKey = KeyCode.D;
		KeyCode KickKey = KeyCode.LeftControl;
		KeyCode JumpKey = KeyCode.Space;
		KeyCode SwordKey = KeyCode.LeftShift;

		//Energyball
		Vector3 energyBallDestination; //Where is the energy ball going to?

		protected override void Awake ()
		{				
				base.Awake ();
				weapon = transform.Find ("Weapon").gameObject;
				weapon.SetActive (false);
				EnergyBalls = 1;

				PlatformScene.Martha = this;
				MaxSpeed = 4f;
				Acceleration = 100f;
				Life = 3;				
		}

		protected override void Update ()
		{
				
				
				base.Update ();
					
				if (!IsHurt && !IsConfused) {
						//JUMP
						if (IsOnGround && Input.GetKeyDown (JumpKey)) {
										
					
								Jump ();
						}
						
						if (IsOnGround && Input.GetKeyDown (KickKey)) {
								StartCoroutine (Kick_cr ());
						}
						
						if (Input.GetKeyDown (SwordKey)) {
								StartCoroutine (SlashSword_cr ());
						}
						
						//EnergyBall Is Fired from the animation
						if (Input.GetMouseButtonDown (0)) {
						
								//ExplodeZombiesInArea ();
								energyBallDestination = Input.mousePosition;						
								animator.SetTrigger ("FireEnergyBall");			
						}
				}
									
		}

		protected override void FixedUpdate ()
		{
				base.FixedUpdate ();

		}


		void ExplodeZombiesInArea ()
		{
				var blastRadius = 10.0f;
				var blastPower = 100;
				var impactPosition = Tools.GetMouse2DWorldPosition ();
				
				//Create sphere by mouse position and get all colliders inside it
				Collider2D[] colliders = Physics2D.OverlapCircleAll (impactPosition, blastRadius); 
				Blip2D.BlipAtPosition (impactPosition, 0.2f, 1.0f, 0.5f);
				foreach (var collider in colliders) {
						
						var zombie = collider.gameObject.GetComponent<ZombieBoyController> ();
						if (zombie) {
								zombie.AddForceAtPosition (impactPosition, blastPower);
								zombie.rigidbody2D.AddForce (new Vector2 (0, blastPower));
						}						
				
						var ico = collider.gameObject.GetComponent<Ico> ();
						if (ico) {
								ico.AddForceAtPosition (impactPosition, blastPower);
						}
				}
		}

		/// <summary>
		/// Only sets the movement speed, used by the animator. Inherited method MoveHorizontaly handles the movement itself
		/// </summary>
		protected override void SetMovementSpeed ()
		{
				IsWalking = false;
				movement = 0;

				//WALK RIGHT
				if (Input.GetKey (WalkLeftKey) && !Input.GetKey (WalkRightKey)) {
						IsWalking = true;
						movement = -1; 
				}

				//WALK LEFT
				if (Input.GetKey (WalkRightKey) && !Input.GetKey (WalkLeftKey)) {
						IsWalking = true;
						movement = 1; 
				}

				animator.SetFloat ("Speed", Mathf.Abs (movement));
		}


		void OnCollisionEnter2D (Collision2D  collision)
		{
				if (!IsDead && PlatformScene.IsGameOn) {
						switch (collision.gameObject.name) {
					
						case "ZombieBoy": 
								HitByZombie (collision);
					
					
								break;
						case "PlatformPickup":
								GetPickup (collision);
								break;
						default:
								break;
						}
				}
				
		}

		void GetPickup (Collision2D collision)
		{
				EnergyBallsCollected ++;
				var energyBall = collision.gameObject.GetComponent<PlatformPickup> ();
				SFXMan.sfx_Shot.Play ();
				energyBall.Die ();
				
		}

		void HitByZombie (Collision2D collision)
		{
				if (!IsHurt) {

						var zombie = collision.gameObject.GetComponent<ZombieBoyController> ();
						var contacts = collision.contacts;
						
					
					if (zombie.renderer.bounds.max.y <= renderer.bounds.min.y + 1 && IsJumping) {
								//ZOMBIE IS BELOW MARTHA					
								if (!zombie.IsHurt) {								
										zombie.Confused (2f);

										Jump ();
								}
						} else {
								if (!zombie.IsConfused && !zombie.IsDead && attack != Attacks.Kick  ) {
                                    if (attack != Attacks.SwordSlash)
                                    {
                                        
                                    
										Life --;
										var zombiePosition = collision.gameObject.transform.position;
										SFXMan.sfx_MarthaPain.Play ();	
										AddForceAtPosition (zombie.transform.position, 300);
										rigidbody2D.AddForce (new Vector2 (0, 200));
										Hurt (1f, zombiePosition, 200f);
                                    }
                                }
						}		
				}
		}
	
		public void ReleaseEnergyBall ()
		{
				if (EnergyBalls > 0) {
						GameObject energyBall = Instantiate (Prefabs.EnergyBall) as GameObject;
						energyBall.transform.position = transform.position;
						var destination = energyBallDestination;				
						destination.z = (transform.position.z - Camera.mainCamera.transform.position.z); //The distance from the camera to the player object
						var worldMousePosition = Camera.main.ScreenToWorldPoint (destination);
						var direction = worldMousePosition - transform.position;
						Debug.DrawRay (transform.position, direction.normalized * 2.0f, Color.red, 3.0f);
						var angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
						energyBall.transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
						energyBall.rigidbody2D.AddForce (energyBall.transform.right * 500);
						EnergyBalls --;
						SFXMan.sfx_EnergyCharge.Play ();
				} else {
						SFXMan.sfx_Error.Play ();
				}

		}

		public void HorizontalFlip ()
		{
				Vector3 flipScale = transform.localScale;
				flipScale.x *= -1;
				transform.localScale = flipScale;
		}

		public IEnumerator Kick_cr ()
		{
				SFXMan.sfx_Kick.Play ();
				attack = Attacks.Kick;
				weapon.SetActive (true);
				animator.SetTrigger ("Kick");
				if (IsFacingRight) {
						rigidbody2D.AddForce (new Vector2 (300, 100));
				} else {
						rigidbody2D.AddForce (new Vector2 (-300, 100));
				}
				
				
					
				yield return new WaitForSeconds (0.5f);

				attack = Attacks.NotAttacking;
				weapon.SetActive (false);
				
		}

		IEnumerator SlashSword_cr ()
		{

				attack = Attacks.SwordSlash;
				weapon.SetActive (true);
				SFXMan.sfx_SwordWhip.Play ();
				animator.SetTrigger ("SlashSword");		 
		
		
				yield return new WaitForSeconds (0.2f);
		
				attack = Attacks.NotAttacking;
				weapon.SetActive (false);

		}	
}
public enum Attacks
{
		NotAttacking,
		Kick, 
		SwordSlash,
		EnergyBall
}
	
	


