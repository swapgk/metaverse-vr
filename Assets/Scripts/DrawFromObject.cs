using UnityEngine;
using UnityEngine.Rendering;
using Photon.Pun;


/* 
The object to which this file is attached will act as a drawing tool
 */
public class DrawFromObject : MonoBehaviour,IPunObservable
{
    private LineRenderer Line;
    private Vector3 pointerPos;
    public Material material;

    private int currLines = 0;
    public float minimumVertexDistance = 0.1f;
    private bool isLineStarted;
    
    public bool activeSelf;
 
    PhotonView photonView_;

    void Start() {
        photonView_ = PhotonView.Get(this);

    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        Debug.Log("DrawFromObject-> OnPhotonSerializeView");

        // if(!photonView_.IsMine){
        //     return;
        // }
        pointerPos = (transform.position);

        if(stream.IsWriting){
            Debug.Log("Sending position OnPhotonSerializeView");
            stream.SendNext(pointerPos);
        }else if(stream.IsReading){
            pointerPos = (Vector3)stream.ReceiveNext();
        }
    }
 
    void Update()
    {   
        if(!photonView_.IsMine){
            return;
        }
        // pointerPos = (transform.position);
        // photonView_.RPC("setPointerPosition", RpcTarget.AllBufferedViaServer, pointerPos);
        // // PhotonNetwork.SendAllOutgoingCommands();


        if (Input.GetMouseButtonDown(0))
        {
            photonView_.RPC("PrecessLeftMouseClick", RpcTarget.AllBuffered, pointerPos);

            // PrecessLeftMouseClick();
        }
        else if (Input.GetMouseButtonUp(0) && Line)   // stop drawing line
        {
            photonView_.RPC("NewMethod", RpcTarget.AllBuffered);
            // NewMethod();
        }
        else if (Input.GetMouseButton(0) && Line)
        {
            // WhileLeftMousePressing();
            photonView_.RPC("WhileLeftMousePressing", RpcTarget.AllBuffered, pointerPos);

        }
    }

    [PunRPC]
    private void setPointerPosition(Vector3 pointerPos_){
        pointerPos = pointerPos_;
    }



    [PunRPC]
    private void NewMethod()
    {
        Line = null;
        currLines++;
        isLineStarted = false;
    }


    [PunRPC]
    public void PrecessLeftMouseClick(Vector3 pointerPos_)
    {
        if (Line == null)
        {
            createLine();
        }
        pointerPos = pointerPos_;
        // Debug.Log("pointerPos after" + pointerPos);
        Line.positionCount = 2;
        Line.SetPosition(0, pointerPos);
        Line.SetPosition(1, pointerPos);
        isLineStarted = true;
    }


    void createLine()
    {
        Line = new GameObject("Line" + currLines).AddComponent<LineRenderer>();
        Line.material = material;
        Line.positionCount = 2;
        Line.startWidth = 0.15f;
        Line.endWidth = 0.15f;
        Line.useWorldSpace = false;
        Line.numCapVertices = 50;

        isLineStarted = false;

    }

    [PunRPC]
    private void WhileLeftMousePressing(Vector3 pointerPos_)
    {
        Vector3 currentPos = pointerPos_;
        float distance = Vector3.Distance(currentPos, Line.GetPosition(Line.positionCount - 1));
        if (distance > minimumVertexDistance)
        {
            UpdateLine(currentPos);
        }
    }
    private void UpdateLine(Vector3 pointerPos_)
    {
        Line.positionCount++;
        pointerPos =  pointerPos_;

        Line.SetPosition(Line.positionCount - 1, pointerPos);
        // Debug.Log("pointerPos before" + pointerPos);

    }

    private Vector3 GetWorldCoordinate(Vector3 mousePositionV)
    {
        Vector3 pointerPos = new Vector3(mousePositionV.x, mousePositionV.y, mousePositionV.z);
        return Camera.main.ScreenToWorldPoint(pointerPos);
    }
    
}