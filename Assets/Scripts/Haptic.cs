using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;


public class Haptic : MonoBehaviour {

	     //connect to the extern method in Objective-C
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _Heavy();

    [DllImport("__Internal")]
    private static extern void _Medium();
    
    [DllImport("__Internal")]
    private static extern void _Light();
#endif

    public static void Light() {
       if(Application.platform == RuntimePlatform.IPhonePlayer) {
           _Light();
       }
    }

     public static void Medium() {
       if(Application.platform == RuntimePlatform.IPhonePlayer) {
           _Medium();
       }
    }

     public static void Heavy() {
       if(Application.platform == RuntimePlatform.IPhonePlayer) {
           _Heavy();
       }
    }
}
