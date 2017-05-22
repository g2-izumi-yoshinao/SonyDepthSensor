using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeWaste : MonoBehaviour {

	public Material pasaMat;
	private float alphaVal=0.0f;
	private bool onFadeIn=false;
	private bool onFadeOut=false;

	void Start () {
		
	}

	void Update () {
		if (onFadeIn) {
			if (!fadeIn ()) {
				onFadeIn = false;
			}
		}
		if (onFadeOut) {
			if (!fadeOut ()) {
				onFadeOut = false;
			}
		}
	}

	public void doFadeIn(){
		onFadeIn=true;
	}

	public void doFadeOut(){
		onFadeOut=false;
	}

	private bool fadeIn(){
		bool onProcess = true;
		alphaVal += Time.deltaTime/2;
		if (alphaVal >= 1.0f) {
			alphaVal = 1.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private bool fadeOut(){
		bool onProcess = true;
		alphaVal -= Time.deltaTime/2;
		if (alphaVal < 0.0f) {
			alphaVal = 0.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private void setAlpha(float alpha){

		if (alpha == 1.0) {
			pasaMat.color 
			= new Color (pasaMat.color.r, pasaMat.color.g, pasaMat.color.b, alpha);
			CommonStatic.SetBlendMode(pasaMat,CommonStatic.blendMode.Opaque);

		}else{
			CommonStatic.SetBlendMode(pasaMat,CommonStatic.blendMode.Transparent);
			pasaMat.color 
			= new Color (pasaMat.color.r, pasaMat.color.g, pasaMat.color.b, alpha);
		}
	}

	public void clean(){
		setAlpha (0);
	}
}
