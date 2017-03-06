using UnityEngine;
using System.Collections;

public class PlatformPickup : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
				name = name.Replace ("(Clone)", "");
				var newX = Random.Range (-15, 15);
				var newY = Random.Range (3, 11);
				transform.position = new Vector2 (newX, newY);
				var forceX = Random.Range (-20f, 20f);
				var forceY = Random.Range (5f, 25f);
				rigidbody2D.AddForce (new Vector2 (forceX, forceY));
				Invoke ("DieOfAge", 10);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		public void Die ()
		{
				rigidbody2D.velocity = new Vector2 (0, 0);
				gameObject.collider2D.enabled = false;
				particleSystem.Stop (true);
				Destroy (gameObject, 2f);
		}

		void DieOfAge ()
		{
				SFXMan.sfx_Shot.Play ();
				Die ();
		}
}
