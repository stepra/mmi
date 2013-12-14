using UnityEngine;
using System.Collections;

public class flyTest2 : MonoBehaviour {

	void Start () {
		rigidbody.freezeRotation = true;
	}
	

               
    // Update is called once per frame
       
    void FixedUpdate ()
    {
		speed();
		steer();
    }
	
	
	
	private bool touchGround = false;
	
	void speed(){
		//50.0f weglassen???
		float throttle = Mathf.Clamp01(50.0f *Input.GetAxis("thrust"));
		
		//gleitflug&gravitation
		if(throttle == 0.0f && !touchGround){
			rigidbody.AddForce(Vector3.up*-50.0f,ForceMode.Acceleration);
		}
		//fliegen
		else /*if (throttle != 0.0f && !touchGround)*/{
			rigidbody.velocity = (transform.forward*200.0f*throttle);
			touchGround=false;
		}
	}
	
	//onGround
	void OnCollisionStay(Collision collisionInfo) {
		touchGround=true;
	}
	
	
	
	private float lage = 0.0f;
	
	void steer()
    {
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
		
        roll = Input.GetAxis("Horizontal") * (Time.fixedDeltaTime * 100.0f);
        pitch = Input.GetAxis("Vertical") * (Time.fixedDeltaTime * 100.0f);
		yaw = -Mathf.Clamp(roll,-0.75f,0.75f); //neigung nach links bzw rechts
        AddRot.eulerAngles = new Vector3(-pitch, roll, yaw);

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
			transform.eulerAngles = new Vector3(rigidbody.rotation.eulerAngles.x,rigidbody.rotation.eulerAngles.y,lage);
		}
		
		//bei richtungsaenderung geschwindigkeit der neuen richtung anpassen
		rigidbody.velocity = Quaternion.Euler(new Vector3 (0, roll, 0)) * rigidbody.velocity;
    }

}


