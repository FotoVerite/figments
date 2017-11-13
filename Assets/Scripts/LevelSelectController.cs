using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelSelectController : MonoBehaviour {

    public GameObject levelSelectButton;
    public GameObject canvas; 

	// Use this for initialization
	void Awake () {
        TTS.Settings();
        SaveData savedData = SaveData.Load();

        for (int i = 0; i < savedData.levels.Length; i++)
        {
            GameObject spawnedLevelSelectButton = Instantiate(levelSelectButton);
            LevelButton buttonScript = spawnedLevelSelectButton.GetComponent<LevelButton>();
            buttonScript.setLevel(i + 1);
            buttonScript.setUnlocked(savedData.levels[i]);
            spawnedLevelSelectButton.transform.SetParent(canvas.transform);
            spawnedLevelSelectButton.transform.localScale = new Vector3(1,1,1);
            spawnedLevelSelectButton.transform.localPosition = new Vector3(1,1,1);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
