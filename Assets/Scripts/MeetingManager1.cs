using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Linq;

public class MeetingManager1 : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;
    static TestHome testhome = null;
    private static GameObject myPlayer = null;

    // public static List<int> playerPhotonViewID = new List<int>();
    // public static  List<uint> playerAgoraUID = new List<uint>();

    // // store (photonID,agoraID) for each player
    // public static Dictionary<int, string> OtherPlayers =  
    //                   new Dictionary<string, string>();  
    void Start()
    {
        Debug.Log("MeetingManager->Start");

        StartVideoCallProcedure();

        StartPhotonPlayerinstantiation();

    }

    private void StartPhotonPlayerinstantiation()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (playerPrefab != null)
            {
                Debug.Log("Instantiating  a player from meeting Manager");
                int randomPosition = Random.Range(-20, 20);
                myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPosition, 5, randomPosition), Quaternion.identity);
                // // keep the main camera on the FPS conrroller(player) disabled
                // myPlayer.transform.Find("MainCamera").gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No player Prefab found");
        }
    }

    private void StartVideoCallProcedure()
    {
        // if (myPlayer != null)
        // {
        //     if (ReferenceEquals(testhome, null))
        //     {
        //         testhome = GetComponent<TestHome>();

        //         Debug.Log("Calling Testhome.onJoinButtonClicked()");
        //         testhome.onJoinButtonClicked();
        //         Debug.Log("Calling Testhome.OnLevelFinishedLoading()");
        //         testhome.OnLevelFinishedLoading(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        //     }

        // }

        if (ReferenceEquals(testhome, null))
        {
            testhome = GetComponent<TestHome>();

            Debug.Log("Calling Testhome.onJoinButtonClicked()");
            testhome.onJoinButtonClicked();
            Debug.Log("Calling Testhome.OnLevelFinishedLoading()");
            testhome.OnLevelFinishedLoading(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("MeetingManager->OnPlayerEnteredRoom: " + newPlayer.NickName  + " joined to "
            + PhotonNetwork.CurrentRoom.Name + ", PlayerCount= "
            + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // public override void OnLeftRoom(){
    //     Debug.Log("OnLeftRoom: The chair was Occupied by Me!, now I left the game");

    // }

    // public override void OnDisconnected(DisconnectCause cause) {
    //     Debug.Log("OnDisconnected: "+ cause);

    // }

}
