using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class PickupAbleObject : MonoBehaviour
{
	[SerializeField]
	Material selectedMaterial;
	[SerializeField]
	Material cannotPlaceMaterial;
	Vector3 positionBeforePickup;
	List<Material[]> oldMaterialList = new List<Material[]>();
	bool followingMouse = false;
	int placeAreaBlocked = 0;
	bool canCurrentlyBePlaced = false;
	[SerializeField]
	Collider objectCollider;
	[SerializeField]
	Collider triggerCollider;
	Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
		rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(followingMouse){
			RaycastHit hit = MousePickup.RaycastToMousePosition(50);
			if(hit.collider != null){
				if(!CheckCanPlace() && canCurrentlyBePlaced){
					MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
					for(int i = 0; i < meshRenderers.Length; i++){
						meshRenderers[i].materials = new Material[]{cannotPlaceMaterial};
					}
					canCurrentlyBePlaced = false;
				}
				else if(CheckCanPlace() && !canCurrentlyBePlaced){
					MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
					for(int i = 0; i < meshRenderers.Length; i++){
						meshRenderers[i].materials = new Material[]{selectedMaterial};
					}
					canCurrentlyBePlaced = true;
				}
				transform.position = hit.point + hit.normal * 0.6f;
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
			if(placeAreaBlocked != 0){
				return false;
			}
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

	public void PlaceAtPreviousLocation(){
		transform.position = positionBeforePickup;
		MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
		for(int i = 0; i < meshRenderers.Length; i++){
			meshRenderers[i].materials = oldMaterialList[i];
		}
		oldMaterialList.Clear();
		objectCollider.enabled = true;
		rigidBody.useGravity = true;
		rigidBody.freezeRotation = false;
		followingMouse = false;
	}

	private bool CheckCanPlace(){

		RaycastHit hit = MousePickup.RaycastToMousePosition(50);
		Debug.Log("Tried to place");
		if(hit.collider != null){
			if(placeAreaBlocked != 0){
				return false;
			}
			return true;
		}
		return false;
	}

	private void OnTriggerEnter(Collider other){
		Debug.Log("Other was Triggered");
		placeAreaBlocked++;	
	}
	
	private void OnTriggerExit(Collider other){
		Debug.Log("Left trigger");
		placeAreaBlocked--;
	}
}
