using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEventMapper {

	const string EID_END      = "end";

	protected ChainEventObject rootEventObject;

	public abstract void Map ();

	public ChainEventObject GetRootEventObject ()
	{
		return rootEventObject;
	}

	protected void EndEvent (ChainEventObject lastEvent)
	{
		ChainEventObject endEvent = new ChainEventObject(EID_END);
		endEvent.isFinalEvent = true;
		lastEvent.AddChildEventObject (endEvent);
	}

}
