using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footPrint : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == ReactionCharacterController.REACTINO_CHARACTER_TAG) {
			Debug.Log ("onfoot");
		}
	}

	void OnTriggerExit(Collider other) {

	}
}
