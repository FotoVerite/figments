using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelectButton : BaseButton, IPointerClickHandler {

     public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;
 
        if (tap == 2)
        {
            SceneManager.LoadScene(0);
        }
        else {
            TTS.StartSpeak("Level Select");
        }
 
    }

}
