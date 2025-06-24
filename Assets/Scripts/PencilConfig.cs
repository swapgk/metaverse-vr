using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using UnityStandardAssets.Characters.FirstPerson;

#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif

public class PencilConfig : MonoBehaviourPunCallbacks
{
    private PhotonView PV;
    private DrawFromObject drawFromObject;
   
        
    private void Start()
    {  
        PV = GetComponent<PhotonView>();
        drawFromObject = GetComponent<DrawFromObject>();

        // if (!PV.IsMine)
        // {
        //     drawFromObject.enabled = false;

        // }
        
    }

    
}
