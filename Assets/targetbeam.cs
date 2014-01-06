using UnityEngine;
using System.Collections;

public class targetbeam : MonoBehaviour {
	Ray ray;
    LineRenderer line;
	RaycastHit[] raycastHits;
	Vector3 temp_hitposition;
	Transform virtualHand;
	
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		/*
		raycastHits = new RaycastHit[0]; // init with empty array
		temp_hitposition = new Vector3(0.0f, 0.0f, 0.0f);
		*/
		ray.direction = virtualHand.forward;
		ray.origin = virtualHand.localPosition;
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
	
	}
	
		
	void OnEnable() 
	{
		virtualHand = transform.parent;
		if (gameObject.GetComponent<LineRenderer>() == null)
		{
			// add component line 
			gameObject.AddComponent<LineRenderer>();
			line = gameObject.GetComponent<LineRenderer>();
			line.SetWidth(0.05f, 0.05f);

			//Debug.Log("LineRenderer added!");
		}
		else
		{
			//ebug.Log("OnEnable: LineRenderer already available!");
		}
		
	
		ray = new Ray(virtualHand.localPosition, virtualHand.forward);
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
