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

	private float scaleCharacter = 1.5f;//may required caluc self in relation with cake scale or some

	private Vector3 postionMainCamera = new Vector3(0f,0,0f);

	private Vector3 postionGround = new Vector3(0f,-0.77f,-1.1f);
	private Vector3 scaleGround =  new Vector3(1.5f,1f,1.5f);

	private Vector3 postionCake = new Vector3(-0.1f,-0.05f,-0.6f);
	private Vector3 scaleCake =  new Vector3(0.2f,0.05f,0.2f);

	private Vector3 postionCap = new Vector3(0.3f,-0.03f,-0.6f);
	private Vector3 scaleCap =  new Vector3(0.1f,0.1f,0.1f);

	void Start () {
		//sensor start 
		sensorPrepared=true; // switch to callback or wait
	}

	void Update () {

		if (sensorPrepared) {
			if (!loaded) {
				loaded = true;

				GameObject.FindGameObjectWithTag ("MainCamera").transform.position = postionMainCamera;

				groundObj = Instantiate (groundPrefab, postionGround, Quaternion.identity);
				groundObj.transform.localScale = scaleGround;

				cakeObj = Instantiate (cakePrefab, postionCake, Quaternion.identity);
				cakeObj.transform.localScale = scaleCake;

				capObj = Instantiate (capPrefab, postionCap, Quaternion.identity);
				capObj.transform.localScale = scaleCap;

				//calc scaleCharacter?

			    //sons
				GameObject son;
				Vector3[] edges;
				int randomVal;
				sonObjs = new SimpleCharacterController[3];

				//first son 
				Vector3 firstSonStartPos = new Vector3 (
					cakeObj.transform.position.x,
					cakeObj.transform.position.y+cakeObj.transform.lossyScale.y,
					cakeObj.transform.position.z);
				
				son = Instantiate (sonPrefabs [0], firstSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [0] = son.GetComponentInChildren<SimpleCharacterController> (true);
			
				//second son
				edges= CommonStatic.getSightEdgePoint (cakeObj);
				randomVal =UnityEngine.Random.Range (0, 10);
				//Vector3 secondSonStartPos=((randomVal % 2)==0)?edges[0]:edges[1];
				Vector3 secondSonStartPos;
				bool edg0 = false;
				if ((randomVal % 2) == 0) {
					edg0 = true;
					secondSonStartPos=edges [0];
				} else {
					secondSonStartPos=edges [1];
				}
				son = Instantiate (sonPrefabs [1], secondSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [1] = son.GetComponentInChildren<SimpleCharacterController> (true);
				if (edg0) {
					son.transform.position += new Vector3(-1.0f*son.transform.lossyScale.y/40.0f, 0, 0);
				} else {
					son.transform.position += new Vector3 (son.transform.lossyScale.y/40f, 0, 0);
				}

				//third son
				edges= CommonStatic.getSightEdgePoint (capObj);
				randomVal =UnityEngine.Random.Range (0, 10);
				Vector3 edgepos=((randomVal % 2)==0)?edges[0]:edges[1];
				float hideback = capObj.transform.lossyScale.x / 2.0f;
				Vector3 thirdSonStartPos = new Vector3 (edgepos.x, edgepos.y, edgepos.z - hideback);
				son = Instantiate (sonPrefabs [2], thirdSonStartPos, Quaternion.identity);
				son.transform.localScale = new Vector3 (scaleCharacter, scaleCharacter, scaleCharacter);
				sonObjs [2] = son.GetComponentInChildren<SimpleCharacterController> (true);

				sonObjs [0].initSon (cakeObj);
				sonObjs [1].initSon (cakeObj);
				sonObjs [2].initSon (capObj);
			
				foreach(SimpleCharacterController mb in sonObjs){
					mb.setAction (true);
				}

				//debug----
				GameObject me=Instantiate(mePrefab,new Vector3(0.1f,-0.2f,-0.55f), Quaternion.identity);
				me.transform.localScale = new Vector3(scaleCharacter, scaleCharacter, scaleCharacter);
				meObj = me.GetComponentInChildren<ReactionCharacterController> (true);
				meObj.setAction (true);

			}
		}

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
