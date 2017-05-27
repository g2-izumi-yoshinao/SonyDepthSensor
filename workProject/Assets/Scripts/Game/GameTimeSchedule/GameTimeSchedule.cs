using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTimeSchedule : MonoBehaviour {

	public bool isTapeAutoDestroy = true;

	GameTimeScheduler gameTimeScheduler;
	Action onTapeFinishedHandler;

	public bool isPlaying{ get; private set;}
	float progressTime;

	public void Init()
	{
		gameTimeScheduler = null;
		onTapeFinishedHandler = null;
		isPlaying = false;
		progressTime = 0;
	}

	public void Update()
	{
		if (!isPlaying)
			return;

		if (gameTimeScheduler == null)
			return;
		
		//add progress time
		progressTime += Time.deltaTime;

		//check event
		TimeEventObject foundEvent = gameTimeScheduler.GetEventAtTime (progressTime);
		if (foundEvent != null) 
			foundEvent.Trigger ();

		//finish tape at tape's length
		if (progressTime >= gameTimeScheduler.length) 
		{
			//this will destroy tape
			if (isTapeAutoDestroy)
				End ();

			//invoke finished event
			if (onTapeFinishedHandler != null) 
			{	
				onTapeFinishedHandler ();
				onTapeFinishedHandler = null;
			}
			//reset parameter
			progressTime = 0;
			isPlaying = false;
		}
	}

	public void SetScheduler (GameTimeScheduler gameTimeScheduleTape)
	{
		this.gameTimeScheduler = gameTimeScheduleTape;
	}

	public void Play (Action onTapeFinished = null)
	{
		if (gameTimeScheduler == null) 
		{
			Debug.LogError ("Error : GameTimeScheduleTape is null. please set before you hit start");
			return;
		}

		//set on Tape Finished event
		onTapeFinishedHandler = onTapeFinished;
			
		progressTime = 0;

		isPlaying = true;
	}

	public void End ()
	{
		if (gameTimeScheduler == null)
			return; 

		isPlaying = false;

		gameTimeScheduler.Destroy ();
		gameTimeScheduler = null;
	}

	public void Destroy ()
	{
		//this will release gameTimeScheduleTape
		End (); 

		GameObject.Destroy (this);
	}

	public float GetTapeLength()
	{
		if (gameTimeScheduler == null)
			return -1;
		
		return gameTimeScheduler.length;
	}

	public float GetRemainTime()
	{
		if (gameTimeScheduler == null)
			return -1;
		
		return gameTimeScheduler.length - progressTime;
	}

}
