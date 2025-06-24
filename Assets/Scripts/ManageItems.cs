using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ManageItems : MonoBehaviour
{
    public GameObject Pencil;
    public GameObject RayCast;

    public bool pencilstate = true;
    public bool raycaststate = true;

    public bool activeSelf;
    PhotonView photonView_;


    void Start()
    {
        pencilstate  = activeSelf;
        photonView_ = PhotonView.Get(this);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            TogglePencil();
            Debug.Log("P pressed");
            Debug.Log(pencilstate);
        }
        if(Input.GetKeyDown(KeyCode.R)){
            ToggleRayCast();
            Debug.Log("R pressed");
            Debug.Log(raycaststate);
        }
    }

    void TogglePencil(){
        Pencil.SetActive(!pencilstate);
        pencilstate = ! pencilstate;
    }

    void ToggleRayCast(){
        RayCast.SetActive(!raycaststate);
        raycaststate = ! raycaststate;
    }
}
