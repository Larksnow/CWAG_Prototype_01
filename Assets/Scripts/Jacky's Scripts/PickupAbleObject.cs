using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PickupAbleObject : MonoBehaviour
{
	[SerializeField]
	Material selectedMaterial;
	Vector3 positionBeforePickup;
	List<Material[]> oldMaterialList = new List<Material[]>();
	bool followingMouse = false;
	Collider objectCollider;
	Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
		rigidBody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(followingMouse){
			RaycastHit hit = MousePickup.RaycastToMousePosition(50);
			if(hit.collider != null){
				transform.position = hit.point + hit.normal * 0.5f;
			}
		}
    }

	public void Pickup(){
		positionBeforePickup = transform.position;

		// Make the item red so the player knows the item was picked up
		MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
		for(int i = 0; i < meshRenderers.Length; i++){
			oldMaterialList.Add(meshRenderers[i].materials);
			meshRenderers[i].materials = new Material[]{ selectedMaterial };
		}
		objectCollider.enabled = false;
		rigidBody.velocity = Vector3.zero;
		rigidBody.freezeRotation = true;
		rigidBody.useGravity = false;
		followingMouse = true;
	}

	public bool Place(){
		RaycastHit hit = MousePickup.RaycastToMousePosition(50);
		Debug.Log("Tried to place");
		if(hit.collider != null){
			MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
			for(int i = 0; i < meshRenderers.Length; i++){
				meshRenderers[i].materials = oldMaterialList[i];
			}
			oldMaterialList.Clear();
			objectCollider.enabled = true;
			rigidBody.useGravity = true;
			rigidBody.freezeRotation = false;
			followingMouse = false;
			return true;
		}
		return false;
	}
}
