using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementWander : MonoBehaviour {

//	GameObject aimTarget;
	int moveframeCnt = 0;
	int ancflg=1;
	float sigWait = 1.0f;
	float forwardSpeed = 0.05f;

//	public void SetAimTarget (GameObject aimTarget)
//	{
//		this.aimTarget = aimTarget;
//	}
	
	// Update is called once per frame
	void Update () 
	{
		moveframeCnt += 1;
		if (moveframeCnt > 10000) {
			moveframeCnt = 0;
		}
		float dz = 0f;
		float dx = 0f;

		dz = ancflg * Mathf.Sin (2.0f * Mathf.PI * sigWait * (float)(moveframeCnt % 200) / (200.0f - 1.0f));
		dx = ancflg * Mathf.Sqrt (1.0f - Mathf.Pow (dz, 2));

		Vector3 seekPois = transform.position 
			+ (new Vector3 (dx * forwardSpeed * 2, transform.position.y-0.2f, dz * forwardSpeed * 2));

		//ground check
		bool findGround = false;
		Collider[] edges = Physics.OverlapSphere (seekPois, 0.1f, -1, QueryTriggerInteraction.Collide);
		for (int i = 0; i < edges.Length; i++) {
			if (edges [i].gameObject.tag == CommonStatic.GROUND_TAG) {
				findGround = true;
				break;
			}
		}

		//cake,cap,son
		seekPois = transform.position 
			+ (new Vector3 (dx * forwardSpeed * 2, transform.position.y+0.1f, dz * forwardSpeed * 2));

		bool entryEny = false;
		Collider[] objes = Physics.OverlapSphere (seekPois, 0.1f, -1, QueryTriggerInteraction.Collide);
		for (int i = 0; i < objes.Length; i++) {
			if ((objes [i].gameObject.tag == CommonStatic.CAP_TAG)||
				(objes [i].gameObject.tag == CommonStatic.CAKE_TAG)||
				(objes [i].gameObject.tag == CommonStatic.SON_TAG)){
				entryEny = true;
				break;
			}
		}

		if ((!findGround)||(entryEny)) {
			ancflg = -1 * ancflg;
		}

		Vector3 direction = new Vector3 (dx, 0, dz);
		transform.rotation = Quaternion.LookRotation (direction);
		transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime;
//		float arclength = aimTarget.transform.lossyScale.x / 2.0f;
//
//		moveframeCnt += 1;
//		if (moveframeCnt > 10000) {
//			moveframeCnt = 0;
//		}
//		float dz = 0f;
//		float dx = 0f;
//
//		dz = ancflg*Mathf.Sin (2.0f * Mathf.PI * sigWait * (float)(moveframeCnt % 200) / (200.0f - 1.0f));
//		dx = ancflg*Mathf.Sqrt (1.0f - Mathf.Pow (dz, 2));
//
//		Vector3 footpos = new Vector3(transform.position.x,aimTarget.transform.position.y, transform.position.z);
//
//		float distCenter = (footpos - aimTarget.transform.position).magnitude;
//		if (distCenter > arclength*0.9) {
//			Vector3 backDir=(aimTarget.transform.position - transform.position).normalized;
//			dx = backDir.x;
//			dz = backDir.z;
//			ancflg = -1 * ancflg;
//		}
//		Vector3 direction = new Vector3 (dx,0,dz);
//		transform.rotation = Quaternion.LookRotation(direction);
//		transform.localPosition += transform.forward * forwardSpeed * Time.fixedDeltaTime;	
	}
}
