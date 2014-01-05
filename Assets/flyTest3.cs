using UnityEngine;
using System.Collections;

public class flyTest3 : MonoBehaviour {
	
	public int lifes = 5;

	public float energy = 10.0f;
	private float energyLoss = 0.04f;
	private float energyLossTerrain = 0.1f;
	
	private bool showEnergyLoss = false;
	private bool showCheckpointReached = false;
	private bool showObstacletReached = false;
	
	private crashForce cf = null;
	
	private GameObject lastCheckpoint;
	public int checkpointID;
	
	private bool touchGround = true;
	
	public int obstacles = 0;
	
	private bool reset = true;
	
	private float inc = 1.0f;
	private string incType = "";
	private float inc2 = 1.0f;
	private string incType2 = "";
	
	private bool wabeColl = false;
	private GameObject wabe = null;
	
	void Start () {
		rigidbody.freezeRotation = true;
		lastCheckpoint = GameObject.Find("checkbox_start");
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
		    GUI.Label(new Rect(10,110,200,20), "Lifes: "+lifes);
			GUI.Label(new Rect(10,125,200,20), "Energy: "+energy);
		
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
		float throttle = Mathf.Clamp01(Input.GetAxis("thrust"));
		float pitch = Mathf.Clamp01(50.0f *Input.GetAxis("Vertical"));
		float wings = Mathf.Clamp01(Input.GetAxis("fly"));
		
		if(energy<=0.0f){
			throttle=pitch=wings=0.0f;	
		}
		else{
			energy-=Mathf.Clamp01(Input.GetAxis("thrust"))*energyLoss;
			
		}
		
		if(throttle == 0.0f && pitch != 0.0f && wings != 0.0f){ //gleitflug
			if(incType=="gleit"){
				if(inc<6.0f)	
					inc*=1.01f;
			}
			else{
				inc = 1.1f;
				incType = "gleit";
			}	
			
			throttle-=0.2f*inc;
			pitch*=(inc/2);
		}
		else if(throttle == 0.0f && wings == 0.0f){ //nichts gedrückt bzw nur vorlehnen
			if(incType=="none"){
				if(inc<6.0f)	
					inc*=1.01f;
			}
			else{
				inc = 1.1f;
				incType = "none";
			}	
			
			throttle = -0.8f*inc;
			pitch = 0.5f*inc;
		}
		else if(throttle == 0.0f && pitch == 0.0f && wings != 0.0f){ //nur flügel gespannt
			if(incType=="wings"){
				if(inc<6.0f)	
					inc*=1.01f;
			}
			else{
				inc = 1.1f;
				incType = "wings";
			}	
			
			throttle = -0.25f*inc;
			pitch = 0.15f*inc;
		}else{
			if(incType=="top" && throttle>=1.0f){
				if(inc<4.0f)	
					inc*=1.01f;
			}
			else{
				inc = 1.1f;
				incType = "top";
			}	
			
			throttle*=inc;
			pitch*=inc;
		}
		
		
		
		
	
		if(touchGround){
			if(	throttle<=0.0f){
				rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
				
				if(energy<=0.0f && reset){
					
					reset = false;
					lifes--;
				
					if(lifes>0){
						transform.position = lastCheckpoint.transform.position + new Vector3(0.0f,5.0f,0.0f);
						StartCoroutine(waiting(3));
					}
					else if(lifes<=0){
						transform.position = GameObject.Find("start").transform.position + new Vector3(0.0f,5.0f,0.0f);
						StartCoroutine(waiting(3));
					}
					
				}
			}
			else{
				touchGround=false;
			}
		}
		else if(wabeColl){
			rigidbody.velocity = new Vector3(0.0f,0.0f,0.0f);
		}
		else{
			rigidbody.velocity = (transform.forward*100.0f*pitch) + (transform.up*100.0f*throttle);	
			
			if(cf != null){
				Vector3 tmp = rigidbody.velocity;
				//rigidbody.velocity = tmp + transform.up*-cf.force;
				rigidbody.velocity = new Vector3(tmp.x,-cf.force,tmp.z);
			}
		}
		
			
		
	}
	
	IEnumerator waiting(int t){
		yield return new WaitForSeconds(t);
		if(lifes<=0)
			lifes = 5;
		energy = 100.0f;
		reset = true;
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
					energy-= cf.energyLoss;
					Invoke("disableCrashForce", 1);
		}
			else if(other.gameObject.tag=="life"){
					Destroy(other.gameObject);
					lifes++;
		}
		
		//TODO: Steffi
		else if(other.gameObject.tag=="wabe"){
					wabeColl=true;
					wabe = other.gameObject;
		}
		
	}
	
	private float lage = 0.0f;
	
	void steer()
    {
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float yaw = 0;
		
        roll = Input.GetAxis("Horizontal") * (Time.fixedDeltaTime * 25.0f);
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

