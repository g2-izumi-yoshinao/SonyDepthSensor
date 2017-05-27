using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUITimer : MonoBehaviour {

	public Func<int> displayTime;

	Text text;

	// Use this for initialization
	public void Init () {
		text = transform.Find("text").GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () {
		if (displayTime != null)
			text.text = displayTime().ToString();
	}

	public void SetActive (bool isActive)
	{
		gameObject.SetActive (isActive);
	}

//	// Use this for initialization
//	void Start () {
//		text = GetComponent<Text> ();
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		if (displayTime != null)
//			text.text = displayTime().ToString();
//	}
}
