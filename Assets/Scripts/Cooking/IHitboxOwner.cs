using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxOwner
{
    void HitboxStay(Collider other);
}
