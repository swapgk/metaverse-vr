using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomNameInputManager : MonoBehaviour
{
    public Button SubmitButton;
    public static string roomName;

    public void SetRoomName(string _roomName){
        if(string.IsNullOrEmpty( _roomName)){
            SubmitButton.interactable = false;
            Debug.Log("Room name is empty");
            return;
        }
        else{
            SubmitButton.interactable = true;
            roomName = _roomName;
        }

    }
}
