using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelButton : BaseButton, IPointerEnterHandler {

    public Text levelText;
    private Image loadingImage;
    private Slider loadingBar;
    private int LevelToLoad;

    private bool locked = true;
    public void setLevel(int level){
        LevelToLoad = level;
        levelText.text = level + "";
    }

    public void setUnlocked(bool _locked) {
        locked = _locked;
        if(locked){
            main =  new Color(0.8f, 0, 0);
            highlight = new Color(1, 0, 0);
        }
        else {
            main = Color.white; 
            highlight = new Color(1, 0.5f, 0);
        }
         mat.SetColor("_OutlineColor", main);
    }

	public void ClickAsync()
	{
        if(locked) {
            return;
        }
        GameObject canvas = GameObject.Find("LevelLoader");
        loadingImage = canvas.GetComponentInChildren<Image>(true);
        loadingBar = canvas.GetComponentInChildren<Slider>(true);
        loadingImage.enabled = true;

		StartCoroutine(LoadLevelWithBar(LevelToLoad));
	}

	IEnumerator LoadLevelWithBar(int level)
	{
        AsyncOperation async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone)
		{
			loadingBar.value = async.progress;
			yield return null;
		}
	}

    public void OnPointerEnter(PointerEventData eventData)
    {   
        base.OnPointerEnter(eventData);
        if(locked) {
            TTS.StartSpeak("level " + levelText.text + " locked");
        }
        else {
            TTS.StartSpeak("level " + levelText.text);
        }
    }
}
