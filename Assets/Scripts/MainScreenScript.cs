using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using agora_gaming_rtc;

public class MainScreenScript : MonoBehaviour
{

    static PhotonView photonView_;
    static string agoraID;

    [SerializeField]
    public Material myMaterial;
 
    Button button ;

    void Start() {
        photonView_ = PhotonView.Get(this);
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }

    public static void setAgoraID(string myagoraID){
        MainScreenScript.agoraID = myagoraID;
    }

    public static void  AddVideoToMainDisplay_(){
        Debug.Log("MainScreenScript->AddVideoToMainDisplay_, calling RPC ->AddVideoToMainDisplay");
        photonView_.RPC("AddVideoToMainDisplay", RpcTarget.AllBufferedViaServer, agoraID);

    }

    public static void  RemoveVideoFromMainDisplay_(){
        Debug.Log("MainScreenScript->RemoveVideoFromMainDisplay_, calling RPC ->RemoveVideoFromMainDisplay");
        photonView_.RPC("RemoveVideoFromMainDisplay", RpcTarget.AllBufferedViaServer, agoraID);

    }

    // photonView_.RPC("PrecessLeftMouseClick", RpcTarget.AllBufferedViaServer, pointerPos);

     


    [PunRPC]
    void AddVideoToMainDisplay(string agoraID_)
    {
        Debug.Log("MainScreenScript->AddVideoToMainDisplay, agoraID_:"+ agoraID_);

        // create a GameObject and assign to this new user
        // VideoSurface videoSurface = makeImageSurfaceMultiplayer(agoraID_);

        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        // go.name = agoraID_;
        // go.transform.parent = gameObject.transform;
        // go.transform.localPosition = new Vector3(0f, 2f,0f);
        // go.transform.localScale= new Vector3(1,1,1);
        // go.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // go.AddComponent<RawImage>();
        // go.AddComponent<VideoSurface>();

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurfaceMultiplayer(agoraID_);

        if(agoraID == agoraID_){
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            return;
        }
        if (!ReferenceEquals(videoSurface, null))
        {
            Debug.Log("MainScreenScript->AddVideoToMainDisplay, configuring main screen");

            // configure videoSurface
            videoSurface.SetForUser(uint.Parse(agoraID_));
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurface.SetGameFps(30);
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            videoSurface.SetEnable(true);


        }
    }

   
    // Author: Swapnil K
    // Purpose: Update according to multiplayer feature
    private const float Offset = 100;
    public VideoSurface makeImageSurfaceMultiplayer(string goName)
    {
        Debug.Log("MainScreenScript->makeImageSurfaceMultiplayer:");
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = goName;
        go.transform.parent = gameObject.transform;
        go.transform.localPosition = new Vector3(0f, 1f,0f);
        go.transform.localScale= new Vector3(1,1,1);
        go.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        go.AddComponent<RawImage>();
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();

        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        
        meshRenderer.material = null;
        Debug.Log("makeImageSurfaceMultiplayer -> Screen Material name is "+ meshRenderer.material.name);
        meshRenderer.material = null;
        Debug.Log("makeImageSurfaceMultiplayer -> Screen Material name is "+ meshRenderer.material.name);

        go.GetComponent<MeshRenderer>().material = myMaterial;
        string materialName =  go.GetComponent<MeshRenderer>().material.name;
        Debug.Log("makeImageSurfaceMultiplayer -> Screen Material name is "+ materialName);
        return videoSurface;
    }


    [PunRPC]
    void RemoveVideoFromMainDisplay(string agoraID_)
    {
        Debug.Log("MainScreenScript->RemoveVideoFromMainDisplay");
        RawImage ri = gameObject.GetComponent<RawImage>();
        VideoSurface vs = gameObject.GetComponent<VideoSurface>();
        GameObject go = gameObject.transform.Find(agoraID_).gameObject;
        if(ri!= null){
            Destroy(ri);
        }
        if(vs!= null){
            Destroy(vs);
        }
        if(go!= null){
            Destroy(go);
        }

    }
}
