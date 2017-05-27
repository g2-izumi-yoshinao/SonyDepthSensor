using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventSystemMap {

	const string EID_OPENHAND      = "OpenHand";
	const string EID_MEWALKFRNT    = "MECharaWalkFront";
	const string EID_EXPDLG        = "ExplanationDlg";
	const string EID_FIRSTSONHAND  = "FirstSonHand";
	const string EID_FIRSTSONME    = "FirstSonME";
	const string EID_SECONDSONHAND = "SecondSonHand";
	const string EID_SECONDSONME   = "SecondSonME";
	const string EID_THIRDSONHAND  = "ThirdSonHand";
	const string EID_THIRDSONME    = "ThirdSonME";

	GameEventMapper gameEventMapper;
	ChainEventObject rootEventObject;

	public GameEventSystemMap (GameEventMapper gameEventMapper)
	{
		this.gameEventMapper = gameEventMapper;
	}

	//create event tree
	public void MapEvent()
	{
		gameEventMapper.Map ();
		rootEventObject = gameEventMapper.GetRootEventObject ();
	}

	public ChainEventObject GetRootEventObject()
	{
		return rootEventObject;
	}

	public List<ChainEventObject> GetAllEventObjects()
	{
		if (rootEventObject == null)
			return null;
		
		List<ChainEventObject> eventObjects = new List<ChainEventObject> ();
		eventObjects.Add (rootEventObject);
		//recursively add child event objects
		AddEventObjectsInChildren (rootEventObject, eventObjects);

		return eventObjects;
	}

	void AddEventObjectsInChildren (ChainEventObject parent, List<ChainEventObject> eventObjects)
	{
		List<ChainEventObject> eventList = parent.GetChildEventObject ();

		//no child events
		if (eventList == null || eventList.Count == 0)
			return;

		//add child events
		foreach (ChainEventObject child in parent.GetChildEventObject())
		{
			if (child != null) 
			{
				eventObjects.Add (child);
				AddEventObjectsInChildren (child, eventObjects);
			}
		}
	}

	public void Destroy(){ }

}
