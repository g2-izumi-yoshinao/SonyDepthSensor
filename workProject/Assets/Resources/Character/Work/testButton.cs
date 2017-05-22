using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testButton : MonoBehaviour {
	
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
	}
}
