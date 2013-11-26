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
/// Class to select and manipulate scene objects with HOMER interaction technique (IT). 
/// 
/// HOMER is a 1st person view IT
/// </summary>
public class HomerInteraction : ObjectSelectionBase
{
	/* ------------------ VRUE Tasks START -------------------
	* 	Implement Homer interaction technique
	----------------------------------------------------------------- */
    // marker handling and 1st person view handling
    GameObject interaction_origin = null;
	GameObject tracker= null;
	
    Vector3 interactionOriginCoordinates;
    Vector3 virtualHandCoordinates;
    Vector3 physicalHandCoordinates;
	Vector3 temp_physicalHandCoordinates;

  	float dist_th = 0.0f;
    float dist_to = 0.0f;
    float dist_vh = 0.0f; 
    float dist_thcurr = 0.0f;
	
	Vector3 hitposition;
	
    // has hand moved to target?
	bool moved = false;
	
	Ray ray;
	Vector3 ray_direction;
    LineRenderer line;
	
	bool singleSelection;
		
	Hashtable oldSelectionObjects; // temp store selected object for multiselection
	

    /// <summary>
    /// </summary>
    void Start()
    {   
		line = null;
		
        //find interaction origion game object
        interaction_origin = GameObject.Find("InteractionOrigin");
        
		tracker = GameObject.Find("TrackerObject");
		
		singleSelection = false;
		oldSelectionObjects = new Hashtable();
		
    }
	
	/// <summary>
	/// Unity Callback: if component is enabled during run time add line renderer component to game object 
	/// for rendering homer ray
    /// </summary>
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
		
	}
	
	/// <summary>
	/// Unity Callback: if component is disabled during run time destroy line renderer component of game object 
    /// </summary>
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

    /// <summary>
    /// Implementation of concrete IT selection behaviour. 
    /// </summary>
    protected override void UpdateSelect()
	{
		
        // INTERACTION TECHNIQUE THINGS ------------------------------------------------
        if (tracker.transform.parent.GetComponent<TrackMarker>().isTracked())
        {
            // show hands (virtual)
			tracker.transform.parent.GetComponent<TrackMarker>().setVisability(this.gameObject, true);
			
            //get current interaction origin coordinates
            interactionOriginCoordinates = interaction_origin.transform.position;

            // get current translate vector for virtual hand object
            physicalHandCoordinates = tracker.transform.position; 
			
			this.transform.rotation = tracker.transform.rotation;
			
            if (!base.selected)
            {
				if( Input.GetKeyDown("m") )
				{ 
					singleSelection = !singleSelection; 
				}	
				
                // direction vector from interaction origin -> homer virtual hand
                ray_direction = (physicalHandCoordinates - interactionOriginCoordinates).normalized;
                
				this.rayCast();
				
                //Update transform of the selector object (virtual hand)
                this.transform.position = physicalHandCoordinates;
				
				// set back to default value -> virtual hand position = physical hand position
				moved = false;
            }
            else if (base.selected)  
            {
                // Transform (transform and rotate) selected object in contect of virtual hand's transformation
				line.enabled = false;

				if (!moved) // only do that when vh has not yet been moved to selected target
                {
					// distance between torso and virtual hand at the beginning of manipulation - must stay the same throughout manipulation
            		dist_th = Vector3.Distance(physicalHandCoordinates, interactionOriginCoordinates);
					
					// get distance torso -> selected target
	            	dist_to = Vector3.Distance(hitposition, interactionOriginCoordinates);

					// hand has been moved to target 
                    moved = true;
                } 
				
				// get current translate vector for virtual hand object
	            dist_thcurr = Vector3.Distance(physicalHandCoordinates, interactionOriginCoordinates);
	
	            // get virtual hand distance
	            dist_vh = dist_thcurr * (dist_to/dist_th);
				
				this.transform.position = interactionOriginCoordinates + dist_vh * (physicalHandCoordinates - interactionOriginCoordinates).normalized;
				this.transformInter(this.transform.position, this.transform.rotation);
            } 

        }// close if tracked
        else
        {
            // make invisible
			tracker.transform.parent.GetComponent<TrackMarker>().setVisability(this.gameObject, false);
        }
   
    } 
	
	/// <summary>
    /// Implementation of single/multi raycast. 
    /// </summary>
	private void rayCast()
	{
		RaycastHit[] raycastHits = new RaycastHit[0]; // init with empty array
		Vector3 temp_hitposition = new Vector3(0.0f, 0.0f, 0.0f);
		
		// generate ray & visualize
        ray = new Ray(physicalHandCoordinates, ray_direction); // from virtual hand in ray direction
		
		// HACK: why sometimes line is null after adding line renderer as component?!
		if (line == null)
		{
			line = gameObject.GetComponent<LineRenderer>();
			Debug.Log("Warning: line was null. Got component again!");
		}
		
		// set line origin and direction
        line.SetPosition(0, ray.origin);
        line.SetPosition(1, ray.origin + (ray_direction * 400));

		
		if(singleSelection)
		{
			RaycastHit hit;

			if (Physics.Raycast(physicalHandCoordinates, ray_direction, out hit))
	        { 
				raycastHits = new RaycastHit[1];
				raycastHits[0] = hit;
			} 		
		} 
		else
		{																	
			raycastHits = Physics.RaycastAll(physicalHandCoordinates, ray_direction);
		}
				
		foreach(RaycastHit hit in raycastHits)
		{
			if (isOwnerCallback())
			{
				GameObject collidee = hit.collider.gameObject; // the game object attached to the collider -> do not use transform.gameobject -> returns parent
				
				if(hasObjectController(collidee))
				{
					if(collidees.Contains(collidee.GetInstanceID()))
					{
						// remove collidee from old key list, this object stays selected
						oldSelectionObjects.Remove(collidee.GetInstanceID());
					}
					else
					{
						collidees.Add(collidee.GetInstanceID(), collidee);
	
	            		// change color so user knows of intersection
	            		collidee.renderer.material.SetColor("_Color", Color.blue);
					}
				}
			}
		}
				
		// check if former collidee-objects are still in old list
		foreach(int key in oldSelectionObjects.Keys)
		{
			// change color so user knows of intersection end
			((GameObject)collidees[key]).renderer.material.SetColor("_Color", Color.white);
			
			// remove from hashtable
			collidees.Remove(key);
		}
		
		// emtpy old selection table for new frame
		oldSelectionObjects.Clear();
				
		foreach(int key in collidees.Keys)
	    {
			// and current selected object to temp old selection
			oldSelectionObjects.Add(key,null);	
				
			// calculate average for homer hand jump point
			temp_hitposition = temp_hitposition + ((GameObject)collidees[key]).transform.position;
		}
		
		this.hitposition = temp_hitposition / collidees.Count;
	}	
	// ------------------ VRUE Tasks END ----------------------------
}