using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundry : MonoBehaviour {

	Spawner spawner;
	void Awake(){
		spawner = GameObject.FindWithTag("GameController").GetComponent<Spawner>();
	}

	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject);
		spawner.instantiateHazards.Remove(other.gameObject);
	}
}
