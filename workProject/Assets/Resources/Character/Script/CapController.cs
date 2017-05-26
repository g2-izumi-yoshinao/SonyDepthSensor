//
//  CapController
//  Created by Yoshinao Izumi on 2017/05/23.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapController : MonoBehaviour {

	public GameObject yugeEffect;
	public GameObject waterSurfaceEffect;
	public GameObject waterFalleEffect;
	public KumamonController kumamonController;

	private bool onWaterOverflowInit=false;
	private bool onWaterOverflow=false;

	private float singLength;
	private Vector3 basePosition;
	private bool onDownUp=false;
	private float eula;
	private float perEula=20;
	private bool first =true;

	void Start () {
		if (yugeEffect != null) {
			yugeEffect.SetActive (false);
		}
		if (waterSurfaceEffect != null) {
			waterSurfaceEffect.SetActive (false);
		}
		if (waterFalleEffect != null) {
			waterFalleEffect.SetActive (false);
		}
	}

	void Update () {

		if (first) {
			first = false;
			singLength = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y) / 6;
		}

		if (onWaterOverflow) {
			if (onWaterOverflowInit) {
				waterFalleEffect.SetActive (true);
				basePosition = waterSurfaceEffect.transform.position;
				eula = 0;
				onDownUp = true;
			}
			if (onDownUp) {
				eula += perEula*Time.deltaTime*3;
				if (eula > 180) {
					onDownUp = false;
				} else {
					float ang = singLength * Mathf.Sin (eula * Mathf.PI / 180);
					waterSurfaceEffect.transform.position = new Vector3 (
						basePosition.x,
						basePosition.y + ang,
						basePosition.z
					);
				}
			}

		} else {
			if (waterFalleEffect.activeSelf) {
				waterFalleEffect.SetActive (false);
				onWaterOverflow = false;
			}
		}
	}
		
	public void showKumamon(){
		kumamonController=GetComponentInChildren<KumamonController> (true);
		kumamonController.doFadeIn ();
		kumamonController.onPointNotify = onPointDelegate;
		yugeEffect.SetActive (true);
		waterSurfaceEffect.SetActive (true);
	}
		
	//kumamono pointed
	public void onPointDelegate(bool onPoint){
		if (onPoint) {
			onWaterOverflowInit = true;
			onWaterOverflow = true;
		} else {
			onWaterOverflow = false;
		}
	}

	public void extYugekStart(){
		yugeEffect.SetActive (true);
	}

	public void extYugeEnd(){
		yugeEffect.SetActive (false);
	}
}
