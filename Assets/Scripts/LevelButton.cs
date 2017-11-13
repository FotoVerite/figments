using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Text levelText;
    private Image loadingImage;
    private Slider loadingBar;
    private int LevelToLoad;
    private bool entered = false;
    private float counter= 0f;
    private Color main;
    private Color highlight;

    private bool locked = true;

    Image image;
    Material mat;
    void Awake() {
        image = GetComponent<Image>();
        mat = Instantiate(image.material);
        image.material = mat;
    }

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
        entered= true;
        counter=0f;
        Haptic.Heavy();
        if(locked) {
            TTS.StartSpeak("level " + levelText.text + " locked");
        }
        else {
            TTS.StartSpeak("level " + levelText.text);
        }
    }

    void Update(){
        Color lerpedColor;
        lerpedColor = Color.Lerp(main, highlight, Mathf.PingPong(counter, 1));
        if(lerpedColor == main){
            counter = 0f;
        }
        if(entered){
            mat.SetColor("_OutlineColor", lerpedColor);
            counter+=.05f;
        }
        else if(!entered && lerpedColor != main){
            mat.SetColor("_OutlineColor", lerpedColor);
            counter+=.05f;
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        entered = false;
        TTS.StopSpeak();
    }
}
