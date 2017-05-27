using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Localization {

	/* ====== format description ==================
	 * 
	 *  [LANG]_[APP]_[SCENE]_[USE]_[ID]
	 * 
	 * LANG:  japanese(JP) or Chinese(CHN)
	 * APP:   incamera(IN) or outcamera(OUT)
	 * SCENE: UI scene(UI) or Game scene(GAME)
	 * USE:   used for UI(UI) or anyothers(OTHER)
	 * ID:    used for identification
	 * 
	 * ============================================
	 */

	#region Japanese

	public static string JP_OUT_GAME_UI_01 = "画面の中央い手を映し、\n拳を握ってから手のひらを上に向けて\n開いてください";
	public static string JP_OUT_GAME_UI_02 = "では、はじめましょう！";
	public static string JP_OUT_GAME_UI_03 = "キャラクターと遊べるよ（30秒）\n残り10秒から動画撮影あり";

	#endregion

	#region Chinese

	public static string CHN_OUT_GAME_UI_01 = "画面の中央い手を映し、\n拳を握ってから手のひらを上に向けて\n開いてください";
	public static string CHN_OUT_GAME_UI_02 = "では、はじめましょう！";
	public static string CHN_OUT_GAME_UI_03 = "キャラクターと遊べるよ（30秒）\n残り10秒から動画撮影あり";

	#endregion
}
