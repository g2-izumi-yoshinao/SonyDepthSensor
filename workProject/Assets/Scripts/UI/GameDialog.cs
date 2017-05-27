using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDialog : MonoBehaviour{

	Text dialogText;

	public void Init()
	{
		dialogText = transform.Find("text").GetComponent<Text> ();
	}

	#region Show Message options

	//we might need to add more options about dialog, for example, aligment... number of lines, font size...
	//create option object if that happens

	//keep showing
	public void ShowMessage(string msg)
	{
		if (!gameObject.activeSelf)
			SetActive (true);

		dialogText.text = msg;
	}

	//by time
	public void ShowMessage(string msg, float duration)
	{
		if (!gameObject.activeSelf)
			SetActive (true);

		dialogText.text = msg;

		//disable dialog after specified duration
		MyTimerFinder.Find ().StartTimer (duration, () => {
			dialogText.text = string.Empty;
			SetActive (false);
		});
	}

	#endregion

	public void ClearMessage()
	{
		dialogText.text = "";
	}

	public void SetActive (bool isActive)
	{
		gameObject.SetActive (isActive);
	}

	public void Destroy()
	{
		GameObject.Destroy (this.gameObject);
	}

}
