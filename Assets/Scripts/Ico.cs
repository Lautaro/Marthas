using UnityEngine;
using System.Collections;

public class Ico : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
				
		}
	
		// Update is called once per frame
		void Update ()
		{
				
	
		}

		public void AddForceAtPosition (Vector2 impactPosition, float force)
		{
				var thisPosition = new Vector2 (transform.position.x, transform.position.y);
		
				Vector2 direction = thisPosition - impactPosition;
		
				rigidbody2D.AddForceAtPosition (direction.normalized * force, impactPosition);
				//	rigidbody2D.AddForce (direction.normalized * 1000);
		
		}

		
}
