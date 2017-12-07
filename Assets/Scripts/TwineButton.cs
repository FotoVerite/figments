using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TwineButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Text buttonText;
    private Image loadingImage;
    private Slider loadingBar;
    private int LevelToLoad;
    private bool entered = false;
    private float counter= 0f;
    private Color main;
    private Color highlight;
    private GameController gameController;
    private TwineButtons twineButtons;

    private string nextEvent;
    private bool seen = false;

    Image image;
    Material mat;
    void Awake() {
        image = GetComponent<Image>();
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
        twineButtons = gameController.GetComponent<TwineButtons>();
        mat = Instantiate(image.material);
        image.material = mat;
        main = Color.white; 
        highlight = new Color(1, 0.5f, 0);
    }

    public void CheckIfPreviouslySeen(string nextEvent) {
        seen = twineButtons.seenChoices.Contains(nextEvent);
        if(seen){
            main =  new Color(0.8f, 0, 0);
            highlight = new Color(1, 0, 0);
        }
        else {
            main = Color.white; 
            highlight = new Color(1, 0.5f, 0);
        }
         mat.SetColor("_OutlineColor", main);
    }

    public void setButton(string _buttonText, string _nextEvent){
        buttonText.text = _buttonText;
        nextEvent = _nextEvent;
        CheckIfPreviouslySeen(nextEvent);
    }



    public void OnPointerEnter(PointerEventData eventData)
    {   
        entered= true;
        counter=0f;
        Haptic.Light();
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

     public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;
 
        if (tap == 2)
        {
            if(!twineButtons.seenChoices.Contains(nextEvent)){
                twineButtons.seenChoices.Add(nextEvent);
            }
            gameController.levelEvent = nextEvent;
        }
        else {
            TTS.StartSpeak(buttonText.text);
        }
 
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        entered = false;
        TTS.StopSpeak();
    }
}
