using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public string id;
    public bool isActive = true;
    public IHitboxOwner parent;
    
    void OnTriggerStay(Collider other)
    {
        if (parent == null) return;
        if (!isActive) return;

        parent.HitboxStay(other);
    }
}
