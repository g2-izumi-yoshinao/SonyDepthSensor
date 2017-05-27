using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectViewController : ViewController {

	// Use this for initialization
	public void OnTakePictureButtonClick () 
	{
		//go to picture capture view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().pictureCaptureView);

	}
	
	// Update is called once per frame
	public void OnUseDefaultCharacterClick ()
	{
		//go to default character confirm view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().defaultCharacterConfirmView);

	}
}
