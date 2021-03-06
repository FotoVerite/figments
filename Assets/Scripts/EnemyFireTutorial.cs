using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireTutorial : MonoBehaviour {

    public GameController gameController;
	public GameObject shot;
    private AudioSource weaponSound;

    private float nextFire = 0;
    private float fireRate = 5f;

    void Awake(){
        weaponSound = GetComponent<AudioSource>();
        gameController.NotifyLevelEvent += resetNextFireTime;

    }

    void resetNextFireTime(string levelEvent) {
        nextFire = Time.time + 2;
    }

    void Update(){
        if(gameController.levelEvent == "enemyFireTutorial" && nextFire == 0 ) {
            nextFire = Time.time;
        }
        if(gameController.levelEvent == "enemyFireTutorial" && nextFire < Time.time){
            FireWeapon();
        }
        if(gameController.levelEvent == "spawnWave") {
            Destroy(gameObject);
        }
    }

    public void FireWeapon(){
		if(gameController.speaking) {
			return;
		}
		if(Time.time < nextFire){
			return;
		}
		nextFire = Time.time + fireRate;
		GameObject instaShot = Instantiate(shot, gameObject.transform.position, gameObject.transform.rotation);
		instaShot.tag = "EnemyShotTutorial";
		weaponSound.Play();
	}

}
