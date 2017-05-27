using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PictureConfirmViewController : ViewController {

	GameObject MeChara;

	#region Button Interaction

	public void OnOKButtonClicked () 
	{
		Fade.FadeOut (0.5f, () => {
			//load game level
			SceneManager.LoadScene(Constants.GAME_SCENE_NAME);

		});
	}

	public void OnRecaptureButtonClicked () 
	{
		ViewManagerFinder.Find().Pop ();
	}

	#endregion

	#region View transition events

	public override void MoveIn()
	{
		base.MoveIn ();

		//Load Me Character
		CharacterLoader characterLoader = new CharacterLoader();
		MeChara = characterLoader.LoadME ();
		MeChara.GetComponent<Rigidbody> ().useGravity = false;

		Vector3 posOffset = new Vector3 (0, -3, 5);
		MeChara.transform.localPosition = Camera.main.transform.localPosition + posOffset;
		MeChara.transform.localScale    = new Vector3 (100,100,100);
		MeChara.transform.eulerAngles   = new Vector3 (0,  180,  0);
	}

	//triggered when view is popped and will be destroyed
	public override void Remove()
	{
		base.Remove ();

		if (MeChara != null)
			Destroy (MeChara);
	}

	#endregion 

}
