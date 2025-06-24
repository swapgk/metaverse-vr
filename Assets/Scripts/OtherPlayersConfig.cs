using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;


public class OtherPlayersConfig : MonoBehaviourPunCallbacks
{
    private static PhotonView PV;

    public static Dictionary<Player, GameObject> gameObjectFromOtherPlayers = new Dictionary<Player, GameObject>();

    private WaitForSeconds delay;
    // private MonoBehaviourPunCallbacks instance;

    
    void Awake() {
        OtherPlayersConfig.PV = GetComponent<PhotonView>();
        gameObjectFromOtherPlayers[OtherPlayersConfig.PV.Owner] = this.gameObject;
        Debug.Log("OtherPlayersConfig->Awake: OtherPlayersConfig.PV");

        // instance = this;
    } 

    void Start() {
        delay = new WaitForSeconds(0.1f);
    }

    private void OnDestroy() {
        gameObjectFromOtherPlayers.Remove(OtherPlayersConfig.PV.Owner);    
    }


    // public void LinkAgoraAndPhoton()
    // {
    //     GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

    //     Debug.Log("OtherPlayersConfig->LinkAgoraAndPhoton, my OtherPlayersConfig.PV.IsMine : " + OtherPlayersConfig.PV.ViewID);

    //     // Find my PhotonView and then link other players' Video gameobject to thier Player GameObject
    //     foreach (GameObject p in Players)
    //     {
    //         PhotonView pv = p.GetComponent<PhotonView>();
    //         if (!pv.IsMine)
    //         {
    //             continue;
    //         }
    //         Debug.Log("OtherPlayersConfig->LinkAgoraAndPhoton, DoLinking()");
    //         instance.StartCoroutine( "DoLinking");
    //         // DoLinking();
    //     }

    // }


    // IEnumerator DoLinking()
    // {
        
    //     foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
    //     {
    //         Debug.Log("OtherPlayersConfig->DoLinking, Found a player in");

    //         if (player.IsLocal)
    //         {
    //             Debug.Log("OtherPlayersConfig->DoLinking, player.IsLocal");
    //             continue;
    //         }

    //         // wait untill "agoraID" is available in CustomProperties for that player
    //         yield return new WaitUntil(()=> IsOkayToStart(player));
          
            
    //         if (player.CustomProperties.TryGetValue("agoraID", out object agoraID))
    //         {

    //             GameObject otherPlayer = gameObjectFromOtherPlayers[player];
    //             GameObject agoraVideoObject = GameObject.Find((string)agoraID);
    //             if (ReferenceEquals(agoraVideoObject, null))
    //             {
    //                 Debug.Log("OtherPlayersConfig->DoLinking failed to find agoraVideoObject");
    //                 yield return null;
    //             }
    //             else
    //             {
    //                 Debug.Log("OtherPlayersConfig->DoLinking, Found agoraVideoObject: " + agoraID.ToString());
    //                 agoraVideoObject.transform.parent = otherPlayer.transform;

    //                 float xPos = 0f;
    //                 float yPos = 3.8f;
    //                 float zPos = 0f;

    //                 agoraVideoObject.transform.localPosition = new Vector3(xPos, yPos, zPos);
    //                 agoraVideoObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
    //                 agoraVideoObject.transform.localScale = new Vector3(2.427728f, 2.427728f, 0.1f);

    //             }

    //         }
    //         else
    //         {
    //             Debug.Log("OtherPlayersConfig->DoLinking, Not found: CustomProperties.TryGetValue(agoraID)");
    //         }

    //     }

         
    // }
    // bool IsOkayToStart(Player player){
    //     if( player.CustomProperties.TryGetValue("agoraID", out object agoraIDTest)){
    //         Debug.Log("OtherPlayersConfig->DoLinking->ProcessDoLinking Waiting for player AgoraID");
    //         return true;
    //     }
    //     return false;
    // }
     

}
