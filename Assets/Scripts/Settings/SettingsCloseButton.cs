using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SettingsCloseButton : BaseButton, IPointerClickHandler {

    public GameObject settingPanel;

     public void OnPointerClick(PointerEventData eventData)
    {
        int tap = eventData.clickCount;
 
        if (tap == 2)
        {
            Time.timeScale = 1;
            gameController.GetComponent<ScreenFader>().FadeOverlay(0.1f);
            settingPanel.SetActive(false);
        }
        else {
            TTS.StartSpeak("Close");
        }
 
    }

}
