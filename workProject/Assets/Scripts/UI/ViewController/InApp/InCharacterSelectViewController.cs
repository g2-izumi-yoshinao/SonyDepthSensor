using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCharacterSelectViewController : ViewController {

	#region Button Interactions

	public void OnFirstCharacterButtonClick()
	{
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InCharacterConfirmView);
	}

	public void OnSecondCharacterButtonClick()
	{
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InCharacterConfirmView);
	}

	public void OnThirdCharacterButtonClick()
	{
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InCharacterConfirmView);
	}

	public void OnBackButtonClick()
	{
		//go to next view
		ViewManagerFinder.Find().Push ( ViewManagerFinder.Find().InExplanationSecondView);
	}

	#endregion

}
