using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventMapperOutApp : GameEventMapper {

	const string EID_CHARASHOWFINISH = "CharaShowFinish";
	const string EID_SHOWSTARTDLG    = "ShowStartDialog";
	const string EID_EXPDLG        = "ExplanationDlg";
	const string EID_FIRSTSONHAND  = "FirstSonHand";
	const string EID_FIRSTSONME    = "FirstSonME";
	const string EID_SECONDSONHAND = "SecondSonHand";
	const string EID_SECONDSONME   = "SecondSonME";
	const string EID_THIRDSONHAND  = "ThirdSonHand";
	const string EID_THIRDSONME    = "ThirdSonME";

	public override void Map()
	{
		GameViewControllerOutApp gameViewControllerOutApp = GameObject.Find ("Canvas").GetComponent<GameViewControllerOutApp>();

		//root Event Object
		rootEventObject = new ChainEventObject("root");

		//all character show finish event
		ChainEventObject characterShowFinishEvent = new ChainEventObject(EID_CHARASHOWFINISH);
		//TODO: add event to maybe character or somthing 
		rootEventObject.AddChildEventObject (characterShowFinishEvent);

		//lets start dialog event
		ChainEventObject showStartDlg = new ChainEventObject(EID_SHOWSTARTDLG);
		//TODO: add event to maybe character or somthing 
		showStartDlg.onActivate += () => {
			
			//show Dialog that dissapears in few seconds
			gameViewControllerOutApp.ShowDialog (WORDS.OUT_GAME_UI_02, 2.0f);

			//go to next event after 3sec
			MyTimerFinder.Find().StartTimer( 3.0f, ()=>{ showStartDlg.Trigger();});
		};
		characterShowFinishEvent.AddChildEventObject (showStartDlg);

		EndEvent (showStartDlg);


		//game start event
//		ChainEventObject MECharaWalkFrontEvent = new ChainEventObject(EID_MEWALKFRNT);
//		//TODO: add event
////		gameUIController.onMeCharacterReachFrontEventObjTrg = MECharaWalkFrontEvent.GetEventObjectTrigger ();
//		characterShowFinishEvent.AddChildEventObject (MECharaWalkFrontEvent);

		//displaying Explanation Dialog event
//		ChainEventObject showExplanationDlgEvent = new ChainEventObject(EID_EXPDLG);
//		showExplanationDlgEvent.onTrigger += () => {
//			//start 30 sec timer
//			GameProgressionManagerOutApp gameProgressionManager = GameObject.Find("GameProgressionManager").GetComponent<GameProgressionManagerOutApp>();
//			gameProgressionManager.StartTimer();
//		};
//		//TODO: add event
////		gameUIController.onExplanationDlgDoneEventObjTrg = showExplanationDlgEvent.GetEventObjectTrigger ();
//		MECharaWalkFrontEvent.AddChildEventObject (showExplanationDlgEvent);

		//*** Free Interaction Start ***

//		ChainEventObject playStartEndEvent = showExplanationDlgEvent;
//
//		ChainEventObject firstSonHandEvent = new ChainEventObject(EID_FIRSTSONHAND);
//		ChainEventObject firstSonMEEvent   = new ChainEventObject(EID_FIRSTSONME);
//
//		ChainEventObject secondSonHandEvent = new ChainEventObject(EID_SECONDSONHAND);
//		ChainEventObject secondSonMEEvent   = new ChainEventObject(EID_SECONDSONME);
//
//		ChainEventObject thirdSonHandEvent = new ChainEventObject(EID_THIRDSONHAND);
//		ChainEventObject thirdSonMEEvent   = new ChainEventObject(EID_THIRDSONME);
//
//		playStartEndEvent.AddChildEventObject (firstSonHandEvent);
//		playStartEndEvent.AddChildEventObject (firstSonMEEvent);
//		playStartEndEvent.AddChildEventObject (secondSonHandEvent);
//		playStartEndEvent.AddChildEventObject (secondSonMEEvent);
//		playStartEndEvent.AddChildEventObject (thirdSonHandEvent);
//		playStartEndEvent.AddChildEventObject (thirdSonMEEvent);
	}
}
