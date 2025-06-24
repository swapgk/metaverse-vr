using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class LaunchManager : MonoBehaviourPunCallbacks
{
    public GameObject JoinMeetingPanel;
    public GameObject ConnectionStatusPanel;
    public GameObject LobbyPanel;
    
    public static string roomName;

    private string HomeSceneName = "SceneHome";

    private string PlaySceneName = "MeetingScene";



    // STARTOF: Agora
    // Use this for initialization
    [SerializeField]
    private string AppID = "your_appid";
    #if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        private ArrayList permissionList = new ArrayList();
    #endif
        static TestHelloUnityVideo app = null;
    // ENDOF: Agora

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
		permissionList.Add(Permission.Microphone);         
		permissionList.Add(Permission.Camera);               
#endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        JoinMeetingPanel.SetActive(true);
        ConnectionStatusPanel.SetActive(false);
        LobbyPanel.SetActive(false);

    }

    void Update()
    {
        // Debug.Log("PlayerNames: ");
        // foreach (Player player in PhotonNetwork.PlayerList) 
        // {
        //     Debug.Log(player.NickName);
        // }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log(PhotonNetwork.NickName+ ": Connect to Photon Master!");
        LobbyPanel.SetActive(true);

    }

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet!");
    }

    public void ConnectToPhotonServer(){
        if(!PhotonNetwork.IsConnected){
            PhotonNetwork.ConnectUsingSettings();
            JoinMeetingPanel.SetActive(false);
            ConnectionStatusPanel.SetActive(true);
        }
    }


    public void JoinOrCreateRoom()
    {
        Debug.Log("JoinOrCreateRoom ...");
        roomName = RoomNameInputManager.roomName;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible  = true;
        roomOptions.MaxPlayers = 20;
        roomOptions.PublishUserId = true;

        PhotonNetwork.JoinOrCreateRoom(roomName,roomOptions,null);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("LaunchManager-> " + PhotonNetwork.NickName + ",Joined Room:"+ PhotonNetwork.CurrentRoom.Name);
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MeetingScene");
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("LaunchManager->OnPlayerEnteredRoom: " + newPlayer.NickName + " joined to "
            + PhotonNetwork.CurrentRoom.Name + ", PlayerCount= "
            + PhotonNetwork.CurrentRoom.PlayerCount);
    }

}
