using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TimeEventObject : EventObject{
	
	public float time;
	public event Action onTrigger;

	public TimeEventObject (string eventName) : base(eventName)
	{
		isActive = true;
	}

	//when trigger was detected, execute this method to move to next event
	public override void Trigger()
	{
		isActive = false;

		if (onTrigger != null)
			onTrigger ();
	}

	//release the event
	public override void Destroy()
	{
		base.Destroy ();

		if (onTrigger != null)
			onTrigger = null;
	}
}
