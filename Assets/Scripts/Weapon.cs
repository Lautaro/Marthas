using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
		MarthaController martha;

		// Use this for initialization
		void Start ()
		{
				martha = transform.parent.GetComponent<MarthaController> ();
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void OnCollisionEnter2D (Collision2D collision)
		{

        if (martha.attack != Attacks.NotAttacking && collision.gameObject.name == "ZombieBoy") {
						ZombieBoyController zombie = collision.gameObject.GetComponent<ZombieBoyController> ();

						switch (martha.attack) {
						case  Attacks.Kick:
								zombie.HitByKick (transform.position);
								break;

						case Attacks.SwordSlash:
								zombie.SlashedBySword (transform.position);
                                
								break;

						default:
								break;
						}

						
				}
		}




}
