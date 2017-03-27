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
				if (PlatformScene.Me.Martha) {
			
				
						target = PlatformScene.Me.Martha.transform.position;
				
						base.Update ();	
				}
		}
}
