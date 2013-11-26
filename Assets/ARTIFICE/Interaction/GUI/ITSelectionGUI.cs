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
using System;
using System.Collections;


/// <summary>
/// Class to show a GUI to select a IT by ARToolkitMarker during runtime. 
/// </summary>
public class ITSelectionGUI : MonoBehaviour
{
	public GUIStyle vrue11Style; 
	protected string _vh; 
	protected string _triggerMarker;
	
	/* ------------------ VRUE Tasks START --------------------------
	* Place required member variables here
	----------------------------------------------------------------- */
	private GameObject _interactionObj = null;
	private ObjectSelectionBase[] _its;
	private ObjectSelectionBase _currentIT; 
	private ObjectSelectionBase _tempCurrentIT; 
	private string _currentMarker;
	private bool _itsAvailable;
	private bool _navigateThroughITs; 
	private bool _virtualHandHasSelected;
	// ------------------ VRUE Tasks END ----------------------------
	
	/// <summary>
	/// Set StartUp Data. Method is called by OnEnable Unity Callback
	/// Must be overwritten in deriving class
    /// </summary>
	protected virtual void StartUpData()
	{	
		// name of interaction object in Unity Hierarchy
		_vh = "VirtualHand";
		
		// name of trigger marker
		_triggerMarker = "Marker2";
	}
	
	/// <summary>
    /// </summary>
	void OnEnable()
	{	
		// set init data
		StartUpData();
		Debug.Log("IT Selection GUI enabled");
		
		/* ------------------ VRUE Tasks START --------------------------
		* find ITs (components) attached to interaction game object
		* if none is attached, manually attach 3 ITs to interaction game object
		* initially det default IT
		----------------------------------------------------------------- */
		_interactionObj = GameObject.Find(_vh);
		
		_itsAvailable = false;
		_navigateThroughITs = false;
		_virtualHandHasSelected = false;


		// get all ITs attached to interaction obj
		_its = _interactionObj.GetComponents<ObjectSelectionBase>();
		
		if (_its.Length > 0)
		{
			RegisterITs();
		}
		else
		{
			Debug.Log("No interaction techniques are attached to Interaction Object! Add by Script.");
			
			// add ITs manual
			_interactionObj.AddComponent<VirtualHandInteraction>();
			_interactionObj.AddComponent<GoGoInteraction>();
			_interactionObj.AddComponent<HomerInteraction>();
			
			_its = _interactionObj.GetComponents<ObjectSelectionBase>();
			
			RegisterITs();
		}
		
		// ------------------ VRUE Tasks END ----------------------------
	}


	/// <summary>
    /// Unity Callback
    /// OnGUI is called every frame for rendering and handling GUI events.
    /// </summary>
	void OnGUI () {
		
		/* ------------------ VRUE Tasks START --------------------------
		* check if ITs are available
		* if trigger marker is visible and no objects are currently selected by interaction game object show GUI
		* depending on visible marker switch through availabe ITs
		* implement user confirmation and set selected IT only if user has confirmed it
		* disable the GUI if virtual hand has selected objects and if user has confirmend an IT
		----------------------------------------------------------------- */
		if (_itsAvailable)
		{
			// check if someone is selected
			_virtualHandHasSelected = _currentIT.getSelectionState();
			
			// get current marker face
			_currentMarker = gameObject.GetComponent<MultiMarkerSwitch>().GetFaceFront();
			
			if ((_currentMarker == _triggerMarker) && !_navigateThroughITs)
			{
				_navigateThroughITs = true;
			}
			else if (_navigateThroughITs && Input.GetButtonUp("Fire1"))
			{
				// set IT depending on cube rotation
				SetCurrentIT(_tempCurrentIT);
				_navigateThroughITs = false;
			}
	

			if(!_virtualHandHasSelected && _navigateThroughITs)
			{
				// Make a group on the center of the screen
				GUI.BeginGroup (new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 300, 500));	
				
				// switch through ITs
				SwitchThroughIT(_currentMarker);

				// We'll make a box so you can see where the group is on-screen.
				GUI.Box (new Rect (0,0,300,50), _tempCurrentIT.GetType().ToString(), vrue11Style);
				
				// End the group we started above. This is very important to remember!
				GUI.EndGroup ();
		    }
		}
		// ------------------ VRUE Tasks END ----------------------------
	}
	
	
	/* ------------------ VRUE Tasks START -------------------
	----------------------------------------------------------------- */

		
	/// <summary>
    /// Called on Startup to register the attached IT components by first disabling them all
    /// and then enable the default IT. 
    /// </summary>
	private void RegisterITs()
	{
		_itsAvailable = true;
		
		// to be on the safe side disable all attached components
		foreach (ObjectSelectionBase it in _its)
		{
			it.enabled = false;
		}
		
		// by default set VirtualHand Interaction
		_currentIT = null;
		_tempCurrentIT = null;
		SwitchThroughIT("");
	}
	
	/// <summary>
    /// Depending on given marker name switch through ITs to show them in the GUI. 
    /// Does not set the current IT
    /// </summary>
	private void SwitchThroughIT(String markerName)
	{
		bool firstSelection = false;
		
		// disable current IT component
		if(_currentIT == null)
		{
			firstSelection = true;
		}
		
		foreach (ObjectSelectionBase it in _its)
		{

			if (firstSelection)
			{
				if (it.GetType().ToString() == "VirtualHandInteraction")
				{
					// set it as current
					_tempCurrentIT = it;
					SetCurrentIT(_tempCurrentIT);

					//_currentIT = it;
					//it.enabled = true;
					
					Debug.Log("Init Interaction Technique set!");
				}
			} 
			else
			{

				if (//_currentIT != it && (
				      (markerName == "Marker4" && it.GetType().ToString() == "GoGoInteraction") ||
				      (markerName == "Marker0" && it.GetType().ToString() == "HomerInteraction")  || 
				      (markerName == "Marker5" && it.GetType().ToString() == "VirtualHandInteraction")
				     //)
				   )
				{
					_tempCurrentIT = it;
					return;
				} 

			}
		}
		
		if (_tempCurrentIT == null)
		{
			Debug.Log("Could not set Interaction Technique!");
			_itsAvailable = false;
		}
	}
	
	/// <summary>
    /// Set given IT by enabling its corresponding component and disabling the other. 
    /// </summary>
	private void SetCurrentIT(ObjectSelectionBase it)
	{

		if(_currentIT != null)
		{	
			// disable current IT component
			_currentIT.enabled = false;
		}
		
		// set new current it and enable
		_currentIT = it;
		_currentIT.enabled = true;
		
		_tempCurrentIT = _currentIT;
	}
	// ------------------ VRUE Tasks END ----------------------------

}
