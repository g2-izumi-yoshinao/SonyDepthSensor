using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testChara : MonoBehaviour {

	private static string ANIM_BASE_LAYER ="Base Layer";
	private static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"walk";
	static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);

	Animator anim;
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void Update () {
		if(Input.GetKey(KeyCode.Space)){
			AnimatorStateInfo currentAnimationState= anim.GetCurrentAnimatorStateInfo (0);
			if (currentAnimationState.fullPathHash != WalkingState) {
				anim.SetTrigger ("Walk");
			}
		}
	}
}
