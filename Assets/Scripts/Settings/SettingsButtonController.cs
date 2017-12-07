using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsButtonController : BaseButton, IPointerClickHandler {

    public GameObject settingPanel;

     public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;
 
        if (tap == 2)
        {
            gameController.GetComponent<ScreenFader>().FadeOut(0.1f);
            Time.timeScale = 0;
            settingPanel.SetActive(true);
        }
        else {
            TTS.StartSpeak("Settings");
        }
 
    }

}
