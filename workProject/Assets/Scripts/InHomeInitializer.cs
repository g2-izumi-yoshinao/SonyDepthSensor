using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InHomeInitializer : MonoBehaviour {

	void Start () {

		//init view manager
		ViewManager viewManager = ViewManagerFinder.Find();
		viewManager.Init ();

		//start first view
		if (GlobalProperties.isAfterGame)
			viewManager.Push (viewManager.EndingView);
		else
			viewManager.Push (viewManager.InStartView);

		//this is all for now


		Destroy (this);
	}
}
