//
//  LoaderOutScene
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderOutScene : MonoBehaviour {

	public GameObject mePrefab;
	public GameObject[] sonPrefabs;
	public GameObject groundPrefab;
	public GameObject cakePrefab;
	public GameObject capPrefab;

	private ReactionCharacterController meObj;
	private SimpleCharacterController[] sonObjs;
	private GameObject groundObj;
	private GameObject cakeObj;
	private GameObject capObj;

	private bool sensorPrepared=false;
	private bool loaded=false;

	//sencor value estimage 
	private float groundy=-0.2f;
	private float scaleCharacter = 1.5f;//may required caluc self in relation with cake scale or some
	private Vector3 postionMainCamera = new Vector3(0f,0,0f);
	private Vector2 postionGroundCenterXY = new Vector2(0f,-1.5f);
	private Vector2 postionCakeCenterXY = new Vector2(0.2f,-1.5f);
	private Vector2 postionCapCenterXY = new Vector2(-0.15f,-1.48f);

	private Vector3 groundExecuteSize;
	private Vector3 cakeExecuteSize;
	private Vector3 capExecuteSize;

	void Start () {
		//sensor start 
		sensorPrepared=true; // switch to callback or wait
	}

	void Update () {

		if (sensorPrepared) {
			if (!loaded) {
				loaded = true;

				GameObject.FindGameObjectWithTag ("MainCamera").transform.position = postionMainCamera;

				Vector3 meshSize;
				groundObj = Instantiate (groundPrefab, new Vector3 (1, 0, 0), Quaternion.identity);
				groundObj.transform.localScale = CommonStatic.outCamScaleGround;
				meshSize=  groundObj.GetComponent<MeshFilter>().mesh.bounds.size;
				groundExecuteSize 
				   = new Vector3 (meshSize.x * CommonStatic.outCamScaleGround.x,
					meshSize.y * CommonStatic.outCamScaleGround.y,
					meshSize.z * CommonStatic.outCamScaleGround.z);
				Vector3 postionGround = new Vector3 (postionGroundCenterXY.x, groundy-groundExecuteSize.y/2, postionGroundCenterXY.y);
				groundObj.transform.position = postionGround;

				//double cakeSizeY=CommonStatic.cakeRateY * CommonStatic.outCamScaleCake.y;
				//Vector3 postionCake = new Vector3 (postionCakeCenterXY.x, groundy+(float)cakeSizeY/2.0f, postionCakeCenterXY.y);

				Quaternion cakeQ = Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0));
				cakeObj = Instantiate (cakePrefab, new Vector3(0,0,0), cakeQ);
				cakeObj.transform.localScale = CommonStatic.outCamScaleCake;
				meshSize = cakeObj.GetComponent<MeshFilter>().mesh.bounds.size;
				cakeExecuteSize 
					= new Vector3 (
					meshSize.x * CommonStatic.outCamScaleCake.x,
					meshSize.y * CommonStatic.outCamScaleCake.y,
					meshSize.z * CommonStatic.outCamScaleCake.z);
				Vector3 postionCake = new Vector3 (postionCakeCenterXY.x, groundy+cakeExecuteSize.z/2.0f, postionCakeCenterXY.y);
				cakeObj.transform.position = postionCake;
	

				//double capSizeY=CommonStatic.capRateY * CommonStatic.outCamScaleCap.y;
				capObj = Instantiate (capPrefab, new Vector3(0,0,0), Quaternion.identity);
				capObj.transform.localScale =  CommonStatic.outCamScaleCap;

				MeshFilter[] capmeshed=capObj.GetComponentsInChildren<MeshFilter>(true);
				foreach(MeshFilter mf in capmeshed){
					if (mf.name == "mesh_cup_base") {
						meshSize  = mf.mesh.bounds.size;
						break;
					}
				}
				capExecuteSize 
					= new Vector3 (meshSize.x * CommonStatic.outCamScaleCap.x,
						meshSize.y * CommonStatic.outCamScaleCap.y,
						meshSize.z * CommonStatic.outCamScaleCap.z);
				Vector3 postionCap = new Vector3 (postionCapCenterXY.x, groundy+capExecuteSize.y/2.0f, postionCapCenterXY.y);
				capObj.transform.position = postionCap;


				Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
				float diff= groundObj.transform.position.z-cameraPos.z;
				Vector3 VirtualCameraPos = new Vector3 (cameraPos.x, cameraPos.y, -2.0f);
				//Vector3 VirtualCameraPos = cameraPos;

			    //sons
				GameObject son;
				Vector3[] edges;
				int randomVal;
				sonObjs = new SimpleCharacterController[3];
	
				//first son 
				Vector3 firstSonStartPos = new Vector3 (
					cakeObj.transform.position.x,
					cakeObj.transform.position.y+cakeExecuteSize.z/2.0f + CommonStatic.charaRateY/2f,
					cakeObj.transform.position.z);
				
				son = Instantiate (sonPrefabs [0], firstSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [0] = son.GetComponentInChildren<SimpleCharacterController> (true);
			
				//second son
				edges= CommonStatic.getSightEdgePoint (cakeObj,cakeExecuteSize.x/2.0f,VirtualCameraPos);
				randomVal =UnityEngine.Random.Range (0, 10);
				//Vector3 secondSonStartPos=((randomVal % 2)==0)?edges[0]:edges[1];
				Vector3 secondSonStartPos;
				bool onleft = false;
				if ((randomVal % 2) == 0) {
					onleft = true;
					secondSonStartPos=edges [0] ;
				} else {
					secondSonStartPos=edges [1];
				}
				secondSonStartPos.y = groundy+CommonStatic.charaRateY / 2f;
				son = Instantiate (sonPrefabs [1], secondSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [1] = son.GetComponentInChildren<SimpleCharacterController> (true);
				float sideSpan = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y)/2 ;
				int flg = onleft ? 1 : -1;
				son.transform.position += new Vector3 (flg*sideSpan, 0,0);

				//third son
				edges= CommonStatic.getSightEdgePoint (capObj,capExecuteSize.x/2.0f,VirtualCameraPos);
				randomVal =UnityEngine.Random.Range (0, 10);
				Vector3 edgepos=((randomVal % 2)==0)?edges[0]:edges[1];
				float hideback = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y);
				Vector3 thirdSonStartPos = new Vector3 (edgepos.x, 
					groundy+CommonStatic.charaRateY / 2f, edgepos.z + hideback);
				son = Instantiate (sonPrefabs [2], thirdSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [2] = son.GetComponentInChildren<SimpleCharacterController> (true);

				//---init ---
				sonObjs [0].initSon (cakeObj,cakeExecuteSize,VirtualCameraPos);
				sonObjs [1].initSon (cakeObj,cakeExecuteSize,VirtualCameraPos);
				sonObjs [2].initSon (capObj,capExecuteSize,VirtualCameraPos);
			
				startScene ();
			}
		}
	}

	private void startScene(){

		foreach(SimpleCharacterController mb in sonObjs){
			mb.setAction (true);
			mb.doFadeIn ();
		}
			
		CapController cap = capObj.GetComponentInChildren<CapController> (true);
		cap.showKumamon ();

		//debug----
		Vector3 meStartPos = new Vector3 (0,
			groundy + CommonStatic.charaRateY / 2f,-1.2f);
		GameObject me=Instantiate(mePrefab,meStartPos, Quaternion.identity);
		me.transform.localScale = new Vector3(scaleCharacter, scaleCharacter, scaleCharacter);
		meObj = me.GetComponentInChildren<ReactionCharacterController> (true);
		meObj.setAction (true);

		ReactionCharacterController rc = meObj.GetComponentInChildren<ReactionCharacterController> (true);
		rc.stableType = true;

	}

	private void clean(){

		meObj.clear ();
		meObj = null;

		foreach(SimpleCharacterController sn in sonObjs){
			sn.clear();
		}
		sonObjs = null;
		groundObj = null;
		cakeObj = null;
		capObj = null;

	}


}
