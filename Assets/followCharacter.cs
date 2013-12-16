using UnityEngine;
using System.Collections;

public class followCharacter : MonoBehaviour {

	private GameObject bird;
		Vector3 hitposition;
	
    // has hand moved to target?
	//bool moved = false;
	
	Ray ray;
	
    LineRenderer line;
	
	RaycastHit[] raycastHits;
	Vector3 temp_hitposition;
	TrackSpaceMouse mouseTracker;
	public GameObject tox;
	
	void Start () {
		bird = GameObject.Find("BlockparBird(Clone)");
		mouseTracker = GameObject.Find("Spacemouse").GetComponent<TrackSpaceMouse>();
		transform.localPosition = transform.localPosition + bird.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = transform.localPosition + bird.transform.localPosition;
		
		
		raycastHits = new RaycastHit[0]; // init with empty array
		temp_hitposition = new Vector3(0.0f, 0.0f, 0.0f);
		
		ray.direction = transform.forward;
		ray.origin = transform.localPosition;
		// HACK: why sometimes line is null after adding line renderer as component?!
		if (line == null)
		{
			line = gameObject.GetComponent<LineRenderer>();
			Debug.Log("Warning: line was null. Got component again!");
		}
		
		// set line origin and direction
        line.SetPosition(0, ray.origin);
        line.SetPosition(1, ray.origin + (ray.direction * 400));

		/*
		RaycastHit hit;

		if (Physics.Raycast(ray.origin, ray.direction, out hit))
	    { 
			
			// Debug.Log ("hitpos: "+ hit.transform.localPosition);
			
		} 
		*/
		
		if(mouseTracker.button == 1){
			Debug.Log("1111");
		}
		if(mouseTracker.button == 2){
			GameObject toxi = (GameObject)Network.Instantiate(tox, transform.localPosition, Quaternion.identity,0);
			toxi.GetComponent<moveToxin>().direction = ray.direction ;
		
		}
		
		if(mouseTracker.button == 3) {
			Debug.Log("3");
		}
	
		
		
	}



	
	void OnEnable() 
	{
		if (gameObject.GetComponent<LineRenderer>() == null)
		{
			// add component line 
			gameObject.AddComponent<LineRenderer>();
			line = gameObject.GetComponent<LineRenderer>();
			line.SetWidth(0.1f, 0.1f);

			Debug.Log("LineRenderer added!");
		}
		else
		{
			Debug.Log("OnEnable: LineRenderer already available!");
		}
		
	
		ray = new Ray(transform.localPosition, transform.forward);
		 
		
	
	
	
	
	
	}
	
	
	void OnDisable() 
	{
		if (gameObject.GetComponent<LineRenderer>() != null)
		{
			Destroy(GetComponent<LineRenderer>());
			line = null;
			
			Debug.Log("LineRenderer destroyed!");
		}
		else
		{
			Debug.Log("OnDisable: no LineRenderer available to destroy!");	
		}
	}

}
