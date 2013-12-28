using UnityEngine;
using System.Collections;

public class ai : MonoBehaviour {
	
	Vector3 speed;
	bool itemBounceUp = true;
	int count = 0;

	// Use this for initialization
	void Start () {
		speed = new Vector3(Random.Range(-0.2f,0.2f),0.0f,0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		
		
		
		if(itemBounceUp)
			this.transform.position = this.transform.position + speed;
		else
			this.transform.position = this.transform.position - speed;
		
		count++;
		if(count>100){
			count=0;
			itemBounceUp = !itemBounceUp;
			if(itemBounceUp)
				speed = new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f));
		}
			

	}
	
	
	void itembounce () {
		while(true)
			StartCoroutine(waiting(1.0f));
		}
	
	IEnumerator waiting(float t){
		yield return new WaitForSeconds(t);
		itemBounceUp = !itemBounceUp;
	}
	
	
	
}










