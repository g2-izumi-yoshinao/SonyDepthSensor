//
//  CommonStatic
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonStatic : MonoBehaviour {
	public static string GROUND_TAG = "ground";
	public static string CAKE_TAG = "cake";
	public static string CAP_TAG = "cap";
	public static string CAP_SURFACE_TAG = "capWaterSurface";
	public static string SON_TAG = "son";
	public static string PASAPASA_TAG = "pasapasa";

	//charactre poly to unity 1 float scale scae base
	public static float charaRateX=0.04f;//scale1 as unity1
	public static float charaRateY=0.05f;//scale1 as unity1
	public static float charaRateZ=0.04f;//scale1 as unity1

//	//cake poly to unity float scale scae base
//	public static double cakeRateXZ=0.1f;//scale1 as unity1
//	public static double cakeRateY=0.03f;//scale1 as unity1
//	//cap poly to unity float scale scae base
//	public static double capRateXZ=0.13f;//scale1 as unity1
//	public static double capRateY=0.05f;
//	//ground poly to unity float scale scae base
//	public static double groundRateXZ=1;//scale1 as unity1
//	public static double groundRateY=1;//scale1 as unity1

	//display scale
	public static Vector3 outCamScaleCharacter =  new Vector3(1.5f,1.5f,1.6f);
	public static Vector3 outCamScaleCap =  new Vector3(0.5f,0.71f,0.5f);
	public static Vector3 outCamScaleGround =  new Vector3(1.5f,1f,1.5f);
	public static Vector3 outCamScaleCake =  new Vector3(2f,2f,2f);

	public static Vector3 outCamScaleKumamon =  new Vector3(0.7f,0.7f,0.5f);

	public static Vector3[] getSightEdgePoint(GameObject aimTarget,float aimRadius,Vector3 cameraPos){

		//Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		Vector2 cameraXY = new Vector2 (cameraPos.x,cameraPos.z);
		Vector2 aimTargetXY = new Vector2 (aimTarget.transform.position.x, aimTarget.transform.position.z);

		Vector3 CamAimVec = (aimTargetXY - cameraXY);
		float camAimLen = CamAimVec.magnitude;

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


	public enum blendMode
	{
		Opaque,
		Transparent,
		Cutout
	}

	public static void SetBlendMode(Material material, blendMode mode){

		switch (mode)
		{
		case blendMode.Opaque:
			material.SetOverrideTag("RenderType", "");
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt("_ZWrite", 1);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = -1;
			break;
		case blendMode.Transparent:
			material.SetOverrideTag("RenderType", "Transparent");
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 3000;
			break;
		case blendMode.Cutout:
			material.SetOverrideTag("RenderType", "TransparentCutout");
			material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
			material.SetInt("_ZWrite", 1);
			material.EnableKeyword("_ALPHATEST_ON");
			material.DisableKeyword("_ALPHABLEND_ON");
			material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
			material.renderQueue = 2450;
			break;
		}
	}

}
