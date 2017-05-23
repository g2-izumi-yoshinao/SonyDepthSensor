using System.Collections;
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

	public void ClickBtn () {
		GameObject[] sons = GameObject.FindGameObjectsWithTag ("son");
		foreach (GameObject s in sons) {
			SimpleCharacterController sp = s.GetComponentInChildren<SimpleCharacterController> (true);
			sp.testSetProximity ();
		}

//		GameObject cakePirce = Instantiate (cakePiecePref, new Vector3(0,0,0), Quaternion.identity);
//		Rigidbody cakeRig = cakePirce.GetComponent<Rigidbody> ();
//		cakeRig.AddForce (new Vector3(0.0f,1.1f,0.1f), ForceMode.Impulse);
//
	}

	public void ClickBtn2 () {
		GameObject[] sons = GameObject.FindGameObjectsWithTag ("son");
		foreach (GameObject s in sons) {
			SimpleCharacterController sp = s.GetComponentInChildren<SimpleCharacterController> (true);
			sp.onPoint ();
		}
		GameObject kuma = GameObject.FindGameObjectWithTag ("kumamon");
		KumamonController kmc = kuma.GetComponentInChildren<KumamonController> (true);
		kmc.onPoint ();
	}

	public void ClickBtn3 () {
		CapController cap= capPref.GetComponentInChildren<CapController> ();
		cap.showKumamon ();

	}
}
