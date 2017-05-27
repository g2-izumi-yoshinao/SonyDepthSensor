using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventMapperInAppSecond : GameEventMapper {

	const string EID_READYBUTTON      = "ReadyButton";
	const string EID_STARTREALGAME    = "StartRealGame";


	public override void Map()
	{
		//get reference
		GameUIController gameUIController = GameObject.Find("Canvas").GetComponent<GameUIController>();

		//root Event Object
		rootEventObject = new ChainEventObject("root");

		//Wait until player holds ME character's cheek
		ChainEventObject holdCheekEvent = new ChainEventObject(EID_READYBUTTON);
		//TODO: set event
		rootEventObject.AddChildEventObject (holdCheekEvent);

		//show OK Dialog for like 2 seconds
		ChainEventObject okDlgEvent = new ChainEventObject(EID_STARTREALGAME);
		//TODO: set event
		holdCheekEvent.AddChildEventObject (okDlgEvent);

		//end event
		EndEvent (okDlgEvent);

	}
}
