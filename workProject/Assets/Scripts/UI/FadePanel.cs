using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour {

	float progressTime = 0;
	float duration = 0;
	float subAlpha = 0;

	Image image;
	Action onFinish = null;

	enum FadeState{
		FADEIN, FADEOUT, NONE, FADE
	}

	FadeState fadeState = FadeState.NONE;

	// Use this for initialization
	public void Init () 
	{
		image       = GetComponent<Image> ();
		image.color = new Color(0,0,0,0);
		fadeState   = FadeState.NONE;
		onFinish    = null;
	}

	void Update()
	{
		if (fadeState == FadeState.NONE)
			return;
		
		progressTime += Time.deltaTime;

		float rate = progressTime / duration;

		//set alpha
		float deltaAlpha;
		if(fadeState == FadeState.FADEOUT)
			deltaAlpha = subAlpha * rate;
		else
			deltaAlpha = 1.0f - (subAlpha * rate);
		
		image.color = new Color (0, 0, 0, deltaAlpha);

		if (progressTime >= duration) 
		{
			if (onFinish != null)
				onFinish ();

			fadeState = FadeState.NONE;
			progressTime = 0;

			this.gameObject.SetActive (false);
		}
	}

	public void FadeIn (float duration, Action onFinish = null)
	{
		this.gameObject.SetActive (true);

		image.color = new Color(0,0,0,1);
		this.onFinish = onFinish;
		this.duration = duration;
		subAlpha = image.color.a;
		fadeState     = FadeState.FADEIN;
	}

	public void FadeOut (float duration, Action onFinish = null)
	{
		this.gameObject.SetActive (true);

		image.color = new Color(0,0,0,0f);
		this.onFinish = onFinish;

		this.duration = duration;
		subAlpha = 1.0f - image.color.a;
		fadeState     = FadeState.FADEOUT;
	}

	public void Destroy()
	{
		onFinish = null;
	}
}
