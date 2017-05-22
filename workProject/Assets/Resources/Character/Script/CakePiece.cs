//
//  CakePiece
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//at start has mass rigid and collider 
//on ground, swithc to trigger and gravity effect off

public class CakePiece : MonoBehaviour {

	void Start () {
	}
	void Update () {
		if (transform.position.y < -1.0f) {
			Destroy (gameObject);
		}
	}


}
