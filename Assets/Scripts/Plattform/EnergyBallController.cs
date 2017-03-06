using UnityEngine;
using System.Collections;

public class EnergyBallController : MonoBehaviour
{

		float lifeTimer;
		float lifeSpan = 1.5f;
		ParticleSystem deathEffect;

		void Awake ()
		{
				name = name.Replace ("(Clone)", "");
				deathEffect = transform.FindChild ("DeathEffect").GetComponent<ParticleSystem> ();
				if (deathEffect == null)
						Debug.LogWarning ("deathEffect reference is missing in EnergyBallController");
				deathEffect.Stop ();


				lifeTimer = Time.time;
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{

				if (Time.time - lifeTimer > lifeSpan) {

						DeathByTimeOut ();
				}
	
		}

		public	void DeathByImpact ()
		{
				//EXPLODE
				
				lifeTimer = 0; //so it doesnt die before death effect
				var rigidbody = GetComponent<Rigidbody2D> ();
				rigidbody.velocity = Vector2.zero;
				var collider = GetComponent<CircleCollider2D> ();
				collider.enabled = false;
				var energyBallParticleSystem = GetComponent<ParticleSystem> ();
				var renderer = GetComponent<SpriteRenderer> ();
				
				energyBallParticleSystem.Stop ();
				deathEffect.Play ();
				SFXMan.sfx_Explosion.Play ();
				Destroy (gameObject, 2.0f);				
		}


		void DeathByTimeOut ()
		{
				var energyBallParticleSystem = GetComponent<ParticleSystem> ();
				var renderer = GetComponent<SpriteRenderer> ();
				
				energyBallParticleSystem.Stop ();
				var tailParticleSystem = transform.Find ("Tail").GetComponent<ParticleSystem> ();
				tailParticleSystem.Stop ();
				Destroy (gameObject, 1.0f);
		}
}
