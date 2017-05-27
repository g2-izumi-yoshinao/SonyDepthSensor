using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class GameEventSystem {

	GameEventSystemMap map;
	ChainEventObject rootEventObject;

	//triggerd when event set as "isFinalEvent" is finished
	bool _isEventFinished = false;
	public bool isEventFinished{get{ return _isEventFinished;}}

	public event Action onFinalEventFinished;

	public GameEventSystem (GameEventMapper gameEventMapper)
	{
		map = new GameEventSystemMap (gameEventMapper);
		map.MapEvent ();
		rootEventObject = map.GetRootEventObject ();
	}

	public void StartEventSystem ()
	{
		_isEventFinished = false;

		//set final event message trigger detection
		SetEventToFinalEventObjects();

		//start first event
		rootEventObject.Activate ();
	}

	public List<ChainEventObject> GetCurrentActiveEvents()
	{
		List<ChainEventObject> allEventList = map.GetAllEventObjects ();
		return allEventList.Where (e => e.isActive).ToList ();
	}

	void SetEventToFinalEventObjects()
	{
		List<ChainEventObject> allEventList   = map.GetAllEventObjects ();
		List<ChainEventObject> finalEventList = allEventList.Where (e => e.isFinalEvent).ToList ();

		//no final event
		if (finalEventList.Count == 0)
			return;
//		Debug.Log ("finalEventList count : " + finalEventList.Count);
		foreach (ChainEventObject chain in finalEventList) 
		{
//			Debug.Log ("finalEvent : " + chain.eventName);
			chain.onActivate += () => {
				//set is finished flag to true
				_isEventFinished = true;
//				Debug.Log ("finalEvent triggered!! : " + chain.eventName);
				//can be triggered only once
				if (onFinalEventFinished != null)
				{
					onFinalEventFinished();
					onFinalEventFinished = null;
				}
			};

		}
	}

	public void Destroy()
	{
		if (map != null) 
		{
			map.Destroy ();
			map = null;
		}
		_isEventFinished = false;
	}

}
