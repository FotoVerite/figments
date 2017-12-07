using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour {
    
    public float timer;
    public GameController gameController;
    public Text timerText;
    void Awake() {
    }
    public bool UpdateClock(bool frozen)
        {
            // don't decrement clock if characters are speaking
            if (frozen || Time.timeScale == 0)
            {
                return false;
            }
            timer = timer - Time.deltaTime;
            if(gameController.debug && timerText != null){
                string minutes = Mathf.Floor(timer / 60).ToString("00");
                string seconds = Mathf.Floor(timer % 60).ToString("00");
                timerText.text = minutes + ":" + seconds;
            }
            if (timer < float.Epsilon)
            {
               return true;
            }
            else {
             return false;
            }
        }
}
