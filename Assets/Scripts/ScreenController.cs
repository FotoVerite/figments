using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class ScreenController : MonoBehaviour
{

    public GameObject textPanel;
    public Text character;
    public Text chacterSpeech;

    public string nextEventNameOverride;
    public string startWaveOverride;
    private AudioSource soundPlayer;
    private GameController gameController;

    private ScriptInfo scriptInfo;
    private LevelScriptEvent nextEvent;
    private int levelNumber;
    private bool fastForward = false;
    private string sceneName;

    private bool endScene = false;

    void Awake()
    {
        soundPlayer = GetComponent<AudioSource>();
        gameController = GetComponent<GameController>();
        setInitialLevelValues();
        levelNumber = SceneManager.GetActiveScene().buildIndex;
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        CheckPlayScreens();
    }

    void setInitialLevelValues()
    {
        textPanel.SetActive(false);
        gameController.speaking = false;
        LoadScriptData();
    }

    IEnumerator printCharacterText()
    {
        gameController.speaking = true;
        textPanel.SetActive(true);
        for (int i = 0; i < nextEvent.screens.Count; i++)
        {   
            // deal with double click
            fastForward = false;
            if(endScene) {
                endScene = false;
               break; 
            }
            LevelScriptScreens screen = nextEvent.screens[i];
            character.text = screen.character;
            chacterSpeech.text = "";
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(DisplayCharacterText(screen));
        }
        yield return new WaitForSeconds(1f);
        EndScreens(nextEvent);
    }

    IEnumerator DisplayCharacterText(LevelScriptScreens screen){
        for (int x = 0; x < screen.text.Length; x++)
        {
            if(fastForward) {
                fastForward = false;
                chacterSpeech.text = screen.text;
                break;
            }
            chacterSpeech.text = chacterSpeech.text + screen.text[x];
            yield return new WaitForSeconds(0.12f);
        }
        if(screen.sfx != null) {
            AudioClip sfx = (AudioClip) Resources.Load(screen.sfx);
            float sfxLength = sfx.length;
            soundPlayer.PlayOneShot(sfx);
            while(!fastForward && sfxLength > 0){
                sfxLength -= Time.deltaTime;
                yield return null;
            }
            if(fastForward && soundPlayer.isPlaying){
                soundPlayer.Stop();
            }
        }
        float delay = 1.2f;
        while(!fastForward && delay > 0){
             delay -= Time.deltaTime;  //deduce time passed this frame.
             yield return null; 
        }  
    }

    private void EndScreens(LevelScriptEvent scriptEvent){
        SetNextEvent(scriptEvent.nextEvent);
        nextEvent = null;
        gameController.speaking = false;
        textPanel.SetActive(false);
        StartWave(scriptEvent.startWave);
    }

    private void CheckPlayScreens()
    {
        FindAndSetNextScreen();
        if (nextEvent == null ||  gameController.speaking)
        {
            return;
        }
        StartCoroutine(printCharacterText());
    }

    private void FindAndSetNextScreen()
    {
        if (scriptInfo.events.Count == 0 || gameController.speaking)
        {
            return;
        }
       SetScreenForEvent();
    }

    private void SetScreenForEvent(){
        for (int x = 0; x < scriptInfo.events.Count; x++)
        {
            LevelScriptEvent levelScriptEvent = scriptInfo.events[x];
            if (
                levelScriptEvent.kickoff == gameController.levelEvent || 
                (levelScriptEvent.eventType == "timeCode"  && levelScriptEvent.timeCode == Mathf.Ceil(gameController.timer))
                )
            {
                 if(PlayerPrefs.GetInt(sceneName + "_" + levelScriptEvent.kickoff) == 1 && levelScriptEvent.skip){
                    if(levelScriptEvent.skipToEvent != null) {
                        SetNextEvent(levelScriptEvent.skipToEvent);
                    }
                    else {
                        SetNextEvent(levelScriptEvent.nextEvent);
                    }
                    StartWave(levelScriptEvent.startWave);
                    continue;
                }
                nextEvent = levelScriptEvent;
                gameController.StopSpawn();
                if(levelScriptEvent.remove){
                    scriptInfo.events.RemoveAt(x);
                }
                //set intro and other skipple events are not repeated
                if(levelScriptEvent.skip) {
                    PlayerPrefs.SetInt(sceneName + "_" + levelScriptEvent.kickoff, 1);
                }
                break;
            }
        }
    }
    

    private void LoadScriptData()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, SceneManager.GetActiveScene().name + ".json");

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            scriptInfo = JsonUtility.FromJson<ScriptInfo>(dataAsJson);

        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    public void FastForwardScene() {
        fastForward = true;
    }

    public void EndScene() {
        fastForward = true;
        endScene = true;
    }

    public void SetOverrides(string _nextEventOverride = null, string _startWave = null) { 
        if(_nextEventOverride != string.Empty){
            nextEventNameOverride = _nextEventOverride;
        }
        else {
            nextEventNameOverride = gameController.levelEvent;
        }
        startWaveOverride = _startWave;
    }

    private void SetNextEvent(string nextEvent) {
        if(nextEventNameOverride != string.Empty){
            gameController.levelEvent = nextEventNameOverride;
            nextEventNameOverride = null;
        }
        else {
            gameController.levelEvent = nextEvent;
        }
    }

    private void StartWave(bool startWave) {
        if(gameController.spawning || startWaveOverride == "false"){
            startWaveOverride = null;
            return;
        }
        if(startWaveOverride == string.Empty || !startWave) {
            startWaveOverride = null;
            return;
        }
        gameController.spawning = true;
        gameController.StartSpawns();
    }
}