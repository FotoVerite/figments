using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Vector3 spawnValues;
    public float timer = 85f;
    public Text scoreText;
    public Text timerText;
    public Text gameOverText;

    public GameObject overlay;
    public bool debug = true;
    
    private AudioSource loadingLevelPlayer;

    private Spawner spawner;
    private Clock clock;
    public GameObject textPanel;
    public GameObject[] interactivePanels;

    private int levelNumber;
    public string levelEvent {

        get { return _levelEvent; }
        set {
            if(_levelEvent == value) {return;}
             _levelEvent = value;
            if(value == "gameOver"){
                GameOver();
            }
            if(NotifyLevelEvent != null ) {
                NotifyLevelEvent(_levelEvent);
            }
        }
    
    }
    public string _levelEvent;

    public bool speaking;
    private bool gameOver;
    private bool restart;
    private bool stageComplete;
    private int score;

    private bool loadingNextStage = false;

    public delegate void NotifyLevelEventDelegate(string levelEvent);
    public event NotifyLevelEventDelegate NotifyLevelEvent;

    void Awake()
    {
        loadingLevelPlayer = GetComponents<AudioSource>()[1];
        spawner = GetComponent<Spawner>();
        clock = GetComponent<Clock>();
        interactivePanels = GameObject.FindGameObjectsWithTag("InteractivePanel");
        setInitialLevelValues();
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        UpdateScore();
        PlayerPrefs.DeleteAll();
        GetComponent<ScreenFader>().FadeIn();
    }

    void Update()
    {
        if(clock != null) {
            clock.UpdateClock(speaking || !spawner.spawning || stageComplete);
        }
        CheckRestart();
        CheckLoadNextLevel();
        CheckStageComplete();
    }

    void setInitialLevelValues()
    {
        gameOver = false;
        restart = false;
        stageComplete = false;
        gameOverText.text = "";
        score = 0;
        textPanel.SetActive(false);
        speaking = false;
        foreach (var panel in interactivePanels)
        {
            panel.SetActive(false);
        }
    }

    public void changeLevelEvent(string name) {
        levelEvent = name;
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
        spawner.StopSpawn();
        TTS.StartSpeak("Press to Restart");
        restart = true;
    }

    public void CheckStageComplete()
    {
        if(clock == null) {
            return;
        }
        if(clock.timer < float.Epsilon) {
            stageComplete = true;
        }
        else{
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

    private void CheckLoadNextLevel(){
        if(levelEvent != "nextLevel") {
            return;
        }
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut() {
        GetComponent<ScreenFader>().FadeOut();
        loadingLevelPlayer.Play();
        yield return new WaitForSeconds(2.5f);
        LoadScene(levelNumber + 1);
    }

    public void setInteractivePanels(bool activeFlag) {
        for (int i = 0; i < interactivePanels.Length; i++)
        {
            interactivePanels[i].SetActive(activeFlag);   
        }
    }

    void LoadScene(int index) {
        SceneManager.LoadScene(index);
    }

    public void LightTap() {
        Haptic.Light();
    }
}