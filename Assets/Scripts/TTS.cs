using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class TTS : MonoBehaviour {

	     //connect to the extern method in Objective-C
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _StartSpeak(string _message);

    [DllImport("__Internal")]
    private static extern void _SettingSpeak(string _language, float _pitch, float _rate);
    
    [DllImport("__Internal")]
    private static extern void _StopSpeak();
#endif

    public static void Settings() {
       if(Application.platform == RuntimePlatform.IPhonePlayer) {
        _SettingSpeak("en-US", 1f, 0.6f);
       }
    }

     //Call the method from Unity3D.
     public static void StartSpeak(string text) {
		 // Call plugin only when running on real device
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_StartSpeak(text);
	 }

     public static void StopSpeak() {
		 // Call plugin only when running on real device
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_StopSpeak();
	 }
}
