using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ViewManagerFinder {

	static ViewManager viewManager;

	public static ViewManager Find()
	{
		if (viewManager == null)
			viewManager = GameObject.Find ("Canvas").GetComponent<ViewManager> ();

		return viewManager;
	}

}
