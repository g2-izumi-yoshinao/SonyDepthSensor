using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InStartViewController : ViewController {

	public void OnStartButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InExplanationFirstView);
	}
}
