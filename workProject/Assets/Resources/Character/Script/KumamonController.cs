//
//  KumamonController
//  Created by Yoshinao Izumi on 2017/05/23.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KumamonController : MonoBehaviour {

	public static string KUMAMON_TAG = "kumamon";

	public delegate void onPointNotifyDelegate(bool onPoint);
	public onPointNotifyDelegate onPointNotify;

	private float alphaVal=0.0f;
	public Material mat;
	private bool onFadeIn=false;

	private bool onPointState=false;
	private bool onPointStateInit=false;
	private float singLength;
	private Vector3 basePosition;
	private bool onDownUp=false;
	private float eula;
	private float perEula=20;
	private bool first=true;
	void Start () {
		
		setAlpha (0);
	}
	
	void Update () {
		if (first) {
			first = false;
			singLength = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y) / 4;
		}
		if (onFadeIn) {
			if (fadeIn ()) {
				return;
			} else {
				onFadeIn = false;
			}
		}
		if (onPointState) {
			if (onPointStateInit) {
				onPointStateInit = false;
				if (onPointNotify != null) {
					onPointNotify (true);
					basePosition = transform.position;
					eula = 0;
					onDownUp = true;
				}
			}
			if (onDownUp) {
				eula += perEula*Time.deltaTime*3;
				if (eula > 180) {
					onDownUp = false;
					onPointState = false;
					onPointNotify (false);

				} else {
					float ang = singLength * Mathf.Sin (eula * Mathf.PI / 180);
					transform.position = new Vector3 (
						basePosition.x,
						basePosition.y - ang,
						basePosition.z
					);
				}
			}
		}
	}

	public void doFadeIn(){
		onFadeIn = true;
	}

	private bool fadeIn(){
		bool onProcess = true;
		alphaVal += Time.deltaTime;
		if (alphaVal >= 1.0f) {
			alphaVal = 1.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private bool fadeOut(){
		bool onProcess = true;
		alphaVal -= Time.deltaTime;
		if (alphaVal < 0.0f) {
			alphaVal = 0.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private void setAlpha(float alpha){
		if (alpha == 1.0) {
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, alpha);
			CommonStatic.SetBlendMode (mat, CommonStatic.blendMode.Opaque);
		}else if (alpha == 0.0){
			CommonStatic.SetBlendMode (mat, CommonStatic.blendMode.Cutout);
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, alpha);
		}else{
			CommonStatic.SetBlendMode (mat, CommonStatic.blendMode.Transparent);
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, alpha);
		}
	}

	public void onPoint(){
		if (!onPointState) {
			onPointStateInit = true;
			onPointState = true;
		}
	}
}
