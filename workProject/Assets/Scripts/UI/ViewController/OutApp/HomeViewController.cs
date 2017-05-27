using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeViewController : ViewController {

	//triggered when moved in as new view
	public override void MoveIn()
	{
		this.gameObject.SetActive (true);

		Fade.FadeIn (0.3f);

	}

	public void OnNextButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().characterSelectView);
	}

}
