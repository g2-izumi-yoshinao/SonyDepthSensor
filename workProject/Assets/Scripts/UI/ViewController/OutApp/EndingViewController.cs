using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingViewController : ViewController {
	
	public void OnEndButtonClicked()
	{
		GlobalProperties.isAfterGame = false;

		Fade.FadeOut (0.5f, () => {
			//reload same scene to restart
			UnityEngine.SceneManagement.SceneManager.LoadScene (Constants.HOME_SCENE_NAME);
		});
	}
}
