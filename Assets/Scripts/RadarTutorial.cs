using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarTutorial : MonoBehaviour {

	public float speed;
    public GameController gameController;
    public Rigidbody rb;

    private Rigidbody shot;
    private AudioSource sound;

    void Awake(){
        sound = GetComponent<AudioSource>();
    }

    void Update(){
        if(gameController.levelEvent == "radarTutorial" && !sound.isPlaying){
            sound.Play();
        }
        if(gameController.levelEvent == "astroidField") {
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {   
        if(shot == null && GameObject.FindWithTag("PlayerShot") != null){ 
            shot = GameObject.FindWithTag("PlayerShot").GetComponent<Rigidbody>();
        }
        else if(shot) {
            float dir = AngleDir(shot.transform);
            if(shot.position.z > transform.position.z && gameController.levelEvent == "radarTutorial") {
                if(dir < -0.05f){
                    gameController.levelEvent = "shotLeft";
                }
                else if(dir > 0.05f){
                    gameController.levelEvent = "shotRight";
                }
            }
        }
    }


    void OnTriggerEnter(Collider other) {
        	if (other.tag == "PlayerShot")
		{
		gameController.levelEvent = "radarSuccess";
		}
    }

    public float AngleDir(Transform target) {
		Vector3 targetDir = target.position - transform.position;
        float degrees = Vector3.Angle(targetDir, target.right);
        return Mathf.Cos(degrees * (Mathf.PI / 180));
	}


}
