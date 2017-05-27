using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEventObject : EventObject{

	List<ChainEventObject> childEventObjects;

	public event Action onActivate;
	public event Action onTrigger;

	//is set to true, the whole event map will end when reach this event
	public bool isFinalEvent = false;

	public ChainEventObject (string eventName) : base (eventName){ }

	//when trigger was detected, execute this method to move to next event
	public override void Trigger()
	{
		Debug.Log (eventName + " event triggered!! move to child events");

		if (onTrigger != null)
			onTrigger ();

		EndEventAction ();
	}

	public override void Destroy()
	{
		isActive = false;

		if (onActivate != null)
			onActivate = null;

		if (onTrigger != null)
			onTrigger = null;
	}

	#region Chain Factor

	//implement trigger turn on. For example, turn on the collider to detect collision as event trigger.
	public void Activate ()
	{
		isActive = true;

		if (onActivate != null)
			onActivate ();
	}

	//add eventHandler to detection in order to handle trigger.
	public void AddEventHandler ()
	{

	}
		
	void EndEventAction ()
	{
		//disable current event
		isActive = false;

		//early return if no child
		if (childEventObjects == null || childEventObjects.Count == 0)
			return;

		//if there is child event object, activate those events
		foreach (ChainEventObject eventObject in childEventObjects) 
		{
			eventObject.Activate ();
		}
	}

	public void AddChildEventObject (ChainEventObject eventObject)
	{
		if(childEventObjects == null)
			childEventObjects = new List<ChainEventObject> ();

		if (childEventObjects.Contains (eventObject))
			Debug.LogWarning ("Error : child event objects already contains assined object");
		else
			childEventObjects.Add (eventObject);
	}

	public List<ChainEventObject> GetChildEventObject ()
	{
		return childEventObjects;
	}

	public EventObjectTrigger GetEventObjectTrigger()
	{
		return new EventObjectTrigger (this);
	}

	#endregion
}

