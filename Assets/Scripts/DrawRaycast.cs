using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DrawRaycast : MonoBehaviour,IPunObservable
{
    LineRenderer lineRenderer;
    private Vector3 pointerDirection;
    PhotonView photonView_;

    bool isWriting;


    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        photonView_ = PhotonView.Get(this);

    }


    void IPunObservable.OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info){
        Debug.Log("DrawRaycast-> OnPhotonSerializeView");

        // if(!photonView_.IsMine){
        //     return;
        // }
        pointerDirection = (transform.forward);

        if(stream.IsWriting){
            Debug.Log("Sending position OnPhotonSerializeView");
            stream.SendNext(pointerDirection);
            isWriting = true;
        }else if(stream.IsReading){
            Debug.Log("Recieving position OnPhotonSerializeView");
            pointerDirection = (Vector3)stream.ReceiveNext();
            isWriting = false;
        }
    }
 

    private void OnEnable()
    {
        Application.onBeforeRender += UpdateRoute;
    }
    
    private void OnDisable()
    {
        Application.onBeforeRender -= UpdateRoute;
    }

    void UpdateRoute(){
            if(isWriting){
                pointerDirection = transform.forward;
            }

            lineRenderer.SetPosition(0,transform.position);

            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            layerMask = ~layerMask;

            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, pointerDirection, out hit, 1000 ))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                lineRenderer.SetPosition(1, hit.point);

            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
                lineRenderer.SetPosition(1, pointerDirection * 1000);
            }
        
        
    }
}
