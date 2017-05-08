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

	void Start () {
	}

	void Update () {
	}

	void OnTriggerEnter(Collider other) {
		if (!grapp) {
			if (other.gameObject.tag.Substring (0, ReactionCharacterController.PINCH_POINT_PREFIX.Length) ==
			    ReactionCharacterController.PINCH_POINT_PREFIX) {
				lastTargetName = other.tag;
				pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();
			}
		} 
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag.Substring (0, ReactionCharacterController.PINCH_POINT_PREFIX.Length) ==
			ReactionCharacterController.PINCH_POINT_PREFIX) {
			if (other.tag == lastTargetName) {
				if ((pinchingCharacter.getPinchState () == ReactionCharacterController.PinchState.cheek_cheek) 
					||pinchingCharacter.getOnCheekShurink()){
				} else {
					pinchingCharacter.pinchChanged (lastTargetName, false, null);
					lastTargetName = "";
					pinchingCharacter = null;
				}
			}
		}
	}

	public void onFingerGrapStateChangedDelegate(bool grap){
		grapp = grap;
		if (pinchingCharacter != null) {
			pinchingCharacter.pinchChanged (lastTargetName, grapp, this.transform);
		}
	}

	public void forceRemove(){
		pinchingCharacter.pinchChanged (lastTargetName, false, null);
		lastTargetName = "";
		pinchingCharacter = null;
	}
}
