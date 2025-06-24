using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class PlayerConfig : MonoBehaviourPunCallbacks
{
    public GameObject playerCamera;
    private PhotonView PV;

    public FixedButton fixedButton;
    public FixedTouchField fixedTouchField;
    public FixedJoystick fixedJoystick;

    private RigidbodyFirstPersonControllerWithMobile fpsControllerWithMobile;
    private RigidbodyFirstPersonController fpsController;
    private GameObject canvas;
    private ManageItems manageItems;
    // private ManageItems D;

    private Rigidbody playerRigidbody;
   
        
    private void Start()
    {  
        PV = GetComponent<PhotonView>();
        playerRigidbody = GetComponent<Rigidbody>();
        
        Camera myCamera = playerCamera.GetComponent<Camera>();
        AudioListener audioListener = playerCamera.GetComponent<AudioListener>();

        // Enable Mobile control in Mobile
        fpsControllerWithMobile = gameObject.GetComponent<RigidbodyFirstPersonControllerWithMobile>();
        fpsController = gameObject.GetComponent<RigidbodyFirstPersonController>();
        manageItems = gameObject.GetComponent<ManageItems>();

        // canvas = GameObject.Find("CanvasForFPSCOntrollerMobile");
        canvas = gameObject.transform.Find("CanvasForFPSControllerMobile").gameObject;
        
        if (fpsControllerWithMobile == null){
            Debug.Log("fpsControllerWithMobile not found");
        }
        if (fpsController == null){
            Debug.Log("fpsController not found");
        }
        if (canvas == null){
            Debug.Log("canvas not found");
        }

        Debug.Log("Disalbling everything");
        fpsControllerWithMobile.enabled = false;
        fpsController.enabled = false;
        canvas.SetActive(false);

        // myPhotoViewEnableControl(myCamera, audioListener);

        if (PV.IsMine)
        {
            myPhotoViewEnableControl(myCamera, audioListener);

        }
        else
        {
            notMyPhotonViewDisableControls(myCamera, audioListener);
            notMyPhotonViewDisablePhysics();
        }
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_ANDROID
            fpsControllerWithMobile.RunAxis = fixedJoystick.Direction;
            fpsControllerWithMobile.JumpAxis = fixedButton.Pressed;
            fpsControllerWithMobile.mouseLook.lookaxis = fixedTouchField.TouchDist;
        #endif
        
    }

    private void myPhotoViewEnableControl(Camera myCamera, AudioListener audioListener)
    {
        #if UNITY_ANDROID
            Debug.Log("UNITY_ANDROID: Setting up for android");
            fpsControllerWithMobile.enabled = true;     
            fpsController.enabled = false;
            canvas.SetActive(true);
        #else
            Debug.Log("NOT Android:");
            fpsController.enabled = true;
            fpsControllerWithMobile.enabled = false;
            canvas.SetActive(false);
        #endif

            // myCamera.enabled = true;
            // audioListener.enabled = true;
            // print("Correct PhotonView" + PV.ViewID);
    }

    private void notMyPhotonViewDisableControls(Camera myCamera, AudioListener audioListener)
    {
        fpsController.enabled = false;
        myCamera.enabled = false;
        audioListener.enabled = false;
        fpsControllerWithMobile.enabled = false;
        canvas.SetActive(false);
        manageItems.enabled = false;

        print("Incorrect PhotonView" + PV.ViewID);
    }

    private void notMyPhotonViewDisablePhysics(){
            playerRigidbody.isKinematic = true;
    }
    
}
