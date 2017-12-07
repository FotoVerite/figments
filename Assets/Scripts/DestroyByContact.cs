using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private GameController gameController;
	private Spawner spawner;
	private PlayerController playerController;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindWithTag("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent<GameController>();
			spawner = gameControllerObject.GetComponent<Spawner>();

		}
		if (gameController == null)
		{
			Debug.Log("Cannot find 'GameController' script");
		}

		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
		{
			playerController = player.GetComponent<PlayerController>();
		}
		if (playerController == null)
		{
			Debug.Log("Cannot find 'PlayerController' script");
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if(gameController.speaking){
			return;
		}
		if (other.CompareTag("Boundary") || other.CompareTag("Enemy"))
		{
			return;
		}

		if (other.CompareTag("Shield") )
		{
			if(gameObject.tag == "EnemyShotTutorial") {
				gameController.levelEvent = "shieldRaised";
				playerController.StopCoroutine("shutdownSheild");
				playerController.shield.SetActive(false);
			}
			if(spawner.instantiateHazards.Contains(gameObject)) {
				spawner.instantiateHazards.Remove(gameObject);
			}
			gameController.AddScore(scoreValue);
			Destroy(gameObject);
			return;
		}

		if (explosion != null &&  other.tag !="Player")
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}

		if (other.tag == "Player")
		{
			if(gameObject.tag == "EnemyShotTutorial") {
				gameController.levelEvent = "shieldNotRaised";
				Destroy(gameObject);
				return;
			}
			
			if(playerController.invincibleFrames){
				Instantiate(explosion, transform.position, transform.rotation);
				return;
			}
			spawner.instantiateHazards.Remove(gameObject);
			playerController.hitPoints--;
			if(playerController.hitPoints == 0){
				//Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
				Destroy(other.gameObject);
				gameController.GameOver();
			}
			else{
				gameController.levelEvent = "playerHit";
				playerController.invincibleFrames = true;
				Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
			}
			Destroy(gameObject);
			return;
		}
		
		else{ 
			gameController.AddScore(scoreValue);
			spawner.instantiateHazards.Remove(gameObject);
			Destroy(other.gameObject);
			Destroy(gameObject);
			return;

		}
	}
}