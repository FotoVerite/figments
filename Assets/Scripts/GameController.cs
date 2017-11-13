using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public GameObject[] hazards;
    public List<GameObject> instantiateHazards;
    public Vector3 spawnValues;

    public int hazardCount;
    public bool spawning = false;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public float timer = 85f;
    public Text scoreText;
    public Text timerText;
    public Text restartText;
    public Text gameOverText;
    
    private AudioSource soundPlayer;
    private AudioSource loadingLevelPlayer;

    private ScriptInfo scriptInfo;
    public GameObject textPanel;
    public GameObject[] interactivePanels;
    public Text character;
    public Text chacterSpeech;

    private int levelNumber;
    public string levelEvent {

        get { return _levelEvent; }
        set {
             _levelEvent = value;
        }
    
    }
    private string _levelEvent;

    public bool speaking;
    private bool gameOver;
    private bool restart;
    private bool stageComplete;
    private int score;

    private bool fastForward = false;
    private bool loadingNextStage = false;

    void Awake()
    {
        soundPlayer = GetComponents<AudioSource>()[0];
        loadingLevelPlayer = GetComponents<AudioSource>()[1];
        interactivePanels = GameObject.FindGameObjectsWithTag("InteractivePanel");
        setInitialLevelValues();
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        UpdateScore();
        StartSpawns();
        PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        SetClock();
        CheckRestart();
        CheckLoadNextLevel();
        CheckPanels();
    }

    void setInitialLevelValues()
    {
        gameOver = false;
        restart = false;
        stageComplete = false;
        restartText.text = "";
        gameOverText.text = "";
        score = 0;
        textPanel.SetActive(false);
        levelEvent = "levelStart";
        speaking = false;
    }

    void SetClock()
    {
        // don't decrement clock if characters are speaking
        if (speaking || !spawning || stageComplete)
        {
            return;
        }
        timer = timer - Time.deltaTime;
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = Mathf.Floor(timer % 60).ToString("00");
        timerText.text = minutes + ":" + seconds;
        if (timer < float.Epsilon)
        {
            stageComplete = true;
            StageComplete();
        }
    }

    public void  StartSpawns() {
        if(speaking || !spawning) {
            return;
        }
        for (int i = 0; i < instantiateHazards.Count; i++)
        {
            instantiateHazards[i].GetComponent<Mover>().Continue();
        }
        StartCoroutine("SpawnWaves");
    }

    public void StopSpawn() {
        spawning = false;
        StopCoroutine("SpawnWaves");
        for (int i = 0; i < instantiateHazards.Count; i++)
        {
            instantiateHazards[i].GetComponent<Mover>().Pause();
        }
    }
    IEnumerator SpawnWaves()
    {   
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            if(speaking) {
                break;
            }
            for (int i = 0; i < hazardCount; i++)
            {
                GameObject hazard = hazards[Random.Range(0, hazards.Length)];
                Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
                Quaternion spawnRotation = Quaternion.identity;
                instantiateHazards.Add(Instantiate(hazard, spawnPosition, spawnRotation));
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);
        }
    }

    public void AddScore(int newScoreValue)
    {
        score += newScoreValue;
        UpdateScore();
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverText.text = "Game Over!";
        gameOver = true;
        StopSpawn();
        TTS.StartSpeak("Press to Restart");
        restart = true;
    }

    public void StageComplete()
    {
        if(!stageComplete){
            return;
        }
        levelEvent = "levelEnd";
        SaveData savedData = SaveData.Load();
        savedData.score[levelNumber - 1 ] = score;
        savedData.levels[levelNumber] = false;
        SaveData.Save(savedData);
    }

    private void CheckRestart()
    {
        if (!restart)
        {
            return;
        }
        if (Input.GetButton("Fire1"))
        {
            LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void FastForwardScene() {
        fastForward = true;
    }

    private void CheckLoadNextLevel(){
        if(levelEvent != "nextLevel") {
            return;
        }
        LoadScene(levelNumber + 1);
    }

    void CheckPanels() {
        if(speaking){
            for (int i = 0; i < interactivePanels.Length; i++)
            {
             interactivePanels[i].SetActive(false);   
            }
            
        }
        else{
          for (int i = 0; i < interactivePanels.Length; i++)
            {
             interactivePanels[i].SetActive(true);   
            }
        }
    }

    void LoadScene(int index) {
        loadingLevelPlayer.Play();
        SceneManager.LoadScene(index);
    }
}