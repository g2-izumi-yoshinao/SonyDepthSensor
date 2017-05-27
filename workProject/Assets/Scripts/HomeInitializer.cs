using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeInitializer : MonoBehaviour {

	// Use this for initialization
	void Start () {

		//init view manager
		ViewManager viewManager = ViewManagerFinder.Find();
		viewManager.Init ();

		//start first view
		if (GlobalProperties.isAfterGame)
			viewManager.Push (viewManager.EndingView);
		else
//			viewManager.Push (viewManager.pictureCaptureView);
			viewManager.Push (viewManager.homeView);

		//this is all for now


		Destroy (this);
	}

}
