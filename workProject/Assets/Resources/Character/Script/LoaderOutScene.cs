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
	public ChonanController chonan;
	public JinanController jinan;
	public SannanController sannnan;
	public GameObject groundPrefab;
	public GameObject cakePrefab;
	public GameObject capPrefab;

	private ReactionCharacterController meObj;
	private SimpleController[] sonObjs;
	private GameObject groundObj;
	private GameObject cakeObj;
	private GameObject capObj;

	private bool sensorPrepared=false;
	private bool loaded=false;

	//sencor value estimage 
	private float groundy=-0.6f;
	private float scaleCharacter =2.0f; //1.5f;//may required caluc self in relation with cake scale or some
	//private Vector3 postionMainCamera = new Vector3(0f,0,0f); for debug
	private Vector2 postionGroundCenterXY = new Vector2(0f,0.25f);
	private Vector2 postionCakeCenterXY = new Vector2(-0.165f,0.8f);
	private Vector2 postionCapCenterXY = new Vector2(0.15f,0.597f);

	private Vector3 groundExecuteSize;
	private Vector3 cakeExecuteSize;
	private Vector3 capExecuteSize;

	private float groundTop=0;

	void Start () {
		//sensor start 
		sensorPrepared=true; // switch to callback or wait
	}

	void Update () {

		if (sensorPrepared) {
			if (!loaded) {
				loaded = true;

				//GameObject.FindGameObjectWithTag ("MainCamera").transform.position = postionMainCamera;

				Vector3 meshSize;
				groundObj = Instantiate (groundPrefab, new Vector3 (1, 0, 0), Quaternion.identity);
				groundObj.transform.localScale = CommonStatic.outCamScaleGround;
				meshSize=  groundObj.GetComponent<MeshFilter>().mesh.bounds.size;
				groundExecuteSize 
				   = new Vector3 (meshSize.x * CommonStatic.outCamScaleGround.x,
					meshSize.y * CommonStatic.outCamScaleGround.y,
					meshSize.z * CommonStatic.outCamScaleGround.z);
				Vector3 postionGround = new Vector3 (postionGroundCenterXY.x, groundy, postionGroundCenterXY.y);
				groundObj.transform.position = postionGround;

				groundTop = groundy + groundExecuteSize.y / 2;


				Quaternion cakeQ = Quaternion.AngleAxis (-90, new Vector3 (1, 0, 0));
				cakeObj = Instantiate (cakePrefab, new Vector3(0,0,0), cakeQ);
				cakeObj.transform.localScale = CommonStatic.outCamScaleCake;
				meshSize = cakeObj.GetComponent<MeshFilter>().mesh.bounds.size;
				cakeExecuteSize 
					= new Vector3 (
					meshSize.x * CommonStatic.outCamScaleCake.x,
					meshSize.y * CommonStatic.outCamScaleCake.y,
					meshSize.z * CommonStatic.outCamScaleCake.z);
				//groundTop+cakeExecuteSize.z/2.0f
				Vector3 postionCake = new Vector3 (postionCakeCenterXY.x,-0.1f, postionCakeCenterXY.y);
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
				//groundTop+capExecuteSize.y/2.0f
				Vector3 postionCap = new Vector3 (postionCapCenterXY.x, -0.1f, postionCapCenterXY.y);
				capObj.transform.position = postionCap;


				Vector3 cameraPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
				//float diff= groundObj.transform.position.z-cameraPos.z;
				//Vector3 VirtualCameraPos = new Vector3 (cameraPos.x, cameraPos.y, -2.0f);
				Vector3 VirtualCameraPos = cameraPos;

			    //sons
				Vector3[] edges;
				int randomVal;
				sonObjs = new SimpleController[3];
	
				//first son 
				Vector3 firstSonStartPos = new Vector3 (
					cakeObj.transform.position.x,
					cakeObj.transform.position.y+cakeExecuteSize.z/2.0f + CommonStatic.charaRateY/2f,
					cakeObj.transform.position.z);
				
				ChonanController chonanObj = Instantiate (chonan, firstSonStartPos, SimpleController.identityQue());
				chonanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [0] = chonanObj.GetComponentInChildren<SimpleController> (true);
			
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
				secondSonStartPos.y = groundTop+CommonStatic.charaRateY / 2f;
				JinanController jinanObj = Instantiate (jinan, secondSonStartPos, SimpleController.identityQue());
				jinanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [1] = jinanObj.GetComponentInChildren<SimpleController> (true);
				float sideSpan = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y)/2 ;
				int flg = onleft ? 1 : -1;
				jinanObj.transform.position += new Vector3 (flg*sideSpan, 0,0);

				//third son
				edges= CommonStatic.getSightEdgePoint (capObj,capExecuteSize.x/2.0f,VirtualCameraPos);
				randomVal =UnityEngine.Random.Range (0, 10);
				Vector3 edgepos=((randomVal % 2)==0)?edges[0]:edges[1];
				float hideback = (CommonStatic.charaRateY * CommonStatic.outCamScaleCharacter.y)*1.1f;
				Vector3 thirdSonStartPos = new Vector3 (edgepos.x, 
					groundTop+CommonStatic.charaRateY / 2f, edgepos.z + hideback);
				SannanController sannanObj = Instantiate (sannnan, thirdSonStartPos, SimpleController.identityQue());
				sannanObj.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [2] = sannanObj.GetComponentInChildren<SimpleController> (true);

				//---init ---
				sonObjs [0].initSon (cakeObj,cakeExecuteSize,VirtualCameraPos);
				sonObjs [1].initSon (cakeObj,cakeExecuteSize,VirtualCameraPos);
				sonObjs [2].initSon (capObj,capExecuteSize,VirtualCameraPos);
			
				startScene ();
			}
		}
	}

	private void startScene(){

		foreach(SimpleController mb in sonObjs){
			mb.setAction (true);
			mb.doFadeIn ();
		}
			
		CapController cap = capObj.GetComponentInChildren<CapController> (true);
		cap.showKumamon ();

		//debug----
		Vector3 meStartPos = new Vector3 (postionGroundCenterXY.x ,
			groundTop + CommonStatic.charaRateY / 2f,
			postionCapCenterXY.y+0.04f);
		GameObject me=Instantiate(mePrefab,meStartPos, SimpleController.identityQue());
		me.transform.localScale = new Vector3(scaleCharacter, scaleCharacter, scaleCharacter);
		meObj = me.GetComponentInChildren<ReactionCharacterController> (true);
		meObj.setAction (true);

		ReactionCharacterController rc = meObj.GetComponentInChildren<ReactionCharacterController> (true);
		rc.stableType = false;

	}

	private void clean(){

		meObj.clear ();
		meObj = null;

		foreach(SimpleController sn in sonObjs){
			sn.clear();
		}
		sonObjs = null;
		groundObj = null;
		cakeObj = null;
		capObj = null;

	}


}
