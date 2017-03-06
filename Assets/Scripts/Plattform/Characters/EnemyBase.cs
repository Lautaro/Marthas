using UnityEngine;
using System.Collections;

public class EnemyBase : CharacterBase
{
		protected Vector3 target; 

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		protected override void Update ()
		{
				// Check Marthas position.
				if (PlatformScene.Martha) {
			
				
						target = PlatformScene.Martha.transform.position;
				
						base.Update ();	
				}
		}
}
