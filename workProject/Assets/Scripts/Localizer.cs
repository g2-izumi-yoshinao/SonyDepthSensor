using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localizer {

	public void LocalizeWords()
	{
		#if REGION_JP

		WORDS.OUT_GAME_UI_01 = Localization.JP_OUT_GAME_UI_01;
		WORDS.OUT_GAME_UI_02 = Localization.JP_OUT_GAME_UI_02;
		WORDS.OUT_GAME_UI_02 = Localization.JP_OUT_GAME_UI_02;

		#endif
	}
}
