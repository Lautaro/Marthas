using UnityEngine;
using System.Collections;

public class Bouncer : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void OnCollisionEnter2D (Collision2D collision)
		{
				if (collision.gameObject.name == "Martha") {
						
						var martha = collision.gameObject.GetComponent<MarthaController> ();
						var forcePosition = new Vector2 (transform.position.x, martha.transform.position.y);
						martha.AddForceAtPosition (forcePosition, 400);
				}
		}
}
