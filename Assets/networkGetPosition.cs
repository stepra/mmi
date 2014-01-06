using UnityEngine;
using System.Collections;

public class networkGetPosition : MonoBehaviour {
	
	[RPC]
    public virtual void getPos(Vector3 pos)
    {
        
		pos = transform.localPosition;
		//Debug.Log ("posbird" + transform.localPosition);
		
    }
}
