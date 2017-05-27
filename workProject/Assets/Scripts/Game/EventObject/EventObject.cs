using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventObject {

	public bool isActive = false;

	string _eventName;
	public string eventName{ 
		get{ return _eventName; }
	}

	public EventObject (string eventName)
	{
		this._eventName = eventName;
	}

	//trigger the event
	public abstract void Trigger();

	//release the event
	public virtual void Destroy()
	{
		isActive = false;
	}

}
	
public class EventObjectTrigger{

	EventObject eventObject;

	public EventObjectTrigger (EventObject eventObject)
	{
		this.eventObject = eventObject;
	}

	public void Trigger()
	{
		eventObject.Trigger ();
	}

}