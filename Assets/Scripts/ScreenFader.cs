using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ScreenFader : MonoBehaviour {
    public GameController gameController;
    public Image img;

    void Awake(){
        img.canvasRenderer.SetAlpha(1f);
    }


    public void FadeOut(float speed = 2f){
        img.color = new Color(0, 0, 0);
        img.canvasRenderer.SetAlpha( 0.0f );
        img.gameObject.SetActive(true);
        img.CrossFadeAlpha(1f, speed, true);
    }

    public void FadeIn(float speed = 4f){
        img.CrossFadeAlpha(0f, speed, false);
        StartCoroutine(SetLevelEvent("levelStart", speed + 1));
    }

     public void FadeOverlay(float speed = 4f){
        img.CrossFadeAlpha(0f, speed, true);
        img.gameObject.SetActive(false);
    }
    IEnumerator SetLevelEvent(string name, float time) {
        yield return new WaitForSeconds(time);
        gameController.levelEvent = name;
        img.gameObject.SetActive(false);
    }
}