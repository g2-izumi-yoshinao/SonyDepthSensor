using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InCharacterConfirmViewController : ViewController {

	public void OnStartButtonClick()
	{
		Fade.FadeOut (0.5f, () => {
			//load game level
			SceneManager.LoadScene(Constants.GAME_SCENE_NAME_IN);

		});
	}

	public void OnBackButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InCharacterSelectView);
	}

}
