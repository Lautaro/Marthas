using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions
{

		public  static void LineBlip (this GameObject self)
		{
				var position = self.transform.position;

				var newPosition = new Vector3 (position.y, position.x + 1, position.z);		
		
				Debug.DrawLine (position, newPosition, Color.red, 0.5F, true);	
		}
}

