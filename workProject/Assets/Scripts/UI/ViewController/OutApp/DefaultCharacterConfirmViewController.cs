using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefaultCharacterConfirmViewController : ViewController {

	public void OnOKButtonClicked () 
	{
		Fade.FadeOut (0.5f, () => {
			//load game level
			SceneManager.LoadScene(Constants.GAME_SCENE_NAME);

		});
	}

	public void OnReselectButtonClicked () 
	{
		ViewManagerFinder.Find().Pop ();
	}
}
