using UnityEngine;
using System.Collections;

public class flyTest3 : MonoBehaviour {

	public float energy = 10.0f;
	private float energyLoss = 0.04f;
	private float energyLossTerrain = 0.1f;
	
	public bool showEnergyLoss = false;
	public bool showCheckpointReached = false;
	public bool showObstacletReached = false;
	
	public crashForce cf = null;
	
	public GameObject lastCheckpoint;
	public int checkpointID;
	
	public bool touchGround = true;
	
	public int obstacles = 0;

	void Start () {
		rigidbody.freezeRotation = true;
		lastCheckpoint = GameObject.Find("start");
		checkpointID = ((checkpoint)lastCheckpoint.GetComponent("checkpoint")).id;
	}
	

               
    // Update is called once per frame
       
    void FixedUpdate ()
    {
			speed();
			steer();
    }
	
	
		void OnGUI()
		{
		    GUI.Label(new Rect(10,20,200,20), "Energy: "+energy);
		
			if(showEnergyLoss){
				GUI.Label(new Rect(200,200,200,20), "Energy loss!");
			}
			if(showCheckpointReached){
				GUI.Label(new Rect(200,230,200,20), "Yay, new checkpoint!");
			}
			if(showObstacletReached){
				GUI.Label(new Rect(200,230,200,20), "Yay, new obstacle!");
			}
	
		}

	
	
	void speed(){
		
		//50.0f weglassen???
		float throttle = Mathf.Clamp01(50.0f *Input.GetAxis("thrust"));
		float pitch = Mathf.Clamp01(50.0f *Input.GetAxis("Vertical"));
		
		if(energy<=0.0f){
			throttle=pitch=0.0f;	
		}
		else{
			energy-=Mathf.Clamp01(Input.GetAxis("thrust"))*energyLoss;
			
		}
		
		if(throttle == 0.0f && pitch != 0.0f)
			throttle = -0.3f;
		if(throttle == 0.0f)
			throttle = -0.8f;
		if(pitch == 0.0f && throttle<=0.0f)
			pitch = 0.5f;
		
		
		
		
	
		if(touchGround){
			if(	throttle<=0.0f){
				rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
				if(energy<=0.0f){
					transform.position = lastCheckpoint.transform.position + new Vector3(0.0f,10.0f,0.0f);
					StartCoroutine(waiting(3));
				}
			}
			else{
				touchGround=false;
			}
		}
		else{
			rigidbody.velocity = (transform.forward*100.0f*pitch) + (transform.up*50.0f*throttle);	
			
			if(cf != null){
				Vector3 tmp = rigidbody.velocity;
				//rigidbody.velocity = tmp + transform.up*-cf.force;
				rigidbody.velocity = new Vector3(tmp.x,-cf.force,tmp.z);
			}
		}
		
			
		
	}
	
	IEnumerator waiting(int t){
		yield return new WaitForSeconds(t);
		energy = 100.0f;
	}
	
	//onGround
	void OnCollisionStay(Collision ci) {
		if(ci.collider.gameObject.tag=="terrain" || ci.collider.gameObject.tag=="checkpoint")
				touchGround=true;
		
		if(ci.collider.gameObject.tag=="terrain")
			energy-=energyLossTerrain;
	}
	
	void disableShowEnergyLoss(){
		showEnergyLoss = false;
	}
	
	void disableShowCheckpointReached(){
		showCheckpointReached = false;
	}
	
	void disableShowObstacletReached(){
		showObstacletReached = false;
	}
	
	void disableCrashForce(){
		cf = null;
	}
	
	
	
	void OnTriggerEnter(Collider other)
		{
		   if(other.gameObject.tag=="energy"){
		    Destroy(other.gameObject);   
			energy+=5000.0f;
		}
			else if(other.gameObject.tag=="terrain"){
				energy-=5.0f;
				showEnergyLoss=true;
				Invoke("disableShowEnergyLoss", 2);
				touchGround=true;
		}
			else if(other.gameObject.tag=="checkpoint"){
				touchGround=true;
				checkpoint cp = ((checkpoint)other.gameObject.GetComponent("checkpoint"));
				if(cp.id == (checkpointID+1) && cp.obstacles <= obstacles){
						checkpointID++;
						obstacles = 0;
						showCheckpointReached = true;
						Invoke("disableShowCheckpointReached", 2);
						lastCheckpoint = other.gameObject;
						energy+= ((checkpoint)other.gameObject.GetComponent("checkpoint")).energy;
			}
		}
			else if(other.gameObject.tag=="obstacle"){
					obstacles++;
					Destroy(other.gameObject.GetComponent("BoxCollider"));
			
					showObstacletReached = true;
					Invoke("disableShowObstacletReached", 2);
		}
			else if(other.gameObject.tag=="hurdle"){
					cf = (crashForce)other.gameObject.GetComponent("crashForce");
					Invoke("disableCrashForce", 1);
		}
		
	}
	
	private float lage = 0.0f;
	
	void steer()
    {
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float yaw = 0;
		
        roll = Input.GetAxis("Horizontal") * (Time.fixedDeltaTime * 50.0f);
		yaw = -Mathf.Clamp(roll,-0.75f,0.75f); //neigung nach links bzw rechts
		yaw = 0.0f;
        AddRot.eulerAngles = new Vector3(0.0f, roll, yaw);

        rigidbody.rotation *= AddRot;
		
		//model nach links bzw rechts neigen bei richtungsaenderung
		if(yaw != 0)
			lage = rigidbody.rotation.eulerAngles.z;
		
		if(yaw == 0.0f){
			if(lage<180.0f)
				lage = lage*0.92f;
			else if(lage<360.0f)
				lage = lage*1.005f;
			
			//aktuelle neigung
			//transform.eulerAngles = new Vector3(rigidbody.rotation.eulerAngles.x,rigidbody.rotation.eulerAngles.y,lage);
		}
		
		//bei richtungsaenderung geschwindigkeit der neuen richtung anpassen
		rigidbody.velocity = Quaternion.Euler(new Vector3 (0, roll, 0)) * rigidbody.velocity;
    }

}

