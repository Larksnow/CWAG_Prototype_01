using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurgerIngredient : MonoBehaviour
{
    new public string name;
    public bool active = true;

    public float height;

    [SerializeField] new MeshCollider collider;

    public void DisableCollider()
    {
        collider.gameObject.layer = 7;
    }

    public void EnableColldier()
    {
        collider.gameObject.layer = 6;
    }
}
