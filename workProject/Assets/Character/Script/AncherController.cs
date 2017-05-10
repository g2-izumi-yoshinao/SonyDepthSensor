//
//  AncherController
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AncherController : MonoBehaviour {

	public delegate void onGrapStateChangedDelegate(bool grap);
	private bool grapp = false;
	public onGrapStateChangedDelegate onGrapStateChanged;
	private string lastTargetName;
	private ReactionCharacterController pinchingCharacter;

	public bool onAction=true;

	void Start () {
	}

	void Update () {
		if (!onAction) {
			return;
		}

	}

	public void clear(){
		onAction = false;
		pinchingCharacter = null;
	}


	void OnTriggerEnter(Collider other) {
		if (!onAction) {
			return;
		}
		if (!grapp) {
			if (other.gameObject.tag.Substring (0, ReactionCharacterController.PINCH_POINT_PREFIX.Length) ==
			    ReactionCharacterController.PINCH_POINT_PREFIX) {
				lastTargetName = other.tag;
				pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();
			}
		} 
		if (other.gameObject.tag == SimpleCharacterController.POINT_POINT_TAG) {
			SimpleCharacterController simpleCharacter= other.gameObject.GetComponentInParent<SimpleCharacterController> ();
			simpleCharacter.onPoint ();
		}
	}

	void OnTriggerExit(Collider other) {
		if (!onAction) {
			return;
		}
		if (other.gameObject.tag.Substring (0, ReactionCharacterController.PINCH_POINT_PREFIX.Length) ==
			ReactionCharacterController.PINCH_POINT_PREFIX) {
			if (other.tag == lastTargetName) {
				if ((pinchingCharacter.getPinchState () == ReactionCharacterController.PinchState.cheek_cheek) 
					||pinchingCharacter.getOnCheekShurink()){
				} else {
					pinchingCharacter.pinchChanged (lastTargetName, false, this.transform);
					lastTargetName = "";
					pinchingCharacter = null;
				}
			}
		}
	}

	public void onFingerGrapStateChangedDelegate(bool grap){
		if (!onAction) {
			return;
		}
		grapp = grap;
		if (pinchingCharacter != null) {
			pinchingCharacter.pinchChanged (lastTargetName, grapp, this.transform);
		}
	}

	public void forceRemove(){
		if (!onAction) {
			return;
		}
		pinchingCharacter.pinchChanged (lastTargetName, false, null);
		lastTargetName = "";
		pinchingCharacter = null;
	}
}
