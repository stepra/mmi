/* =====================================================================================
 * ARTiFICe - Augmented Reality Framework for Distributed Collaboration
 * ====================================================================================
 * Copyright (c) 2010-2012 
 * 
 * Annette Mossel, Christian Schönauer, Georg Gerstweiler, Hannes Kaufmann
 * mossel | schoenauer | gerstweiler | kaufmann @ims.tuwien.ac.at
 * Interactive Media Systems Group, Vienna University of Technology, Austria
 * www.ims.tuwien.ac.at
 * 
 * ====================================================================================
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * =====================================================================================
*/

/*
 *REFERENCE fix network lag
 * http://www.paladinstudios.com/2013/07/10/how-to-create-an-online-multiplayer-game-with-unity/
 */

using UnityEngine;
using System.Collections;


public class NetworkAttributeSynchronization :  MonoBehaviour
{
	public bool distributePos = true;
	public bool distributeRot = true;
	public bool distributeVisibility = true;
	public bool distributeActivSelf = true;
	///Network Lag Solution through interpolation
	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0.0f;
	private float syncTime = 0.0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;
	private bool interpolate = false;
	
	void Update(){
		if(!networkView.isMine && interpolate){
			synchronizePosition();
		}
	}
	
	private void synchronizePosition(){
		syncTime += Time.deltaTime;
		rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay) ;
		//Debug.Log (syncStartPosition +" -> " + transform.localPosition + " -> " +syncEndPosition + "  || moveToward " + syncTime / syncDelay + " || syncTime " + syncTime +" || syncDelay " +syncDelay);
	}
	
	 /// <summary>
    /// Callback to stream data to all other clients and the server.
    /// For the distribution of the position, localRotation, and renderer visibility
    /// </summary>
    /// <param name="stream">Bitstream used</param>
    /// <param name="info">Info of the sender</param>
    public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {	
		Vector3 syncVelocity = Vector3.zero;
		Vector3 syncPosition = Vector3.zero;
        if (stream.isWriting)
        {
            //Executed on the owner of the networkview;
            //position and orientation is distributed over the network
           	if (distributePos)
			{
				if(GetComponent<Rigidbody>() != null){
					syncPosition = rigidbody.position;
	            	stream.Serialize(ref syncPosition);//"Encode" it, and send it
				
					syncVelocity = rigidbody.velocity;
       				stream.Serialize(ref syncVelocity);
				}
				
			}
			
			if (distributeRot)
			{
				Quaternion rot = transform.localRotation;
	            stream.Serialize(ref rot);//"Encode" it, and send it
			}
			
			if (distributeVisibility)
			{
				bool rendererState = true;
				// also search for child objects, which should be enables / disabled if marker is not tracked
				Renderer[] rendererComponents = this.GetComponentsInChildren<Renderer>();
				if(rendererComponents != null)
				{
		        	foreach (Renderer component in rendererComponents) 
					{
						// todo: save and send every single renderer state
						rendererState = component.enabled;
					}
				}
				stream.Serialize(ref rendererState);
			}
			
                      
			
		
        }
        else
        {
			Debug.Log("distributePos");
            //Executed on the others; in this case the Clients and Server
            //The clients receive a pos & orient and set the object to it
			if (distributePos)
			{
			
	            stream.Serialize(ref syncPosition); //"Decode" it and receive it
				stream.Serialize(ref syncVelocity);
				rigidbody.velocity = syncVelocity;
				
				if (syncStartPosition.Equals(Vector3.zero)){
					rigidbody.position = syncPosition;
					syncStartPosition = rigidbody.position;
					}
				else{
					syncTime = 0.0f;
						
        			syncDelay = Time.time - lastSynchronizationTime;
        			lastSynchronizationTime = Time.time;
        			 	
        			syncEndPosition = syncPosition; //+ syncVelocity * syncDelay;
        			syncStartPosition = rigidbody.position;
						
					if(!interpolate)
					interpolate = true;
				}
			
			}
			
			if (distributeRot)
			{
				Quaternion rot = Quaternion.identity;
				stream.Serialize(ref rot); //"Decode" it and receive it
				transform.localRotation = rot;
			}
			
			if (distributeVisibility)
			{
				bool rendererState = true;
				stream.Serialize(ref rendererState);//"Decode" it and receive it
	      		
				
				Renderer[] rendererComponents = this.GetComponentsInChildren<Renderer>();
				
				// enable/disable all child components, due to marker visibility
				if(rendererComponents != null)
				{
		        	foreach (Renderer component in rendererComponents) 
					{
		            	component.enabled = rendererState;
					}
				}
			}
			
			
        }
    }
}
