using UnityEngine;
using System.Collections;

public class moveToxin : MonoBehaviour {
	
	public Vector3 direction;
	public float speed;
	public float reach;
	private float scale;
	private Vector3 startPos;
	

	// Use this for initialization
	void Start () {
		startPos = transform.localPosition;
		// direction = new Vector3(0.0f, 0.0f,0.0f);
		speed = 2.0f;
		reach = 15.0f;
		scale = 0.1f;
		
		transform.localScale = new Vector3(scale, scale, scale); 
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log("direction: " + direction);
		transform.localPosition = transform.localPosition + direction* speed;
		
		Vector3 temp = startPos- transform.localPosition;
		
		if (scale < 1.5f){
			transform.localScale = 	new Vector3(scale, scale, scale); 
			scale = scale + 0.3f;
		}
		if (temp.magnitude > reach){
			Network.Destroy(transform.GetChild(0).gameObject);
			Network.Destroy (this.gameObject);
			//Destroy(transform.parent.GetChild(0));
			//Destroy(transform.parent);
			
		}
	}
	
	void OnTriggerEnter(Collider coll)
	{
		
	   	if(coll.gameObject.tag=="hurdle"){
	    	Destroy(coll.gameObject);   
			Debug.Log("collide with bee");
		}
		
		if(coll.gameObject.tag=="player"){
	    	Debug.Log("collide with bird: TODO ENERGIE LOSS");  
			
		}
	}
	
}
