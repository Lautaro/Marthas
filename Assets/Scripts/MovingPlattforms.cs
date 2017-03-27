using UnityEngine;
using System.Collections;

public class MovingPlattforms : MonoBehaviour
{

		float speed = 0.02f;
		public float startY; 
		public float endY ;
		float minSpeed = 0.1f;
		float maxSpeed = 1f;
		//public float targetY;


		// Use this for initialization
		void Start ()
		{
				speed = Random.Range (minSpeed, maxSpeed);
				var y = Random.Range (startY, endY);
				transform.position = new Vector3 (transform.position.x, y);
		}
	
		// Update is called once per frame
		void Update ()
		{
	

				if (PlatformScene.Me.IsGameOn) {
						var pos = transform.position;
						var newPos = new Vector3 (pos.x, pos.y + (speed * Time.deltaTime));
						transform.position = newPos;
				}

				if (transform.position.y > endY) {
						transform.position = new Vector3 (transform.position.x, startY);
						speed = Random.Range (minSpeed, maxSpeed);
				}
		}
}
