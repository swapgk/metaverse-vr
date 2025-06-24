using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    bool collisionDisabled = false;

    void Start() {

    }

    void Update() {
        RespondToDebugKeys();

    }
    void RespondToDebugKeys(){
    //   if( Input.GetKeyDown(KeyCode.C)){
    //         collisionDisabled = ! collisionDisabled;
    //         Debug.Log("Collision disabled");
    //     }
    }

    void OnCollisionEnter(Collision other) {
        if(collisionDisabled ){ return; }

        switch (other.gameObject.tag)
        {
        case "Player":
            Debug.Log("Collided with Player");
            break;
        default:
            Debug.Log("Collided with Obstacle");
            break;
        }
        
        
    }
    
}
