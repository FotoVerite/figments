using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class QuitGame : BaseButton, IPointerClickHandler {

     public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;
 
        if (tap == 2)
        {
            Application.Quit();
        }
        else {
            TTS.StartSpeak("Quit");
        }
 
    }

}
