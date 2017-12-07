using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TwineButtons : MonoBehaviour
{

    public GameObject buttonPanel;
    public string levelEvent;
    public bool sceneChanged;
    private List<TwineChoices> buttons;
    private TwineEvents twineEvents;
    public GameObject twineButton;
    private GameController gameController;
    public List<string> seenChoices = new List<string>();


    void Awake()
    {   
        gameController = GetComponent<GameController>();
        setInitialLevelValues();
        gameController.NotifyLevelEvent += NotifySceneChanged;
    }

    void Update()
    {
        AddButtons();
    }

    void setInitialLevelValues()
    {
        LoadScriptData();
    }

    public void NotifySceneChanged(string _levelEvent) {
        levelEvent = _levelEvent;
        sceneChanged = true;
    }


    private void AddButtons()
    {
        if (!sceneChanged)
        {
            return;
        }
        foreach(Transform button in buttonPanel.GetComponentInChildren<Transform>()) {
            Destroy(button.gameObject);
        }
        buttons = FindTwineEvent(levelEvent);
        foreach (var button in buttons)
        {
            
            GameObject newTwineButton = Instantiate(twineButton);
            newTwineButton.GetComponent<TwineButton>().setButton(button.buttonText, button.eventName);
            newTwineButton.transform.SetParent(buttonPanel.transform, false);
        }
        sceneChanged = false;
    }

    private void FindAndSetNextScreen()
    {
        if (gameController.speaking || sceneChanged == false)
        {
            return;
        }
       
    }

    private List<TwineChoices> FindTwineEvent(string eventName){
        for (int x = 0; x < twineEvents.events.Count; x++)
        {
            TwineEvent twineEvent = twineEvents.events[x];
            if ( twineEvent.kickoff == eventName) {
                return twineEvent.choices;
            }
        }
        return new List<TwineChoices>();
    }
    

    private void LoadScriptData()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, SceneManager.GetActiveScene().name + "Choices.json");

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            twineEvents = JsonUtility.FromJson<TwineEvents>(dataAsJson);
        }
        else
        {
            Debug.LogError("Cannot load twine choice data!");
        }
    }

}