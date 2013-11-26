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

using UnityEngine;
using System.Collections;

/// <summary>
/// Class to select and manipulate scene objects with gogo interaction technique (IT). 
/// 
/// GoGo is a 1st person view IT
/// </summary>
public class GoGoInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement GoGo interaction technique
	----------------------------------------------------------------- */
    // marker handling and 1st person view handling
    GameObject interaction_origin = null;
	GameObject tracker= null;
	
	
    //interaction technique stuff
    public float armlength = 12.2f; // the length of your arm = 0,6cm * 20
    float k = 1 + 0.99f; // GoGo Factor, add 1.00 because we measure in [m]
    float d; //threshold linear <-> exponential mapping
    public float gogo; 
	
    Vector3 interactionOriginCoordinates;
    Vector3 vec_distance;

    float distance; //distance between interaction origin and virtual hand (marker1)

    /// <summary>
    /// </summary>
    public void Start()
    {   
        //find interaction origion game object
        interaction_origin = GameObject.Find("InteractionOrigin");

		tracker = GameObject.Find("TrackerObject");
		
        // define threshold for linear/exponentiell mapping
        d = 0.5f * this.armlength; // use 2/3 of armlength
    }

    /// <summary>
    /// Implementation of concrete IT selection behaviour. 
    /// </summary>
    protected override void UpdateSelect()
	{
        // INTERACTION TECHNIQUE THINGS ------------------------------------------------
        if (tracker.transform.parent.GetComponent<TrackMarker>().isTracked())
        {
			// show hands (virtual and physical)
			tracker.transform.parent.GetComponent<TrackMarker>().setVisability(gameObject, true);

            // ------------- Interaction technique implementation -------

            //get current interaction origin coordinates
            interactionOriginCoordinates = interaction_origin.transform.position;
    		
            // get the distance between torso and virtual hand physical position
            distance = Vector3.Distance(tracker.transform.position, interactionOriginCoordinates);

            // get the distance vector between torso and virtual hand physical position in WC
		    vec_distance = (tracker.transform.position - interactionOriginCoordinates);

            if (distance > d)
            {
                //exponential mapping
                //Debug.Log("gogo");

                // Mapping for gogo: Rr + k(Rr – D)2; rr = length vector torso -> virtual hand
			    gogo = k * Mathf.Pow((distance - d), 2.0f);
            }
            else
            {
			    //linear mapping
                //Debug.Log("linear");
    			
                // Mapping for gogo: Rr; rr = length vector torso -> virtual hand
                gogo = 0.0f;
            }
    	
		    //Update transform of the selector object (virtual hand)
            this.transform.position = tracker.transform.position + gogo * vec_distance.normalized;
		    this.transform.rotation = tracker.transform.rotation;
    	
            // Transform (transform and rotate) selected object in contect of virtual hand's transformation
            if (selected)
            {	
			    this.transformInter(this.transform.position, this.transform.rotation);
            }
        }
        else
        {
            // make invisible
			tracker.transform.parent.GetComponent<TrackMarker>().setVisability(gameObject, false);
        }
    }
    // ------------------ VRUE Tasks END ----------------------------
	
    /// <summary>
    /// Callback
    /// If our selector-Object collides with anotherObject we store the other object 
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other">GameObject giben by the callback</param>
    public void OnTriggerEnter(Collider other)
    {		
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {

                collidees.Add(collidee.GetInstanceID(), collidee);
                //Debug.Log(collidee.GetInstanceID());

                // change color so user knows of intersection
                collidee.renderer.material.SetColor("_Color", Color.blue);
            }
        }
    }

    /// <summary>
    /// Callback
    /// If our selector-Object moves out of anotherObject we remove the other object from our list
    /// 
    /// For usability purpose change color of collided object
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (isOwnerCallback())
        {
            GameObject collidee = other.gameObject;

            if (hasObjectController(collidee))
            {
                collidees.Remove(collidee.GetInstanceID());

                // change color so user knows of intersection end
                collidee.renderer.material.SetColor("_Color", Color.white);
            }
        }
    }
}