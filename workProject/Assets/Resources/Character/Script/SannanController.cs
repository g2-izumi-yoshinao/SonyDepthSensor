using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SannanController : SimpleController {

	private static string ANIM_TRIGGER_WOWJUMP_NAME = "WowJump";
		private float bodySinframeCnt;
	private float bodySignFreq = 3.0f;

	private bool onRunning=false;
	private float perRndRote=3;
	private int runLoopCnt=0;
	private bool preHit=false;
	private Collider targetCollider;
	private int runLoopMaxCnt=6;

	private float rndflg=1;
	private float currentRnd=0;

	protected override void onStart(){
		
	}

	protected override void onUpdate () {
		
		if (loadFirst) {
			loadFirst = false;
			lookCamera ();
			Collider[] colliders=AimTarget.GetComponentsInChildren<Collider>(true);
			foreach(Collider cl in colliders){
				if (cl.gameObject.name=="mesh_cup_base") {
					targetCollider = cl;
					break;
				}
			}
		}

		if (onPointState) {
			if (onPointStateInit) {
				onPointStateInit = false;
				onProximity = true;
				initProximity = true;
			}
		}

		if (onProximity) {
			if (initProximity) {
				initProximity = false;
				runLoopCnt = 0;
				preHit = false;
				transform.rotation = Quaternion.identity;
				if (pinchingCharacter != null) {
					Vector3 targetDir = new Vector3 (pinchingCharacter.transform.position.x,
						transform.position.y,
						pinchingCharacter.transform.position.z);
					transform.root.LookAt (targetDir);
				} else {
					onProximityPreset = false;
					onProximity = false;
				}
				animator.SetTrigger (ANIM_TRIGGER_WOWJUMP_NAME);
			}
		}

		if (onRunning) {
			rotaionXYTaget ();
			Vector3 rayDir = (VirtualCameraPos - transform.position).normalized;
			Ray ray = new Ray(transform.position,rayDir);
			RaycastHit hit;
			bool currenthit = (targetCollider.Raycast (ray, out hit, 5.0f));
			if (preHit != currenthit) {
				preHit = currenthit;
				runLoopCnt++;
				if (runLoopCnt > runLoopMaxCnt) {
					if (!preHit) {
						onRunning = false;
						onProximity = false;
						onPointState = false;
						animator.SetTrigger (ANIM_TRIGGER_STANDING_NAME);
					}
				}
			}
		}

	}

	protected override void onLateUpdate(){
		if ((!onProximity)&&(!onPointState)){
			swing ();
		}
	}
		
	private bool swing(){

		float signAmp = Mathf.Sin ((float)(2.0f * Mathf.PI * bodySignFreq * (bodySinframeCnt++ % 200) / 199.0f));
		if (bodySinframeCnt > 10000) {
			bodySinframeCnt = 0;
		}
		Quaternion bodyQ = Quaternion.AngleAxis (54f*signAmp, new Vector3 (0, 0, 1));
		transform.rotation = bodyQ;
		return true;

	}

	private void rotaionXYTaget(){

		Vector3 currentPos = transform.position;
		Vector3 nextPos = getRotationXYTragetPos (transform.position, rndflg*perRndRote);
		transform.position = nextPos;
		currentRnd -= perRndRote;
		Vector3 movedir = (transform.position - currentPos).normalized;
		transform.rotation = Quaternion.LookRotation(movedir);
	}

	public void OnJumpEndFlame(){
		Debug.Log ("OnJumpEndStartFlame");
	}
	public void OnRunAnimationStartFlame(){
		Debug.Log ("OnRunAnimationStartFlame");
		onRunning = true;
	}


}
