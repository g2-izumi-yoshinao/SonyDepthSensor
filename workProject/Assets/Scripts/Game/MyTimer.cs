using UnityEngine;
using System.Collections;
using System;

public static class MyTimerFinder{

	public static MyTimer Find()
	{
		//create timer
		MyTimer timer = GameObject.Find ("Main Camera").GetComponent<MyTimer> ();
		if (timer == null) 
			timer = GameObject.Find ("Main Camera").AddComponent<MyTimer> ();

		return timer;
	}
}

public class MyTimer : MonoBehaviour
{
	public delegate void TimeUp (float numTime);
	public event Action<float> timer;

	public event TimeUp timeUp;

	static MyTimer _instance;

	// Update is called once per frame
	void Update ()
	{
		if (timeUp != null) 
			timeUp (Time.deltaTime);
		
		if (timer != null)
			timer (Time.time);
	}

	public void StartTimer(float time, Action timeUp)
	{
		float startTime = Time.time;
		Action<float> onTimeUp = null;
		onTimeUp = (currentTime) => {
			if (currentTime - startTime > time)
			{
				timer -= onTimeUp;
				timeUp ();
			}
		};

		timer += onTimeUp;
	}

}
