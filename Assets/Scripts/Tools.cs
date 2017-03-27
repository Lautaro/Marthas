using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tools : MonoBehaviour
{
	#region
		public static Tools Me;
	#endregion

		

		void Awake ()
		{
				Me = this;
		}
		/// <summary>
		/// TO get the mouse worldposition the distance from the camera to the desired Z plane. 
		/// 2D does not use z. So this will work if the z plane used is 0. 
		/// </summary>
		/// <returns>The mouse2 D world position.</returns>
		public static Vector2 GetMouse2DWorldPosition ()
		{
				
				var mousePos = Input.mousePosition;				
				mousePos.z = -Camera.main.transform.position.z; //The distance from z=0 to the camera. 
				var worldMousePosition = Camera.main.ScreenToWorldPoint (mousePos);
				return new Vector2 (worldMousePosition.x, worldMousePosition.y);
		}


		
		#region Blips
		public void SphereBlip (Vector2 position)
		{
				StartCoroutine (SphereBlip_cr (position));
		}

		public void SphereBlip (Vector3 position)
		{
				StartCoroutine (SphereBlip_cr (new Vector2 (position.x, position.y)));
		}

		IEnumerator SphereBlip_cr (Vector2 position)
		{
				
				var sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphere.transform.position = new Vector3 (position.x, position.y, -30);//make sure the depth is in front of camera 

				sphere.GetComponent<Collider>().enabled = false;
				
				sphere.GetComponent<Renderer>().material.shader = Shader.Find ("Transparent/Diffuse");

				var alpha = sphere.GetComponent<Renderer>().material.color.a; 

				var t = Time.deltaTime;
				var durationSeconds = 1.6F; 
				var material = sphere.GetComponent<Renderer>().material;
				var sphereTransform = sphere.transform;
				var targetSize = 10;

				while (t < durationSeconds) {
								
						var add = Mathf.Lerp (1, targetSize, t / durationSeconds); //regulates size per cycle
						sphereTransform.localScale = new Vector3 (add, add, add);
						var newAlpha = durationSeconds / t / 2;
						
						material.color = new Color (1F, 1F, 1F, newAlpha);// t / durationSeconds;

						yield return null;
			
						t += Time.deltaTime; // add time for each cycle
				}
				
				DestroyImmediate (sphere);		
		}

//		public void CircleBlip (Vector2 position)
//		{
//				StartCoroutine (CircleBlip_cr (position));
//		}
//
//		IEnumerator CircleBlip_cr (Vector2 position)
//		{
//				GameObject circle = null;// Instantiate (circleBlipPrefab, position, Quaternion.identity) as GameObject;
//					
//				var alpha = circle.renderer.material.color.a; 
//
//				var t = Time.deltaTime;
//				var durationSeconds = 0.6F; 
//				var material = circle.renderer.material;
//				var circleTransform = circle.transform;
//				var targetSize = 0.1f;
//
//				while (t < durationSeconds) {
//			
//						var add = Mathf.Lerp (0, targetSize, t / durationSeconds); //regulates size per cycle
//						circleTransform.localScale = new Vector3 (add, add, add);
//						var newAlpha = durationSeconds / t / 2;
//			
//						material.color = new Color (1F, 1F, 1F, newAlpha);// t / durationSeconds;
//			
//						yield return null;
//			
//						t += Time.deltaTime; // add time for each cycle
//				}
//
//				DestroyImmediate (circle);
//
//		}



		#endregion
}
