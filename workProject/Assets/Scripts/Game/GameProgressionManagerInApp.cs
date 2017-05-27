using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressionManagerInApp : GameProgressionManager {

	//controls scene events
	GameEventSystem gameEventSystem; 

	//controls time progress
	GameTimeSchedule gameTimeSchedule;

	//initialization
	public void Init () 
	{
		//TODO: this object will stay active for the all game, but it is unnecessary
		gameTimeSchedule = new GameObject ("GameTimeSchedule").AddComponent<GameTimeSchedule> ();
		gameTimeSchedule.Init ();
	}

	public override void StartGame ()
	{
		StartCoroutine (EStartGame ());
	}

	public override void FinishGame ()
	{
		GlobalProperties.isAfterGame = true;

		this.Destroy ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (Constants.HOME_SCENE_NAME);
	}

	public override void Destroy ()
	{
		if (gameEventSystem != null) 
		{
			gameEventSystem.Destroy ();
			gameEventSystem = null;
		}

		//this is monobehaviour
		if (gameTimeSchedule != null) 
		{
			gameTimeSchedule.Destroy ();
			gameTimeSchedule = null;
		}
	}

	//game progression
	IEnumerator EStartGame()
	{
		//first navigation part
		gameEventSystem    = new GameEventSystem ( new GameEventMapperInAppFirst() );
		gameEventSystem.StartEventSystem ();
		while (!gameEventSystem.isEventFinished) yield return null;
		gameEventSystem.Destroy ();

		Debug.Log ("first part done, start DEMO");
		//demo game part
		StartDemoGame();
		while (!gameTimeSchedule.isPlaying) yield return null;

		Debug.Log ("DEMO done, start second part");
		//interval part
		gameEventSystem    = new GameEventSystem ( new GameEventMapperInAppSecond() );
		gameEventSystem.StartEventSystem ();
		while (!gameEventSystem.isEventFinished) yield return null;
		gameEventSystem.Destroy ();

		Debug.Log ("start real game");

		//real game part
		StartRealGame();
		while (!gameTimeSchedule.isPlaying) yield return null;

		//end of game

	}


	const string INTERACTION_SCH_NAME = "interaction";
	const float  INTERACTION_SCH_LENGTH = 3;

	const string INTERACTION_TIME_EVENT_A = "timeEventA";
	const string INTERACTION_TIME_EVENT_B = "timeEventB";
	const string INTERACTION_TIME_EVENT_C = "timeEventC";

	//start demo game
	void StartDemoGame()
	{
		//create tape
		GameTimeScheduler interactionScheduler = new GameTimeScheduler(INTERACTION_SCH_NAME, INTERACTION_SCH_LENGTH);

		//add events
		TimeEventObject firstTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_A);
		firstTimeEventObject.time = 2;
		interactionScheduler.AddEvent (firstTimeEventObject);

		TimeEventObject secondTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_B);
		secondTimeEventObject.time = 5;
		interactionScheduler.AddEvent (secondTimeEventObject);

		TimeEventObject thirdTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_C);
		thirdTimeEventObject.time = 10;
		interactionScheduler.AddEvent (thirdTimeEventObject);

		//set tape
		gameTimeSchedule.SetScheduler (interactionScheduler);

		//start timer
		gameTimeSchedule.Play ();

		//Create Timer UI
		GameViewControllerInApp gameUIController = GameObject.Find("Canvas").GetComponent<GameViewControllerInApp> ();
		gameUIController.StartTimer ( () => gameTimeSchedule == null ? -1 : (int)gameTimeSchedule.GetRemainTime() );

	}

	void StartRealGame()
	{
		//create tape
		GameTimeScheduler interactionScheduler = new GameTimeScheduler(INTERACTION_SCH_NAME, INTERACTION_SCH_LENGTH);

		//add events
		TimeEventObject firstTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_A);
		firstTimeEventObject.time = 2;
		interactionScheduler.AddEvent (firstTimeEventObject);

		TimeEventObject secondTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_B);
		secondTimeEventObject.time = 5;
		interactionScheduler.AddEvent (secondTimeEventObject);

		TimeEventObject thirdTimeEventObject = new TimeEventObject(INTERACTION_TIME_EVENT_C);
		thirdTimeEventObject.time = 10;
		interactionScheduler.AddEvent (thirdTimeEventObject);

		//set tape
		gameTimeSchedule.SetScheduler (interactionScheduler);

		//start timer
		gameTimeSchedule.Play ();

		//Create Timer UI
		GameUIController gameUIController = GameObject.Find("Canvas").GetComponent<GameUIController> ();
		gameUIController.CreateTimer ( () => gameTimeSchedule == null ? -1 : (int)gameTimeSchedule.GetRemainTime() );

	}

	public List<ChainEventObject> GetActiveEventObjects()
	{
		return gameEventSystem.GetCurrentActiveEvents ();
	}


}
