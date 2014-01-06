using UnityEngine;
using System.Collections;

public class followCharacter : MonoBehaviour {

	private GameObject bird;
	GameObject tracker = null;
	public Vector3 startOffset = new Vector3(0.0f, 1.5f, 0.0f);
	private Vector3 movement;
	public float maxDistance = 10.0f;
	private GameObject spray;
	private GameObject straw;
	
	
	public GameObject tox;
  
		
	void Start () {
		movement = new Vector3();
		spray = GameObject.Find ("spraycan");
		straw = GameObject.Find("straw");
		spray.SetActive(true);
		straw.SetActive(false);
	}
	
	
	
	//TODO: RPCs here!!!!!!!!!!!!
	[RPC]
	virtual public void moveVirtualHand(Vector3 position){

		if(Vector3.Distance(position, Vector3.zero) > maxDistance){ 
				movement = position.normalized* maxDistance;
		}else{
			movement = position;
		}
		
	}
	
	[RPC]
	virtual public void rotateVirtualHand(Quaternion orientation){
		transform.localRotation = orientation;
	}
	
	[RPC]
	virtual public void changeObject(){
		if(spray.activeSelf){
			spray.SetActive(false);
			straw.SetActive(true);
		}else{
			spray.SetActive(true);
			straw.SetActive(false);
		}
	}
	
	[RPC]
	virtual public void shoot(){
		if(spray.activeSelf){
			GameObject toxi = (GameObject)Network.Instantiate(tox, transform.localPosition, Quaternion.identity,0);
			toxi.GetComponent<moveToxin>().direction = transform.forward;
		}
	}
		
	
	
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (Network.isServer)
    	{
			if(bird == null){
			bird = GameObject.Find("BlockparBird(Clone)");
			}
			if(bird != null){
				
				//TODO: bei richtungs änderung velocity interpolieren??
				
				rigidbody.velocity = bird.rigidbody.velocity;
				rigidbody.position = bird.rigidbody.position + startOffset + movement;
				
			}
		}
		else{
			if (tracker == null){
				tracker = GameObject.Find("Spacemouse");
			}
			if (tracker != null){
						
				networkView.RPC("moveVirtualHand", RPCMode.Server, tracker.transform.localPosition);
				networkView.RPC("rotateVirtualHand", RPCMode.Server, tracker.transform.localRotation);
			}
			if(Input.GetButtonUp("change")){
				Debug.Log("change");
				if(spray.activeSelf){
					spray.SetActive(false);
					straw.SetActive(true);
				}else{
					spray.SetActive(true);
					straw.SetActive(false);
				}
				networkView.RPC("changeObject",RPCMode.Server);
			}
			if(Input.GetButton("shoot")){
				Debug.Log("shoot");
				networkView.RPC("shoot",RPCMode.Server);
			}
			
		}
		
    }

}
