﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testButton : MonoBehaviour {

	public GameObject cakePiecePref;
	public GameObject capPref;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	bool bt1=false;
	public void ClickBtn () {
		if (!bt1) {
			bt1 = true;
			GameObject[] sons = GameObject.FindGameObjectsWithTag ("son");
			foreach (GameObject s in sons) {
				SimpleController sp = s.GetComponentInChildren<SimpleController> (true);
				sp.testSetProximity ();
			}
			bt1 = false;
		}

//		GameObject cakePirce = Instantiate (cakePiecePref, new Vector3(0,0,0), Quaternion.identity);
//		Rigidbody cakeRig = cakePirce.GetComponent<Rigidbody> ();
//		cakeRig.AddForce (new Vector3(0.0f,1.1f,0.1f), ForceMode.Impulse);
//
	}
	bool bt2=false;
	public void ClickBtn2 () {
		if (!bt2) {
			bt2 = true;
			GameObject[] sons = GameObject.FindGameObjectsWithTag ("son");
			foreach (GameObject s in sons) {
				SimpleController sp = s.GetComponentInChildren<SimpleController> (true);
				sp.onPoint ();
			}
			GameObject kuma = GameObject.FindGameObjectWithTag ("kumamon");
			KumamonController kmc = kuma.GetComponentInChildren<KumamonController> (true);
			kmc.onPoint ();
			bt2 = false;
		}
	}

	bool bt3=false;
	public void ClickBtn3 () {
		if (!bt3) {
			bt3 = true;
			CapController cap = capPref.GetComponentInChildren<CapController> ();
			cap.showKumamon ();
			bt3 = false;
		}
	}

	bool bt4=false;
	public void ClickBtn4 () {
		if (!bt4) {
			bt4 = true;
			AutoWalkMeController rc
			= GameObject.FindGameObjectWithTag ("me").GetComponentInChildren<AutoWalkMeController> ();;
			rc.extFootPrint ();
			bt4 = false;
		}

	}

	bool bt5=false;
	public void ClickBtn5 () {
		if (!bt5) {
			bt5 = true;
			GameObject loaderObj = GameObject.FindGameObjectWithTag ("outSeenLoader");
			LoaderOutScene loader = loaderObj.GetComponentInChildren<LoaderOutScene> (true);

			loader.startScene(new Vector3(0f,0f,0.6f));
			bt5 = false;
		}

	}

	bool bt6=false;
	public void ClickBtn6 () {
		if (!bt6) {
			bt6 = true;
			GameObject loaderObj = GameObject.FindGameObjectWithTag ("outSeenLoader");
			LoaderOutScene loader = loaderObj.GetComponentInChildren<LoaderOutScene> (true);
			loader.startMeWalking ();
			bt6 = false;
		}

	}
}
