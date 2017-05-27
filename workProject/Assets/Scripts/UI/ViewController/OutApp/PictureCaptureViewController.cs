using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureCaptureViewController : ViewController {

	//camera related
	public Texture2D cameraAlphaMask;
	public RawImage rawimage; //texture will be copied to this rawimage
	public GameUITimer gameUITimer;

	//captured texture
	Texture2D resultTexture;

	const float WAIT_TIME = 1;
	float processTime = 0;

	bool isTimerActive = false;

	// Use this for initialization
	IEnumerator StartCapture ()
	{
		Debug.Log ("Start ");
		yield return new WaitForSeconds (1.0f);

		//start timer 
		SetUpTimer();

		//start camera
		InitCamera();

		//wait for time up
		isTimerActive = true;
		while (isTimerActive)
			yield return null;

		DisableTimer ();

		//take Picture here
		TakePicture();

		//stop camera
		DisableCamera();

		yield return new WaitForSeconds (1.0f);

		//finish timer and go to picture confirm view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().pictureConfirmView);

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!isTimerActive)
			return;
		
		processTime += Time.deltaTime;
		int leftTime = Mathf.CeilToInt (WAIT_TIME - processTime);
		gameUITimer.displayTime = () => { return leftTime;};

		//deaticavte timer if time is over
		if (leftTime <= 0)
			isTimerActive = false;
	}

	void SetUpTimer()
	{
		//start timer 
		gameUITimer.SetActive (true);
		gameUITimer.Init ();
		processTime = 0;
	}

	void DisableTimer()
	{
		//start timer 
		gameUITimer.SetActive (false);

		processTime = 0;
	}

	#region Camera

	FaceCapture faceCapture;

	void InitCamera()
	{
		faceCapture = gameObject.GetComponent<FaceCapture> ();
		if (faceCapture == null)
			faceCapture = gameObject.AddComponent<FaceCapture> ();

		//set mask texture
		faceCapture.alphaMask = cameraAlphaMask;
		faceCapture.rawimage = rawimage;

		//start camera
		faceCapture.Init ();

	}

	void TakePicture()
	{
		if (faceCapture == null)
			InitCamera ();

		//capture current texture
		resultTexture = faceCapture.Capture ();
	}

	void DisableCamera()
	{
		if (faceCapture != null)
		{
			faceCapture.Destroy ();
			Destroy (faceCapture);
		}

	}

	#endregion

	#region View transition events

	//triggered when drawn over by next view 
	public override void MoveOut()
	{
		base.MoveOut ();
	}

	//triggered when view is popped and will be destroyed
	public override void Remove()
	{
		base.Remove ();
	}

	//triggered when moved in as new view
	public override void MoveIn()
	{
		base.MoveIn ();

		//start countdown
		StartCoroutine( StartCapture ());
	}

	//triggered when previous view is popped and move back in as old view
	public override void MoveBackIn()
	{
		base.MoveBackIn ();

		//start countdown
		StartCoroutine( StartCapture ());
	}

	#endregion
}
