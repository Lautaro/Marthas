using UnityEngine;
using System.Collections;

public class Prefabs : MonoBehaviour
{

		static public GameObject ZombieBoy ;
		static public GameObject Martha ;
		static public GameObject EnergyBall ;
		static public GameObject Pickup ;
		static public GameObject CardPickup ;
		string EnemyPrefabPath = "Prefabs/Enemies/";


		void Awake ()
		{
				ZombieBoy = Resources.Load (EnemyPrefabPath + "ZombieBoy") as GameObject;
				Martha = Resources.Load ("Prefabs/Martha/Martha") as GameObject;
				EnergyBall = Resources.Load ("Prefabs/Martha/EnergyBall") as GameObject;
				Pickup = Resources.Load ("Prefabs/Misc/PlatformPickup") as GameObject;
				
		}

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
