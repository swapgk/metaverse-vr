using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerNameInputManager : MonoBehaviour
{
    public Button SubmitButton;

    public void SetPlayerName(string playerName){
        if(string.IsNullOrEmpty( playerName)){
            SubmitButton.interactable = false;
            Debug.Log("Player name is empty");
            return;
        }
        else{
            SubmitButton.interactable = true;
            PhotonNetwork.NickName = playerName;
        }

    }

}
