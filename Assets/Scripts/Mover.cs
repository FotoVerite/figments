using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

	public float speed;
    public Rigidbody rb;

    private Rigidbody player;
    private GameController gameController;
    private AudioSource sound;

    private float tapWait = 0;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponentInParent<Rigidbody>();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
        sound = GetComponent<AudioSource>();
        float[] randomeArray = {Random.Range(-1.25f, -1f), Random.Range(1f, 1.25f)};
        float pitch =randomeArray[Random.Range(0, randomeArray.Length - 1)];
        if(sound != null){
            if (System.Math.Abs(pitch) > Mathf.Epsilon) {
                sound.pitch = pitch;
            }
        }
    }

    void Start()
	{
		rb.velocity = transform.forward * speed;
	}

    private void FixedUpdate()
    {   
        if(player == null) {
            return;
        }
        float distance = Vector3.Distance(player.position, transform.position);
        if(sound != null) {
		    sound.panStereo = AngleDir(player.transform);
            if(Mathf.Abs(AngleDir(player.transform)) < 0.2f && distance < 4) {
                tapLight();
            }
        }
        tapWait = tapWait - Time.deltaTime;
    }


    public float AngleDir(Transform target) {
		Vector3 targetDir = target.position - transform.position;
        float degrees = Vector3.Angle(targetDir, target.right);
        return Mathf.Cos(degrees * (Mathf.PI / 180));
	}

    public void Pause() {
        rb.velocity = transform.forward * 0;
        if(sound != null){
            sound.Stop();
        }
    }

    public void Continue(){
        rb.velocity = player.transform.forward * speed;
        if(sound != null){
            sound.Play();
        }
    }

    private void tapLight() {
        if(tapWait > 0 || gameController.speaking) {
            return;
        }
        Haptic.Light();
        tapWait = 10f;
    }


}
