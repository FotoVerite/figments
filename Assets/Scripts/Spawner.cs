using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

    public int startWait = 5;
    public int spawnWait = 5;
    public int waveWait = 15;
    public bool spawning = false;

    public GameObject[] hazards;
    public int hazardCount = 1;

    public Vector3 spawnValues;
    public List<GameObject> instantiateHazards;

    private  GameController gameController;


    void Awake()
    {
        gameController = GetComponent<GameController>();
    }

    public void  StartSpawns() {
        spawning = true;
        if(gameController.speaking) {
            return;
        }
        for (int i = 0; i < instantiateHazards.Count; i++)
        {
            if( instantiateHazards[i].GetComponent<Mover>() != null ) {
                instantiateHazards[i].GetComponent<Mover>().Continue();
            }
            if( instantiateHazards[i].GetComponent<Monster>() != null ) {
                instantiateHazards[i].GetComponent<Monster>().Continue();
            }
        }
        StartCoroutine("SpawnWaves");
    }

    public void StopSpawn() {
        spawning = false;
        StopCoroutine("SpawnWaves");
        for (int i = 0; i < instantiateHazards.Count; i++)
        {
            if( instantiateHazards[i].GetComponent<Mover>() != null ) {
                instantiateHazards[i].GetComponent<Mover>().Pause();
            }
            if( instantiateHazards[i].GetComponent<Monster>() != null ) {
                instantiateHazards[i].GetComponent<Monster>().Pause();
            }
        }
    }

    public void restartSpawns() {
        StopCoroutine("SpawnWaves");
        for (int i = 0; i < instantiateHazards.Count; i++) {
            GameObject g = instantiateHazards[i];
            instantiateHazards.Remove(g);
            Destroy(g);
        }
        StartCoroutine("SpawnWaves");
    }


    IEnumerator SpawnWaves()
    {   
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            if(gameController.speaking) {
                break;
            }
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject enemy = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                instantiateHazards.Add(Instantiate(enemy, spawnPosition, spawnRotation));
                if(hazardCount > 1) {
                    yield return new WaitForSeconds(spawnWait);
                }
            }
            yield return new WaitForSeconds(waveWait);
        }
    }    

}