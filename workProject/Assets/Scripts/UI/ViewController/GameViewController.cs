using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameViewController : MonoBehaviour {

	//dialogs
	public GameDialog gameDialog;

	//count down
	public GameUITimer gameUITimer;

	public GameObject recordingImg;

	// Use this for initialization
	void Start () 
	{
		gameDialog.Init ();
		gameUITimer.Init ();
	}

	#region Dialog
	//show dialog that dissapears by duration
	public void ShowDialog (string msg)
	{
		gameDialog.ShowMessage (msg);
	}

	//show dialog that dissapears by duration
	public void ShowDialog (string msg, float duration)
	{
		gameDialog.ShowMessage (msg, duration);
	}

	//clear dialog
	public void CloseDialog (string msg)
	{
		gameDialog.ClearMessage ();
	}
	#endregion


	//timer

	public void StartTimer (Func<int> displayTime)
	{
		gameUITimer.displayTime = displayTime;
		gameUITimer.SetActive (true);
	}

	public void StopTimer ()
	{
		gameUITimer.SetActive (false);
	}

	//others

	public void SetRecordingUIActive (bool isActive)
	{
		recordingImg.SetActive (isActive);
	}


}

