
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class will handle Game Progression
public class GameProgressionManagerOutApp : GameProgressionManager {

	//controls scene events
	GameEventSystem gameEventSystem; 

	//controls time progress
	GameTimeSchedule gameTimeSchedule;

	GameViewControllerOutApp gameViewControllerOut;


	//initialization
	public void Init () 
	{
		gameTimeSchedule = new GameObject ("GameTimeSchedule").AddComponent<GameTimeSchedule> ();
		gameTimeSchedule.Init ();

		gameViewControllerOut = GameObject.Find ("Canvas").GetComponent<GameViewControllerOutApp> ();
	}

	#region Interface implements

	public override void StartGame ()
	{
		StartCoroutine (EStartGame ());
	}

	public override void FinishGame()
	{
		GlobalProperties.isAfterGame = true;

		this.Destroy ();
		UnityEngine.SceneManagement.SceneManager.LoadScene (Constants.HOME_SCENE_NAME);
	}

	public void Destroy()
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

	#endregion

	IEnumerator EStartGame()
	{
		//first UI
		gameViewControllerOut.ShowDialog (WORDS.OUT_GAME_UI_01, 1.0f);

		//first navigation part
		gameEventSystem = new GameEventSystem ( new GameEventMapperOutApp() );
		gameEventSystem.StartEventSystem ();
		while (!gameEventSystem.isEventFinished) yield return null;
		gameEventSystem.Destroy ();

		Debug.Log ("UI navigation finished, Start Game");

		//demo game part
		StartTimer();

		//first UI for gamepart
		gameViewControllerOut.ShowDialog (WORDS.OUT_GAME_UI_03);
		gameViewControllerOut.StartTimer ( () => gameTimeSchedule == null ? -1 : (int)gameTimeSchedule.GetRemainTime()-2 );

		while (gameTimeSchedule.isPlaying) yield return null;

		Debug.Log ("game finished go back to UI scene");

		FinishGame ();

	}


	#region Game

	//scheduler property
	const string INTERACTION_SCH_NAME = "interaction";
	const float  INTERACTION_SCH_LENGTH = 32;
	const float  TIMER_LENGTH = 30;

	//just a name of evnet
	const string INTERACTION_TIME_EVENT_A = "GetReadyForMovie";
	const string INTERACTION_TIME_EVENT_B = "MovieStart";
	const string INTERACTION_TIME_EVENT_C = "GetReadyForPicture";
	const string INTERACTION_TIME_EVENT_D = "EndGamePlay";


	//start 30 second timer
	public void StartTimer()
	{
		//create scheduler
		GameTimeScheduler interactionScheduler = new GameTimeScheduler(INTERACTION_SCH_NAME, INTERACTION_SCH_LENGTH);

		//add events
		TimeEventObject first = new TimeEventObject(INTERACTION_TIME_EVENT_A);
		first.time = 18;
		first.onTrigger += ()=>{
			gameViewControllerOut.ShowDialog (WORDS.OUT_GAME_UI_04, 2.0f);
		};

		TimeEventObject second = new TimeEventObject(INTERACTION_TIME_EVENT_B);
		second.time = 20;
		second.onTrigger += ()=>{
			gameViewControllerOut.SetRecordingUIActive (true);
		};

		TimeEventObject third = new TimeEventObject(INTERACTION_TIME_EVENT_C);
		third.time = 27;
		third.onTrigger += ()=>{
			gameViewControllerOut.ShowDialog (WORDS.OUT_GAME_UI_05);
		};

		TimeEventObject fourth = new TimeEventObject(INTERACTION_TIME_EVENT_D);
		third.time = 30f;
		third.onTrigger += ()=>{
			gameViewControllerOut.ShowDialog (WORDS.OUT_GAME_UI_06);
			gameViewControllerOut.SetRecordingUIActive (false);
			gameViewControllerOut.StopTimer();
		};

		interactionScheduler.AddEvent (first);
		interactionScheduler.AddEvent (second);
		interactionScheduler.AddEvent (third);
		interactionScheduler.AddEvent (fourth);

		//set tape
		gameTimeSchedule.SetScheduler (interactionScheduler);

		//start timer
		gameTimeSchedule.Play ();

	}

	#endregion

	public List<ChainEventObject> GetActiveEventObjects()
	{
		return gameEventSystem.GetCurrentActiveEvents ();
	}
}

