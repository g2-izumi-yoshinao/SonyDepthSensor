//
//  ReactionCharacterController
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonStatic : MonoBehaviour {
	public static string GROUND_TAG = "ground";
	public static string CAKE_TAG = "cake";


	public static Vector3[] getSightEdgePoint(GameObject aimTarget){

		Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		Vector2 cameraXY = new Vector2 (cameraPos.x,cameraPos.z);
		Vector2 aimTargetXY = new Vector2 (aimTarget.transform.position.x, aimTarget.transform.position.z);

		Vector3 CamAimVec = (aimTargetXY - cameraXY);
		float camAimLen = CamAimVec.magnitude;
		float aimRadius = aimTarget.transform.lossyScale.x / 2.0f;

		float cos_a = aimRadius / camAimLen;
		float sin_ql = -cos_a; //sin(π-θ)=-cos(θ)
		float cos_ql = Mathf.Sqrt (1 - sin_ql * sin_ql);
		float sin_qr = -sin_ql; //sin(-θ)=--sin(θ)
		float cos_qr = Mathf.Sqrt (1 - sin_qr * sin_qr);

		Vector2 cambaseVec = aimTargetXY - cameraXY;
		Vector3 leftBasePos = new Vector3 (cos_ql * cambaseVec.x - sin_ql * cambaseVec.y, 0,
			sin_ql * cambaseVec.x + cos_ql * cambaseVec.y);
		Vector3 rightBasePos = new Vector3 (cos_qr * cambaseVec.x - sin_qr * cambaseVec.y,0,
			sin_qr * cambaseVec.x + cos_qr * cambaseVec.y);

		Vector3 cameraX0Z = new Vector3 (cameraXY.x, 0, cameraXY.y);
		Vector3[] ret= new Vector3[2];
		ret[0]=leftBasePos + cameraX0Z;
		ret[1]= rightBasePos + cameraX0Z;
		return ret;
	}


}
