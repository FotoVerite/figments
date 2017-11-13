using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByBoundry : MonoBehaviour {

	GameController gameController;
	void Awake(){
		gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
	}

	void OnTriggerExit(Collider other)
	{
		Destroy(other.gameObject);
		gameController.instantiateHazards.Remove(other.gameObject);
	}
}
