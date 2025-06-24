using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class CollisionHandlerChair : MonoBehaviourPunCallbacks
{
    
    bool collisionDisabled = false;
    bool isPlayerInRange = false;
    bool isPlayerSeated = false;
    bool isOccupied = false;
    bool isTransitioning = false; // whether the sitting anamation is playing

    float inRangeDist = 2.0f;

    private Vector3 oldPosition;
    Transform PlayerTransform = null; 

    PhotonView photonView_;


    void Start() {
        photonView_ = PhotonView.Get(this);
    }

    void Update() {
        RespondToDebugKeys();
    }

    [PunRPC]
    void changeChairOccupancyStatus(bool isChairOccupied){
        isOccupied = isChairOccupied;
        Debug.Log("Chair isOccupied: "+  isOccupied);
    }

    void RespondToDebugKeys(){
        // if( Input.GetKeyDown(KeyCode.C)){
        //     collisionDisabled = ! collisionDisabled;
        //     Debug.Log("Collision disabled");
        // }

        if( Input.GetKeyDown(KeyCode.F) ){
            if (isPlayerInRange && isPlayerSeated == false)
            {
                Debug.Log("Requested to sit on chair");
                isTransitioning = true;
                sitOnChair();
            }
            else if( isPlayerInRange == true && isPlayerSeated == true ){ //already seated
                Debug.Log("Requested to leave the chair");
                isTransitioning = true;
                sitOnChair();
            }
            
        }
        
    }

    void OnCollisionEnter(Collision other) {
        if(collisionDisabled || isTransitioning  ){ return; }

        switch (other.gameObject.tag)
        {
            case "Player":
                Debug.Log("Collided with Player");
                isPlayerInRange = true;
                PlayerTransform = other.gameObject.transform;
                break;
            default:
                Debug.Log("Collided with Obstacle");
                break;
        }
        
    }

    private void OnCollisionExit(Collision other) {
        if(collisionDisabled || isTransitioning ){ return; }

        switch (other.gameObject.tag)
        {
            case "Player":
                if( (transform.position.x == PlayerTransform.position.x ) 
                    && (transform.position.z == PlayerTransform.position.z )  ){
                    Debug.Log("Player still in range and ontop");
                }
                else {
                    Debug.Log("Player moved away");
                    isPlayerInRange = false;
                    PlayerTransform = null;
                }
                break;
            default:
                Debug.Log("Obstacle moved away");
                break;
        }
        
    }

    private void sitOnChair(){
        if(PlayerTransform == null){
            Debug.Log("sitOnChair: PlayerTransform is null");
            return;
        }
        if( isPlayerSeated == true && isOccupied == true){    // make my Player free to move

            PlayerTransform.position = oldPosition;
            isPlayerSeated = false;
            Rigidbody rigidbody = PlayerTransform.gameObject.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Debug.Log("sitOnChair: Player free to move ");
            
             // RPC call
            photonView_.RPC("changeChairOccupancyStatus", RpcTarget.AllBufferedViaServer, false);

        }
        else if( isPlayerSeated == false && isOccupied == true){  // Chair ccupied by someone else
            if(PlayerTransform == null){
                Debug.Log("The chair was Occupied by other player");

            }
            else {
                Debug.Log("The chair is Occupied by other player");
            }
        }
        else if( isPlayerSeated == false && isOccupied == false){  // make Player fixed to chair
            oldPosition = PlayerTransform.position;
            PlayerTransform.position = gameObject.transform.position + new Vector3(0,6,0);
            isPlayerSeated = true;
            Rigidbody rigidbody = PlayerTransform.gameObject.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            Debug.Log("sitOnChair: Player fixed to chair "); 

            // RPC call
            photonView_.RPC("changeChairOccupancyStatus", RpcTarget.AllBufferedViaServer, true);

        }
        isTransitioning = false;
        
    }

    public void cleanChairState() {
        Debug.Log("cleanChairStats: The chair was Occupied by Me!, now I left the game");
        if( isPlayerSeated == true && isOccupied == true){  // Chair was occupied by me , now I left the game
            Debug.Log("The chair was Occupied by Me!, now I left the game");
            photonView_.RPC("changeChairOccupancyStatus", RpcTarget.AllBufferedViaServer, false);
    
        }
    }
    private void OnApplicationQuit() {
        // Debug.Log("OnApplicationQuit: ");
        Debug.Log("OnApplicationQuit: checking chair status");
        if( isPlayerSeated == true && isOccupied == true){  // Chair was occupied by me , now I left the game
            Debug.Log("OnApplicationQuit: The chair was Occupied by Me!, now I left the game");
            if(photonView_ == null){
                Debug.Log("photonView_ is null");
            }
            else{
                Debug.Log("photonView_ is not null");
            }
            photonView_.RPC("changeChairOccupancyStatus", RpcTarget.AllBufferedViaServer, false);
            PhotonNetwork.SendAllOutgoingCommands();
            Debug.Log("changeChairOccupancyStatus call");

        }

    }


}
