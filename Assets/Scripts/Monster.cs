using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

	public string monsterName;
	public float spawnTimeTillPlayerDeath = 15f;
	public float spawnTillAttack = 3f;
	public string killCondition;
	public bool killInstant = false;
	private float spawnFor = 0f;

	private float interval = 1f;
	private float nextInterval = 1f;
	GameController gameController;
	Spawner spawner;
	AudioSource soundController;

	private bool paused = false;

	public void Kill() {
		spawner.instantiateHazards.Remove(gameObject);
		Destroy(gameObject);
	}
	void Awake () {
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		spawner = gameController.GetComponent<Spawner>();
		soundController = GetComponent<AudioSource>();
	}

	public void Pause(){
		paused = true;
	}

	public void Continue(){
		paused  = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(paused) {
			return;
		}
		if(spawnFor < nextInterval) {
			spawnFor = spawnFor + Time.deltaTime;
			return;
		}
		nextInterval = nextInterval + interval;
		if(spawnFor < spawnTillAttack){
			spawnFor = spawnFor + Time.deltaTime;
			return;
		}
		spawnFor = spawnFor + Time.deltaTime;
		if(Random.value > .95){
			soundController.Play();
		}
		if(gameController.levelEvent == killCondition && killInstant){
			Kill();
			spawner.restartSpawns();
			return;
		}
		if(gameController.levelEvent == killCondition && spawnTimeTillPlayerDeath > spawnFor){
			if(Random.value > .8){
				soundController.Play();
			}
			if(Random.value > .8){
				Haptic.Medium();
			}
			return;
		}
		if(gameController.levelEvent == killCondition && spawnTimeTillPlayerDeath < spawnFor){
        	Haptic.Heavy();
			Kill();
			return;
		}

		if(gameController.levelEvent != killCondition) {
			if(Random.value > .75){
				AudioClip sound = (AudioClip) Resources.Load("SFX/chomp");
				soundController.clip = sound;
				soundController.Play();
				Debug.Log("Dead");
				gameController.levelEvent = "eaten" + monsterName;
			}
		}
		return;
		
	}
}
