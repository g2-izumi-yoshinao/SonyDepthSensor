using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTimeScheduler{

	//name of the tape
	string _schedulerName;
	public string schedulerName{ 
		get{ return _schedulerName; }
	}

	//total length of the tape
	float _length;
	public float length{ 
		get{ return _length; }
	}

	//list of event objects in tape (key: time, value: event)
	List<TimeEventObject> eventObjects;

	public GameTimeScheduler(string schedulerName, float length)
	{
		this._schedulerName  = schedulerName;
		this._length    = length;

		eventObjects = new List<TimeEventObject> ();
	}

	public void Destroy()
	{
		_schedulerName = string.Empty;
		_length   = 0;

		if (eventObjects == null || eventObjects.Count == 0)
			return;

		foreach (TimeEventObject e in eventObjects) 
			e.Destroy ();

		eventObjects.Clear ();
		eventObjects = null;
	}

	public void AddEvent(TimeEventObject eventObject)
	{
		if (!eventObjects.Contains (eventObject))
			eventObjects.Add (eventObject);
	}

	public TimeEventObject GetEventAtTime(float time)
	{
		return eventObjects.Where( e => e.isActive).Where( e => e.time <= time).FirstOrDefault();
	}

}

