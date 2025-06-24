using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCharacterCollision : MonoBehaviour
{
    public CapsuleCollider characterCollider;
    public CapsuleCollider blockCharacterCollider;

    void Start()
    {
        Physics.IgnoreCollision(characterCollider, blockCharacterCollider, true);
    }

  
}
