using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;
using agora_utilities;

// To get agora ID from photon view
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerViewControllerBase : IVideoChatClient
{
    public event Action OnViewControllerFinish;
    protected IRtcEngine mRtcEngine;
    protected Dictionary<uint, VideoSurface> UserVideoDict = new Dictionary<uint, VideoSurface>();
    protected const string SelfVideoName = "MyView";
    protected string mChannel;

    // public Material myMaterial;

    // private void Start() {
    //     myMaterial = MainScreenScript.myMaterial;
    // }

    //    string logFilepath =
    //#if UNITY_EDITOR
    //    Application.dataPath + "/testagora.log";
    //#else
    //    Application.persistentDataPath + "/tesagora.log";
    //#endif
    protected bool remoteUserJoined = false;
    protected bool _enforcing360p = false; // the local view of the remote user resolution

    static LinkVideoToPlayer LinkVideoToPlayerObject;

    public PlayerViewControllerBase()
    {
        // Constructor, nothing to do for base
    }

    /// <summary>
    ///   Join a RTC channel
    /// </summary>
    /// <param name="channel"></param>
    public void Join(string channel)
    {
        Debug.Log("PlayerViewControllerBase-> Join");
        Debug.Log("calling join (channel = " + channel + ")");

        if (mRtcEngine == null)
            return;

        mChannel = channel;

        // set callbacks (optional)
        mRtcEngine.OnJoinChannelSuccess = OnJoinChannelSuccess;
        mRtcEngine.OnUserJoined = OnUserJoined;
        mRtcEngine.OnUserOffline = OnUserOffline;
        mRtcEngine.OnVideoSizeChanged = OnVideoSizeChanged;
        // Calling virtual setup function
        PrepareToJoin();

        // join channel
        mRtcEngine.JoinChannel(channel, null, 0);

        Debug.Log("initializeEngine done");
    }

    /// <summary>
    ///    Preparing video/audio/channel related characteric set up
    /// </summary>
    protected virtual void PrepareToJoin()
    {
        // enable video
        mRtcEngine.EnableVideo();
        // allow camera output callback
        mRtcEngine.EnableVideoObserver();
    }

    /// <summary>
    ///   Leave a RTC channel
    /// </summary>
    public virtual void Leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    protected bool MicMuted { get; set; }

    protected virtual void SetupUI()
    {
        Debug.Log("PlayerViewControllerBase-> SetupUI");

        GameObject go = GameObject.Find(SelfVideoName);
        if (go != null)
        {
            UserVideoDict[0] = go.AddComponent<VideoSurface>();
            go.AddComponent<UIElementDragger>();
        }

        Button button = GameObject.Find("LeaveButton").GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnLeaveButtonClicked);
        }

        Button mutton = GameObject.Find("MuteButton").GetComponent<Button>();
        if (mutton != null)
        {
            mutton.onClick.AddListener(() =>
            {
                MicMuted = !MicMuted;
                mRtcEngine.EnableLocalAudio(!MicMuted);
                mRtcEngine.MuteLocalAudioStream(MicMuted);
                Text text = mutton.GetComponentInChildren<Text>();
                text.text = MicMuted ? "Unmute" : "Mute";
            });
        }

        go = GameObject.Find("ToggleScale");
        if (go != null)
        {
            Toggle toggle = go.GetComponent<Toggle>();
            _enforcing360p = toggle.isOn; // initial value
            toggle.onValueChanged.AddListener((val) =>
            {
                _enforcing360p = val;
            });
        }
    }

    protected void OnLeaveButtonClicked()
    {
        Leave();
        UnloadEngine();

        if (OnViewControllerFinish != null)
        {
            OnViewControllerFinish();
        }
    }

    protected virtual void OnVideoSizeChanged(uint uid, int width, int height, int rotation)
    {
        Debug.LogWarningFormat("uid:{3} OnVideoSizeChanged width = {0} height = {1} for rotation:{2}", width, height, rotation, uid);
         
        if (UserVideoDict.ContainsKey(uid))
        {
            GameObject go = UserVideoDict[uid].gameObject;
            Vector2 v2 = new Vector2(width, height);
            RawImage image = go.GetComponent<RawImage>();
            if (_enforcing360p)
            {
                v2 = AgoraUIUtils.GetScaledDimension(width, height, 240f);
            }

            if (IsPortraitOrientation(rotation))
            {
                v2 = new Vector2(v2.y, v2.x);
            }
            image.rectTransform.sizeDelta = v2;
        }
    }

    bool IsPortraitOrientation(int rotation)
    {
        return rotation == 90 || rotation == 270;
    }

    /// <summary>
    ///   Load the Agora RTC engine with given AppID
    /// </summary>
    /// <param name="appId">Get the APP ID from Agora account</param>
    public void LoadEngine(string appId)
    {
        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        mRtcEngine.OnError = (code, msg) =>
        {
            Debug.LogErrorFormat("RTC Error:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
        };

        mRtcEngine.OnWarning = (code, msg) =>
        {
            Debug.LogWarningFormat("RTC Warning:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
        };

        // mRtcEngine.SetLogFile(logFilepath);
        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    // unload agora engine
    public virtual void UnloadEngine()
    {
        Debug.Log("calling unloadEngine");

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    /// <summary>
    ///   Enable/Disable video
    /// </summary>
    /// <param name="pauseVideo"></param>
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

    public virtual void OnSceneLoaded()
    {
        Debug.Log("PlayerViewControllerBase-> OnSceneLoaded -> calling SetupUI");

        SetupUI();
    }

    // implement engine callbacks
    protected virtual void OnJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
        MainScreenScript.setAgoraID(uid.ToString());


        // Add agoraId to Hashtable
        Hashtable hash= new Hashtable();
        hash.Add("agoraID",uid.ToString());
        PhotonNetwork.SetPlayerCustomProperties(hash);
        Debug.Log("TestHelloUnityVideo->onJoinChannelSuccess: PhotonNetwork.SetPlayerCustomProperties, uid:" + uid);

        GameObject CubeForMe = GameObject.Find("CubeForMe");
        CubeForMe.AddComponent<RawImage>();
        CubeForMe.AddComponent<VideoSurface>();


    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    protected virtual void OnUserJoined(uint uid, int elapsed)
    {
        // onUserJoinedOld(uid, elapsed);
        onUserJoinedNew(uid, elapsed);

    }

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
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            UserVideoDict[uid] = videoSurface;
        }
        // // MeetingManager.LinkVideoToPlayer();
        LinkVideoToPlayerObject = new LinkVideoToPlayer();
        LinkVideoToPlayerObject.LinkAgoraAndPhoton();
    }


    private void onUserJoinedOld(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);

        // find a game object to render video stream from 'uid'
        GameObject go = GameObject.Find(uid.ToString());
        if (!ReferenceEquals(go, null))
        {
            return; // reuse
        }

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(uid.ToString());
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForUser(uid);
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
            videoSurface.SetGameFps(30);
            videoSurface.EnableFilpTextureApply(enableFlipHorizontal: true, enableFlipVertical: false);
            UserVideoDict[uid] = videoSurface;
            Vector2 pos = AgoraUIUtils.GetRandomPosition(100);
            videoSurface.transform.localPosition = new Vector3(pos.x, pos.y, 0);

        }
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
        // cube.GetComponent<MeshRenderer>().material = myMaterial;

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

    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    protected virtual void OnUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        if (UserVideoDict.ContainsKey(uid))
        {
            var surface = UserVideoDict[uid];
            surface.SetEnable(false);
            UserVideoDict.Remove(uid);
            GameObject.Destroy(surface.gameObject);
        }
    }

    protected VideoSurface makeImageSurface(string goName)
    {
        GameObject go = new GameObject();

        if (go == null)
        {
            return null;
        }

        go.name = goName;

        // to be renderered onto
        RawImage image = go.AddComponent<RawImage>();
        image.rectTransform.sizeDelta = new Vector2(1, 1);// make it almost invisible

        // make the object draggable
        go.AddComponent<UIElementDragger>();
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            go.transform.SetParent(canvas.transform);
        }
        // set up transform
        go.transform.Rotate(0f, 0.0f, 180.0f);
        Vector2 v2 = AgoraUIUtils.GetRandomPosition(200);
        go.transform.position = new Vector3(v2.x, v2.y, 0);
        go.transform.localScale = Vector3.one;

        // configure videoSurface
        VideoSurface videoSurface = go.AddComponent<VideoSurface>();
        return videoSurface;
    }
}
