using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;

/// <summary>
///    TestHome serves a game controller object for this application.
/// </summary>
public class TestHome : MonoBehaviour
{

    // Use this for initialization
    #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        private ArrayList permissionList = new ArrayList();
    #endif
    static TestHelloUnityVideo app = null;

    private string HomeSceneName = "LaunchScene";
    public string channelName;

    private string PlaySceneName = "MeetingScene";

    // PLEASE KEEP THIS App ID IN SAFE PLACE
    // Get your own App ID at https://dashboard.agora.io/
    [SerializeField]
    private string AppID = "your_appid";

    void Awake()
    {
        #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
                permissionList.Add(Permission.Microphone);         
                permissionList.Add(Permission.Camera);               
        #endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        CheckAppId();
        channelName = LaunchManager.roomName;
        Debug.Log("Inside TestHome-> Start, channelName: "+ channelName);
    }

    void Update()
    {
        CheckPermissions();
    }

    private void CheckAppId()
    {
        Debug.Assert(AppID.Length > 10, "Please fill in your AppId first on Game Controller object.");
        GameObject go = GameObject.Find("AppIDText");
        if (go != null)
        {
            Text appIDText = go.GetComponent<Text>();
            if (appIDText != null)
            {
                if (string.IsNullOrEmpty(AppID))
                {
                    appIDText.text = "AppID: " + "UNDEFINED!";
                }
                else
                {
                    appIDText.text = "AppID: " + AppID.Substring(0, 4) + "********" + AppID.Substring(AppID.Length - 4, 4);
                }
            }
        }
    }

    /// <summary>
    ///   Checks for platform dependent permissions.
    /// </summary>
    private void CheckPermissions()
    {
        #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
                foreach(string permission in permissionList)
                {
                    if (!Permission.HasUserAuthorizedPermission(permission))
                    {                 
                        Permission.RequestUserPermission(permission);
                    }
                }
        #endif
    }
    
    //  // called first
    // void OnEnable()
    // {
    //     Debug.Log("Inside TestHome->OnEnable called");
    //     SceneManager.sceneLoaded += OnLevelFinishedLoading;
    // }

    public void onJoinButtonClicked()
    {
        Debug.Log("Inside TestHome-> onJoinButtonClicked");

        // create app if nonexistent
        if (ReferenceEquals(app, null))
        {
            app = new TestHelloUnityVideo(); // create app
            app.loadEngine(AppID); // load engine
        }

        // join channel
        channelName = LaunchManager.roomName;
        app.join(channelName);
        // SceneManager.sceneLoaded += OnLevelFinishedLoading; // configure GameObject after scene is loaded
        // SceneManager.LoadScene(PlaySceneName, LoadSceneMode.Single);
    }

    public void onLeaveButtonClicked()
    {
        if (!ReferenceEquals(app, null))
        {
            app.leave(); // leave channel
            app.unloadEngine(); // delete engine
            app = null; // delete app
            SceneManager.LoadScene(HomeSceneName, LoadSceneMode.Single);
        }
        Destroy(gameObject); 
    }

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {   
        Debug.Log("Inside TestHome-> OnLevelFinishedLoading, scene name: "+ scene.name);
        if (scene.name == PlaySceneName)
        {           
            Debug.Log("Inside TestHome-> OnLevelFinishedLoading, Scene Name is correct");

            if (!ReferenceEquals(app, null))
            {
                Debug.Log("Inside TestHome-> OnLevelFinishedLoading calling onSceneHelloVideoLoaded()");
                app.onSceneHelloVideoLoaded(); // call this after scene is loaded
            }
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (!ReferenceEquals(app, null))
        {
            app.EnableVideo(paused);
        }
    }

    void OnApplicationQuit()
    {
        if (!ReferenceEquals(app, null))
        {
            app.unloadEngine();
        }
    }
}
