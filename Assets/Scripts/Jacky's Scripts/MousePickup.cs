using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePickup : MonoBehaviour
{
	bool isObjectPickedUp= false;
	PickupAbleObject objectPickedUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1) && !isObjectPickedUp){
			PickUpObject();
		}
		else if(Input.GetKeyUp(KeyCode.Mouse1) && isObjectPickedUp){
			if(objectPickedUp.Place()){
				isObjectPickedUp = false;
			}
			else{
				objectPickedUp.PlaceAtPreviousLocation();
				isObjectPickedUp = false;
			}
		}
    }	
	
	public static RaycastHit RaycastToMousePosition(float raycastLength, string inputMask="Default"){
		RaycastHit hit;

		LayerMask mask = LayerMask.GetMask(inputMask);

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawLine(ray.origin, ray.origin + ray.direction * 50);
		Physics.Raycast(ray, out hit, raycastLength, mask);
		
		return hit;
	}

	private void PickUpObject(){
		RaycastHit hit = RaycastToMousePosition(50, "Pickup Objects");
		if(hit.collider != null){
			PickupAbleObject pickedUpObject = hit.transform.gameObject.GetComponent<PickupAbleObject>();
			if(pickedUpObject != null){
				isObjectPickedUp = true;
				objectPickedUp = pickedUpObject;
				pickedUpObject.Pickup();
			}
			else{
				Debug.Log("it wasn't a pickupObject");
			}
		}
		else{
			Debug.Log("hit was null");
		}
	}
}
