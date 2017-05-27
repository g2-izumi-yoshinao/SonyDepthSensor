using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

	public const string GAME_SCENE_NAME = "OutScene";
	public const string GAME_SCENE_NAME_IN = "InScene";
	public const string HOME_SCENE_NAME   = "HomeScene";

	//camera capture settings
	public static readonly int[] CAM_CAP_CENTER = new int[2]{0,0}; //how many pixels away from center (right and up is +, left and down is -)
	public const int   CAM_CAP_WIDTH  = 500; //basically, width and height should be the same value
	public const int   CAM_CAP_HEIGHT = 500; //basically, width and height should be the same value
}
