using UnityEngine;
using System.Collections;

public class sendToClient : MonoBehaviour {
	
	public bool sendPosition;
	private bool connected;
	private GameObject spaceMouse;
	
	void FixedUpdate(){
		spaceMouse = GameObject.Find("VirtualHand(Clone)");
		if(spaceMouse != null){
			spaceMouse.GetComponent<NetworkView>().RPC("setSpaceMouseOrigin", RPCMode.OthersBuffered, rigidbody.position);
		}
	}
	
	
	/*
	void OnPlayerConnected(NetworkPlayer player) {
		
		connected = true;
        Debug.Log("hello World");
    }
	
	 void OnPlayerDisconnected(NetworkPlayer player) {
		connected = false;
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }
	*/
	
}
