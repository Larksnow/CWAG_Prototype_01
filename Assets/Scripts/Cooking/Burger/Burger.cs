using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Burger : MonoBehaviour, IHitboxOwner
{
    [SerializeField] Hitbox hitbox;

    public List<BurgerIngredient> ingredients;
    [SerializeField] float baseHeight;
    float totalHeight;


    // Start is called before the first frame update
    void Start()
    {
        totalHeight = baseHeight;
        hitbox.parent = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitboxStay(Collider other)
    {
        Rigidbody otherRb = other.attachedRigidbody;
        if (otherRb == null) return;
        if (otherRb.tag != "Ingredient") return;

        BurgerIngredient ingredient = other.attachedRigidbody?.GetComponent<BurgerIngredient>();

        if (ingredient == null) return;
        if (!ingredient.active) return;

        AddIngredient(ingredient);
    }

    void AddIngredient(BurgerIngredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.active = false;
        ingredient.GetComponent<Rigidbody>().isKinematic = true;

        ingredient.transform.parent = transform;
        ingredient.transform.localRotation = Quaternion.identity;
        ingredient.transform.localPosition = new Vector3(0, totalHeight, 0);

        RecalculateHitboxHeight();
    }

    void RecalculateHitboxHeight()
    {
        totalHeight = baseHeight;
        foreach (BurgerIngredient ingredient in ingredients)
            totalHeight += ingredient.height;

        hitbox.transform.localPosition = new Vector3(0, totalHeight, 0);
    }
}
