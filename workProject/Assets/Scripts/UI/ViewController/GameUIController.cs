using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Currently this class is for debug
public class GameUIController : MonoBehaviour {

	public GameObject updateButtonButton;
	public GameObject timeLimit;

	public EventObjectTrigger onHandOpenClickEventObjTrg;
	public EventObjectTrigger onMeCharacterReachFrontEventObjTrg;
	public EventObjectTrigger onExplanationDlgDoneEventObjTrg;

	List<GameObject> buttons = null;

	void DestroyButtons()
	{
		foreach (GameObject btn in buttons) 
		{
			GameObject.Destroy (btn);
		}
		buttons = null;
	}

	public void UpdateButtons()
	{
		buttons = new List<GameObject> ();

		//get GameProgressionManager reference
		List<ChainEventObject> eventObjectList;
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name == Constants.GAME_SCENE_NAME_IN) {
			GameProgressionManagerInApp gameProgressionManager = GameObject.Find ("GameProgressionManager").GetComponent<GameProgressionManagerInApp> ();
			eventObjectList = gameProgressionManager.GetActiveEventObjects ();
		} else {
			GameProgressionManagerOutApp gameProgressionManager = GameObject.Find ("GameProgressionManager").GetComponent<GameProgressionManagerOutApp> ();
			eventObjectList = gameProgressionManager.GetActiveEventObjects ();
		}


		//no events in scene
		if (eventObjectList == null || eventObjectList.Count == 0)
			return;

		//load button prefab
		GameObject buttonRc = Resources.Load<GameObject>("prefab/UI/Button");

		//get position for button
		RectTransform baseRectTr = updateButtonButton.GetComponent<RectTransform>();

		float y = baseRectTr.anchoredPosition.y, x = baseRectTr.anchoredPosition.x;
		float offsetY = -30;

		//initial offset
		y += offsetY;

		//create buttons for each event in scene
		foreach (EventObject events in eventObjectList) 
		{
			GameObject button = GameObject.Instantiate (buttonRc);

			//set transform
			button.transform.parent = this.transform;
			RectTransform rectTr = button.GetComponent<RectTransform> ();
			rectTr.anchoredPosition = new Vector2 (x, y);

			//set button text
			button.transform.Find("Text").GetComponent<Text>().text = events.eventName;

			y += offsetY;

			//add on click event
			button.GetComponent<Button> ().onClick.AddListener (() => {
				events.Trigger ();

				//update Event Buttons after triggering event
				UpdateButtons();
			});

			//add gameobject reference to list
			buttons.Add (button);
		}
	}

	public void CreateTimer (Func<int> displayTime)
	{
		//turn on text
		timeLimit.SetActive (true);

		//set timer function
		GameUITimer gameUITimer = timeLimit.GetComponent<GameUITimer> ();
		gameUITimer.enabled     = true;
		gameUITimer.displayTime = displayTime;
	}

	public void OnHandOpenClick ()
	{
		//ME character appears
		if (onHandOpenClickEventObjTrg != null)
			onHandOpenClickEventObjTrg.Trigger ();
	}

	public void OnMeCharacterReachFront ()
	{
		//Show UI
		if (onMeCharacterReachFrontEventObjTrg != null)
			onMeCharacterReachFrontEventObjTrg.Trigger ();
	}

	public void OnExplanationDlgDone ()
	{
		//Show UI
		if (onExplanationDlgDoneEventObjTrg != null)
			onExplanationDlgDoneEventObjTrg.Trigger ();
	}

}
