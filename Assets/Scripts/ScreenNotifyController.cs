using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class ScreenNotifyController : MonoBehaviour
{

    public GameObject textPanel;
    public GameObject skipButton;
    public Text character;
    public Text chacterSpeech;

    public string nextEventNameOverride;
    private string startWaveOverride = null;
    private AudioSource musicPlayer;

    private AudioSource soundPlayer;
    private GameController gameController;
    private Clock clock;
    private Spawner spawner;

    private ScriptInfo scriptInfo;
    private LevelScriptEvent nextEvent;
    public string levelEvent;
    private bool fastForward = false;
    private string sceneName;

    private bool endScene = false;

    private bool sceneChanged = false;
    private IDictionary<float, string> timeEvents = new Dictionary<float, string>();
    private List<string> seenEvents = new List<string>();
    private List<LevelScriptEventCollections> eventCollections;

    void Awake()
    {
        musicPlayer = GetComponents<AudioSource>()[0];
        soundPlayer = GetComponents<AudioSource>()[1];
        gameController = GetComponent<GameController>();
        clock = GetComponent<Clock>();
        spawner = GetComponent<Spawner>();
        setInitialLevelValues();
        sceneName = SceneManager.GetActiveScene().name;
        gameController.NotifyLevelEvent += NotifySceneChanged;
    }

    void Update()
    {
        CheckForTimeEvent();
        CheckPlayScreens();
    }

    void setInitialLevelValues()
    {
        textPanel.SetActive(false);
        skipButton.SetActive(false);
        gameController.speaking = false;
        LoadScriptData();
        CalculateTimeCodes();
    }

    public void NotifySceneChanged(string _levelEvent) {
        if(CheckEventCollections()){
            Debug.Log("skipped");
            return;
        }
        levelEvent = _levelEvent;
        seenEvents.Add(levelEvent);
        sceneChanged = true;
    }

    bool CheckEventCollections() {
        foreach (var collection in eventCollections)
        {
           
            if(!collection.events.Except(seenEvents).Any()){
                eventCollections.Remove(collection);
                Debug.Log(collection.kickoff);
                gameController.levelEvent = collection.kickoff;
                return true;
            };
           
        }
        return false;
    }

    IEnumerator printCharacterText()
    {
        gameController.speaking = true;
        gameController.setInteractivePanels(false);
        textPanel.SetActive(true);
        skipButton.SetActive(true);
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
            if(screen.text[x] == ' ')   {;
                yield return new WaitForSeconds(0.07f);
            }
            else {
                yield return new WaitForSeconds(0.05f);
            };
        }
        if(screen.music != null) {
             AudioClip music = (AudioClip) Resources.Load(screen.music);
             musicPlayer.clip = music;
             musicPlayer.loop = true;
             musicPlayer.Play();
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
        if(screen.musicVolume != 0){
            musicPlayer.volume = screen.musicVolume;
        }
        float delay = 1.2f;
        while(!fastForward && delay > 0){
             delay -= Time.deltaTime;  //deduce time passed this frame.
             yield return null; 
        }  
    }
    
    private void CalculateTimeCodes() {
         for (int x = 0; x < scriptInfo.events.Count; x++) {
            LevelScriptEvent levelScriptEvent = scriptInfo.events[x];
            if(levelScriptEvent.eventType == "timeCode") {
                timeEvents.Add(levelScriptEvent.timeCode, levelScriptEvent.kickoff);
            }
         }

    }

    private void CheckForTimeEvent() {
        if(clock == null) {
            return;
        }
        if(timeEvents.Count == 0) {
            return;
        }
        if(timeEvents.ContainsKey(Mathf.Ceil(clock.timer))) {
            string eventName = timeEvents[Mathf.Ceil(clock.timer)];
             NotifySceneChanged(eventName);
            timeEvents.Remove(Mathf.Ceil(clock.timer));
        }

    }

    private void EndScreens(LevelScriptEvent scriptEvent){
        nextEvent = null;
        levelEvent  = null;
        gameController.speaking = false;
        gameController.setInteractivePanels(true);
        textPanel.SetActive(false);
        skipButton.SetActive(false);
        SetNextEvent(scriptEvent.nextEvent);
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
        if (scriptInfo.events.Count == 0 || gameController.speaking || sceneChanged == false)
        {
            return;
        }
       SetScreenForEvent();
    }

    private void SetScreenForEvent(){
        Debug.Log("called with " + levelEvent);
        for (int x = 0; x < scriptInfo.events.Count; x++)
        {
            LevelScriptEvent levelScriptEvent = scriptInfo.events[x];
            if (
                levelScriptEvent.kickoff == levelEvent || 
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
                    if(levelScriptEvent.music != null && levelScriptEvent.music != musicPlayer.clip.name) {
                        AudioClip music = (AudioClip) Resources.Load(levelScriptEvent.music);
                        musicPlayer.clip = music;
                        musicPlayer.loop = true;
                        musicPlayer.Play();
                    }
                    StartWave(levelScriptEvent.startWave);
                    continue;
                }
                nextEvent = levelScriptEvent;
                if(spawner != null) {
                    spawner.StopSpawn();
                }
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
        sceneChanged = false;
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
            eventCollections = scriptInfo.eventCollections;

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
        sceneChanged = false;
        if(nextEventNameOverride != string.Empty){
            gameController.levelEvent = nextEventNameOverride;
            nextEventNameOverride = "";
        }
        else {
            gameController.levelEvent = nextEvent;
        }
    }

    private void StartWave(bool startWave) {
        Debug.Log(startWaveOverride);
        if(spawner == null) {
            return;
        }
        if(spawner.spawning || startWaveOverride == "false"){
            startWaveOverride = null;
            return;
        }
        if(startWaveOverride == string.Empty || !startWave) {
            startWaveOverride = null;
            return;
        }
        spawner.StartSpawns();
    }
}