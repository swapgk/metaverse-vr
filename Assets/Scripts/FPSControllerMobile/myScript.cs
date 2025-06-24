using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class myScript : MonoBehaviour
{
    public FixedButton fixedButton;
    public FixedTouchField fixedTouchField;
    public FixedJoystick fixedJoystick;

    private RigidbodyFirstPersonControllerWithMobile fpsWithMobile;
    private RigidbodyFirstPersonController fps;
    private GameObject canvas;
    private GameObject testCube;

    void Start()
    {
        fpsWithMobile = gameObject.GetComponent<RigidbodyFirstPersonControllerWithMobile>();
        fps = gameObject.GetComponent<RigidbodyFirstPersonController>();
        canvas = GameObject.Find("CanvasForFPSCOntrollerMobile");


        if (fpsWithMobile == null){
            Debug.Log("fpsWithMobile not found");
        }
        if (fps == null){
            Debug.Log("fps not found");
        }
        if (canvas == null){
            Debug.Log("fpsWithMobile not found");
        }

        Debug.Log("Disalbling everything");
        fpsWithMobile.enabled = false;
        fps.enabled = false;
        canvas.SetActive(false);

        #if UNITY_ANDROID
            Debug.Log("UNITY_ANDROID: Setting up for android");
            fpsWithMobile.enabled = true;       
            canvas.SetActive(true);
        #else
            Debug.Log("NOT Android:");
            fps.enabled = true;
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_ANDROID
            fpsWithMobile.RunAxis = fixedJoystick.Direction;
            fpsWithMobile.JumpAxis = fixedButton.Pressed;
            fpsWithMobile.mouseLook.lookaxis = fixedTouchField.TouchDist;
        #endif
        
    }
}
