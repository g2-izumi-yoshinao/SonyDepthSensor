using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventMapperInAppFirst : GameEventMapper {

	const string EID_HOLDCHEEK      = "HoldCheek";
	const string EID_OKDLG    = "OkDialog";


	public override void Map()
	{
		//get reference
		GameUIController gameUIController = GameObject.Find("Canvas").GetComponent<GameUIController>();

		//root Event Object
		rootEventObject = new ChainEventObject("root");

		//Wait until player holds ME character's cheek
		ChainEventObject holdCheekEvent = new ChainEventObject(EID_HOLDCHEEK);
		//TODO: set event
		rootEventObject.AddChildEventObject (holdCheekEvent);

		//show OK Dialog for like 2 seconds
		ChainEventObject okDlgEvent = new ChainEventObject(EID_OKDLG);
		//TODO: set event
		holdCheekEvent.AddChildEventObject (okDlgEvent);

		//end event
		EndEvent (okDlgEvent);

	}
}
