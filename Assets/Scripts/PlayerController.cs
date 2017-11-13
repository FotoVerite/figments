using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour {


	public Rigidbody rb;
    public AudioSource weaponSound;
    public int speed;
	public int tilt;

	public int hitPoints = 3;
	public bool invincibleFrames;
	public float invincibleTimer = 2f;

	public Boundary boundary;

	public GameObject shot;
	public GameObject shield;
	private GameController gameController;
	private ScreenController screenController;
	public Transform shotSpawn;
	
	public float fireRate;

	private string[] restrictMovement = new string[] {"enemyFireTutorial", "waitForFireEvent"};
	private float nextFire;
	private bool shakeEventFired = false;

	public bool hasFiredOnce = true;

	private float shieldUptime = 1f;
	private float shieldDowntime = 5f;
	
	private AudioSource shieldAudio;
	private AudioClip shieldRaise;
	private AudioClip shieldLower;

	private AudioClip playerHit;


	// Use this for initialization
	void Awake () {

        rb = GetComponent<Rigidbody>();
        weaponSound = GetComponent<AudioSource>();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		screenController =  GameObject.Find("GameController").GetComponent<ScreenController>();
		if(shield) {
			shieldAudio = shield.GetComponent<AudioSource>();
		}
		shieldRaise = (AudioClip) Resources.Load("SFX/shield_raise");
		shieldLower = (AudioClip) Resources.Load("SFX/shield_lower");
		playerHit = (AudioClip) Resources.Load("kapow_fuzz");
		if(PlayerPrefs.GetInt("fireEvent") == 1) {
			gameController.levelEvent = "fireEvent";
			hasFiredOnce = true;
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
		GameObject instaShot = Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
		instaShot.tag = "PlayerShot";
		if(!hasFiredOnce) {
			weaponSound.PlayOneShot((AudioClip) Resources.Load("bang_normal"), 1f);
			hasFiredOnce = true;
			gameController.levelEvent = "fireEvent";
			PlayerPrefs.SetInt("fireEvent", 1);
		}
		else {
			weaponSound.Play();
		}
	}

	public void Shield() {
		if(gameController.speaking) {
			return;
		}
		if(Time.time < nextFire){
			return;
		}
		nextFire = Time.time + shieldDowntime;
		
		shield.SetActive(true);
		shieldAudio.PlayOneShot(shieldRaise);
		StartCoroutine("shutdownSheild");
	}


	void FixedUpdate()
	{
		float moveHorizontal;
		if(gameController.speaking || restrictMovement.Contains(gameController.levelEvent)) {
			rb.velocity = transform.right * 0;
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			moveHorizontal = Input.acceleration.x;
		}
		else {
			moveHorizontal = Input.GetAxis("Horizontal");
		}
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		rb.velocity = movement * speed;

		rb.position = new Vector3
	   (
		   Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
		   0.0f,
		   0.0f
	   );
	   if(rb.position.x == boundary.xMin || rb.position.x == boundary.xMax ){
			if(Application.platform == RuntimePlatform.IPhonePlayer) {
		   		Handheld.Vibrate();
      		}
		   screenController.SetOverrides(gameController.levelEvent,  gameController.spawning ? "true" : "false");
		   if(!shakeEventFired && !gameController.speaking) {
			   gameController.levelEvent = "shakeEvent";
			   shakeEventFired = true;
		   }
	   }

        rb.rotation = Quaternion.Euler(0.0f, 0.0f, rb.velocity.x * -tilt);

		InvincibleFrames();

	}

	void InvincibleFrames(){
		if(!invincibleFrames) {
			return;
		}
		invincibleTimer = invincibleTimer - Time.deltaTime;
		if(invincibleTimer < float.Epsilon){
			invincibleFrames = false;
			invincibleTimer = 2f;
		}

	}

	IEnumerator shutdownSheild() {
		yield return new WaitForSeconds(shieldRaise.length);
		shieldAudio.PlayOneShot(shieldLower);
		yield return new WaitForSeconds(shieldLower.length);
		shield.SetActive(false);
	}

	public void Hit() {
		weaponSound.PlayOneShot(playerHit);
	}

}
