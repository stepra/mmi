using UnityEngine;
using System.Collections;

public class rocks : MonoBehaviour {
	
	
	private Vector3 startPoint = new Vector3(576,1030,-98);
	private int count = 0;
	public GameObject rock;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		if(count>100){
			count=0;
			rock.transform.localScale = new Vector3(Random.Range(3.0f,15.0f),Random.Range(3.0f,15.0f),Random.Range(3.0f,15.0f));
			Object o = Instantiate(rock,startPoint+new Vector3(Random.Range(-30.0f,0),0,Random.Range(-30.0f,0)),Quaternion.identity);
			Destroy(o,30.0f);
			
		}
	}
}
