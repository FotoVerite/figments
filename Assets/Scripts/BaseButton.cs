using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    protected bool entered = false;
    protected float counter= 0f;
    protected Color main;
    protected Color highlight;
    protected GameController gameController;
    protected Image image;
    protected Material mat;
    void Awake() {
        image = GetComponent<Image>();
        if( GameObject.Find("GameController")) {
		    gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }
        mat = Instantiate(image.material);
        image.material = mat;
        main = Color.white; 
        highlight = new Color(1, 0.5f, 0);
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

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        entered = false;
        TTS.StopSpeak();
    }
}
