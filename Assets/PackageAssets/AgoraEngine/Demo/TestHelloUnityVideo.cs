using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;  

using agora_gaming_rtc;
using agora_utilities;

// To get agora ID from photon view
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

using System.Linq;

// this is an example of using Agora Unity SDK
// It demonstrates:
// How to enable video
// How to join/leave channel
// 
public class TestHelloUnityVideo : MonoBehaviour
{

    // instance of agora engine
    private IRtcEngine mRtcEngine;
    private Text MessageText;

    static LinkVideoToPlayer LinkVideoToPlayerObject;
    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void join(string channel)
    {
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
        mRtcEngine.OnUserJoined = onUserJoined;
        mRtcEngine.OnUserOffline = onUserOffline;
        mRtcEngine.OnWarning = (int warn, string msg) =>
        {
            Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
        };
        mRtcEngine.OnError = HandleError;

        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);
    }

    public string getSdkVersion()
    {
        string ver = IRtcEngine.GetSdkVersion();
        return ver;
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    public void EnableVideo(bool pauseVideo)
    {
        if (mRtcEngine != null)
        {
            if (!pauseVideo)
            {
                mRtcEngine.EnableVideo();
            }
            else
            {
                mRtcEngine.DisableVideo();
            }
        }
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded()
    {
        Debug.Log("Inside TestHelloUnityVideo-> onSceneHelloVideoLoaded,");

        // Attach the SDK Script VideoSurface for video rendering
        // GameObject quad = GameObject.Find("Quad");
        // if (ReferenceEquals(quad, null))
        // {
        //     Debug.Log("failed to find Quad");
        //     return;
        // }
        // else
        // {
        //     quad.AddComponent<VideoSurface>();
        // }

        GameObject cube = GameObject.Find("CubeForMe");
        if (ReferenceEquals(cube, null))
        {
            Debug.Log("failed to find Cube");
            return;
        }
        else
        {
            cube.AddComponent<VideoSurface>();
        }

        // // Swapnil edits
        // GameObject face = GameObject.Find("Face");
        // if (ReferenceEquals(face, null))
        // {
        //     Debug.Log("failed to find a face");
        //     return;
        // }
        // else
        // {
        //     face.AddComponent<VideoSurface>();
        // }

        GameObject text = GameObject.Find("MessageText");
        if (!ReferenceEquals(text, null))
        {
            MessageText = text.GetComponent<Text>();
        }
    }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        // GameObject textVersionGameObject = GameObject.Find("VersionText");
        // textVersionGameObject.GetComponent<Text>().text = "SDK Version : " + getSdkVersion();


        // Add agoraId to Hashtable
        Hashtable hash= new Hashtable();
        hash.Add("agoraID",uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash);
        Debug.Log("TestHelloUnityVideo->onJoinChannelSuccess: PhotonNetwork.SetPlayerCustomProperties, uid:" + uid);


    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        // onUserJoinedOld(uid, elapsed);
        onUserJoinedNew(uid, elapsed);
        // MeetingManager.
    }


    // Author: Swapnil K
    // Purpose: Update according to multiplayer feature
    private void onUserJoinedNew(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        
        // this is called in main thread

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurfaceMultiplayer(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);
            videoSurface.SetGameFps(30);
        }
        // MeetingManager.LinkVideoToPlayer();
        LinkVideoToPlayerObject = new LinkVideoToPlayer();
        LinkVideoToPlayerObject.LinkAgoraAndPhoton();
    }


    // Author: Swapnil K
    // Purpose: Update according to multiplayer feature
    private const float Offset = 100;
    public VideoSurface makeImageSurfaceMultiplayer(string goName)
    {
        Debug.Log("Inside makeImageSurfaceMultiplayer:");
        // GameObject go = new GameObject();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject cubeback = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // if (go == null)
        // {
        //     return null;
        // }

        // go.name = goName;
        cube.name = goName;
        cubeback.transform.parent = cube.transform;
        cubeback.transform.localPosition = new Vector3(0f, 0f,-3f);
        cubeback.transform.localRotation = Quaternion.Euler(0, 0, 0);
        // cube.transform.Rotate(0f, 0.0f, 180.0f);


        // to be renderered onto
        // go.AddComponent<RawImage>();
        cube.AddComponent<RawImage>();

        // make the object draggable
        // go.AddComponent<UIElementDragger>();
        // cube.AddComponent<UIElementDragger>();

        // GameObject canvas = GameObject.Find("Canvas");
        // GameObject face = GameObject.Find("Face");
        // GameObject[] players = GameObject.FindGameObjectsWithTag("Player");


        // if (face != null)
        // {
        //     // go.transform.parent = face.transform;
        //     cube.transform.parent = face.transform;
        // }
      

        // // set up transform
        // // go.transform.Rotate(0f, 0.0f, 180.0f);
        // cube.transform.Rotate(0f, 0.0f, 180.0f);

        // float xPos = 0f;
        // float yPos = 2f;
        // float zPos = 4f;

        // // go.transform.localPosition = new Vector3(xPos, yPos, 0f);
        // cube.transform.localPosition = new Vector3(xPos, yPos, zPos);

        // // go.transform.localScale = new V ector3(2f, 2f, 2f);
        // cube.transform.localScale = new Vector3(2f, 2f, 2f);


        // configure videoSurface
        // VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        VideoSurface videoSurface = cube.AddComponent<VideoSurface>();

        return videoSurface;
    }

    public VideoSurface makePlaneSurface(string goName)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);

        if (go == null)
        {
            return null;
        }
        go.name = goName;
        // set up transform
        go.transform.Rotate(-90.0f, 0.0f, 0.0f);
        float yPos = Random.Range(3.0f, 5.0f);
        float xPos = Random.Range(-2.0f, 2.0f);
        go.transform.position = new Vector3(xPos, yPos, 0f);
        go.transform.localScale = new Vector3(0.25f, 0.5f, 0.1f);

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }

    

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
    }

    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Debug.LogError(msg);
        if (MessageText != null)
        {
            if (MessageText.text.Length > 0)
            {
                msg = "\n" + msg;
            }
            MessageText.text += msg;
        }

        LastError = error;
    }

    #endregion
}
