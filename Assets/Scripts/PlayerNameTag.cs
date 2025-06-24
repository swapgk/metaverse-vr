using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private Text nameText;
    void Start()
    {
        Debug.Log("PlayerNameTag-> photonViewID"+ photonView.ViewID);
        if(photonView.IsMine){ return; }
        setName();

        // MeetingManager.LinkVideoToPlayer();
    }

    // Update is called once per frame
    void setName()
    {           
        Debug.Log("PlayerNameTag->setName, photonViewID"+ photonView.ViewID);
        nameText.text = photonView.Owner.NickName;
    }
}
