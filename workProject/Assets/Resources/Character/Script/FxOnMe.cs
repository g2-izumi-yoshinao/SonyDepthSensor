//
//  ReactionCharacterController
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxOnMe : MonoBehaviour {

	public GameObject Effect1;
	private GameObject footing = null;

	public AudioClip audio;
	private AudioSource sound;

	void Start () {
		sound = GetComponent<AudioSource> ();

		if (Effect1 != null) {
			Effect1.SetActive(false);
		}
	}

	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {

		if (other.gameObject.tag==ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			footing = other.gameObject;
			if (Effect1 != null) {
				Effect1.SetActive (false);
				Effect1.SetActive (true);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag==ReactionCharacterController.REACTINO_CHARACTER_TAG){
			footing = null;
			Effect1.SetActive (false);
		}
	}
}
