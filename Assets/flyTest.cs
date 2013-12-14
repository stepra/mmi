using UnityEngine;
using System.Collections;

public class flyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("TASTENSCHLÄGE:");
	}
	
	/*
	 	Yaw: links/rechts lenken
		Pitch: nach oben/unten schwenken
		Roll: links/rechts kippen
		thrust: beschleunigung
	 */
	
    //The purpose of this script is to simulate Newtonian phy
    private float maxThrust = 50.0f; //The maximum Thrust provided by the thruster(s) at full throttle
    private float rollWeight = 0.3f; //This float and the next two only serve to adjust sensitivity
    private float pitchWeight = 0.31f;//of the controls, and to allow calibration for more massive ships.
    private float yawWeight = 0.3f;//Set these 3 floats to the mass of the rigidbody for sensitive controls
	
	private float forwardVelo = 0.0f;
	private float sqrVelo = 0.0f;
	private Vector3 dragDirection = new Vector3(0.0f,0.0f,0.0f);
	private Vector3 dragAndBrake = new Vector3(0.0f,0.0f,0.0f);
	private Vector3 dragForces = new Vector3(0.0f,0.0f,0.0f);
	
	private Vector3 stabilizationForces = new Vector3(0.0f,0.0f,0.0f);
	private Vector3 stabilizingDrag = new Vector3(2.0f,1.0f,0.0f);
	
	
	private Vector3 drag = new Vector3(2.0f,8.0f,0.05f);
	private float brakeDrag = 0.0f;
	private float brake = 0.0f; //input 
	private float throttle = 0.0f;
	
	//Toruqe coefficient for elevator (pitch)
	private float elevator = 0.3f;

	//Base input for elevator (when no key is pressed), 
	//so plane stays on one height without user input
	private float elevatorCenterSetting = -0.25f;
	
	//Toruqe coefficient for ailerons (roll)
	private float ailerons = 0.03f;
	
	private bool touchGround = false;
               
    // Update is called once per frame
       
    void FixedUpdate ()
    {
		//float yaw = yawWeight*Input.GetAxis("yaw");
		/*float roll = 1.0f*Input.GetAxis("Horizontal");
		float yaw = 0.0f;
		float pitch = 1.0f*Input.GetAxis("Vertical");
        Vector3 Rotation = new Vector3(pitch, roll, yaw);
        rigidbody.AddRelativeTorque(Rotation);
        float throttle = maxThrust *Input.GetAxis("thrust");
       // rigidbody.AddRelativeForce(Vector3.up*throttle);
        System.Console.WriteLine("input is "+ yaw.ToString()+", "+pitch.ToString()+", "+roll.ToString());*/
		
		
		rigidbody.freezeRotation = true;
		//testFly();
		speed();
		UpdateFunction();
		
		
		
		/*if(Input.GetKey(KeyCode.B))
			Debug.Log("B");
		
		Debug.Log("____:"+Input.GetAxis("Vertical")+":_____");*/
    }
	
	private Vector3 vel = new Vector3(0.0f,0.0f,0.0f);
	
	
	void speed(){
		throttle = Mathf.Clamp01(maxThrust *Input.GetAxis("thrust"));
		//throttle=Mathf.Clamp01(throttle + Input.GetAxis("thrust") * Time.deltaTime);

		float engineForce = 200.0f;
		
		if(throttle == 0.0f && !touchGround){
			//rigidbody.AddForce((transform.up*-50.0f)+(transform.forward*100.0f));
			//rigidbody.velocity = ((Vector3.up*-50.0f)+(transform.forward*150.0f));

			rigidbody.AddForce(Vector3.up*-50.0f,ForceMode.Acceleration);
		}
		else{
			rigidbody.velocity = (transform.forward*engineForce*throttle);
			//rigidbody.AddForce(transform.forward*engineForce*throttle);
			touchGround=false;
		}
	}
	
	void OnCollisionStay(Collision collisionInfo) {
		touchGround=true;
	}
	
	void testFly(){
		throttle = Mathf.Clamp01(maxThrust *Input.GetAxis("thrust"));
		//throttle=Mathf.Clamp01(throttle + Input.GetAxis("thrust") * Time.deltaTime);

		float engineForce = 100.0f;
		rigidbody.AddForce(transform.forward*engineForce*throttle);
		//rigidbody.velocity = (transform.forward*engineForce*throttle);
		//wings and drag
	    forwardVelo = Vector3.Dot(rigidbody.velocity,transform.forward);
	    sqrVelo = forwardVelo*forwardVelo;
	
	    dragDirection = transform.InverseTransformDirection(rigidbody.velocity);
	    dragAndBrake = drag+new Vector3(0.0f,0.0f,brakeDrag*brake);
	    dragForces = -Vector3.Scale(dragDirection,dragAndBrake)*rigidbody.velocity.magnitude;
	    //rigidbody.AddForce(transform.TransformDirection(dragForces));
		
		//stabilization (to keep the plane facing into the direction it's moving)
    	stabilizationForces = -Vector3.Scale(dragDirection,stabilizingDrag)*rigidbody.velocity.magnitude;
    	//rigidbody.AddForceAtPosition(transform.TransformDirection(stabilizationForces),transform.position-transform.forward*10);
    	//rigidbody.AddForceAtPosition(-transform.TransformDirection(stabilizationForces),transform.position+transform.forward*10);
		
		//elevator
		//float pitchInput = Input.GetAxis("pitch");
		float pitchInput = Input.GetAxis("Vertical");
		//rigidbody.AddTorque(transform.right*sqrVelo*elevator*(pitchInput+elevatorCenterSetting));
		//rigidbody.AddTorque(transform.right*sqrVelo*(pitchInput-elevatorCenterSetting)*0.01f);
		//rigidbody.AddTorque(transform.right*sqrVelo*pitchInput*0.005f);

 		//ailerons
		//float rollInput = Input.GetAxis("roll");
		float rollInput = Input.GetAxis("Horizontal");
		//float yawInput = Input.GetAxis("yaw");
        //rigidbody.AddTorque(transform.up*sqrVelo*0.005f*rollInput);
		//rigidbody.AddTorque(0,sqrVelo*0.005f*rollInput,0);
		
	}
	
	
	private float lage = 0.0f;
	
	void UpdateFunction()
    {
		float RotationSpeed = 100.0f;
		float AmbientSpeed = 100.0f;
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
        roll = Input.GetAxis("Horizontal") * (Time.fixedDeltaTime * RotationSpeed);
        pitch = Input.GetAxis("Vertical") * (Time.fixedDeltaTime * RotationSpeed);
		yaw = -Mathf.Clamp(roll,-0.75f,0.75f);

       // yaw = Input.GetAxis("Yaw") * (Time.fixedDeltaTime * RotationSpeed);
        AddRot.eulerAngles = new Vector3(-pitch, roll, yaw);
		//rigidbody.AddRelativeTorque(new Vector3(-pitch, roll, 0.0f));
        rigidbody.rotation *= AddRot;
		
		if(yaw != 0)
			lage = rigidbody.rotation.eulerAngles.z;
		
		if(yaw == 0.0f){
			if(lage<180.0f)
				lage = lage*0.92f;
			else if(lage<360.0f)
				lage = lage*1.005f;
			transform.eulerAngles = new Vector3(rigidbody.rotation.eulerAngles.x,rigidbody.rotation.eulerAngles.y,lage);
		}
 		//transform.Translate(new Vector3(1, 0, 0) * roll);
		//transform.Translate(new Vector3(0, 1, 0) * pitch);
		
		rigidbody.velocity = Quaternion.Euler(new Vector3 (0, roll, 0)) * rigidbody.velocity;
    }

}


