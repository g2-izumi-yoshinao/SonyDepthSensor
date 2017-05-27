using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Fade {

	static FadePanel fadePanel;

	public static void FadeIn (float duration, Action onFinish = null)
	{
		if (fadePanel == null)
			LoadFadePanel ();

		fadePanel.FadeIn (duration, onFinish);
	}

	public static void FadeOut (float duration, Action onFinish = null)
	{
		if (fadePanel == null)
			LoadFadePanel ();

		fadePanel.FadeOut (duration, onFinish);
	}

	static void LoadFadePanel ()
	{
		//find "Panel_Fade" in scene if exist
		GameObject fadePanelObj = GameObject.Find("Panel_Fade");
		if (fadePanelObj != null) 
		{
			fadePanel = fadePanelObj.GetComponent<FadePanel> ();
			return;
		}
		//if no fade panel in scene load
		fadePanelObj = GameObjectResourceLoader.Load ("prefab/UI/Panel_Fade", Vector3.zero, 1, Quaternion.identity, GameObject.Find ("Canvas").transform);
		fadePanel = fadePanelObj.GetComponent<FadePanel> ();
		fadePanel.Init ();

		//set rect transform
		RectTransform rectTr = fadePanel.GetComponent<RectTransform>();
		rectTr.anchoredPosition = new Vector2 (0, 0);
	}

}
