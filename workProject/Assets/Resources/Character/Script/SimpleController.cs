using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour {

	private static int BodyAnimationLayor = 0;
	public static string POINT_POINT_TAG = "point";

	protected static string ANIM_BASE_LAYER ="Base Layer";
	protected AnimatorStateInfo currentAnimationState;	

	protected static string ANIM_STANDING_LOOP = ANIM_BASE_LAYER+"."+"Standing@loop";
	protected static string ANIM_WALKING_LOOP = ANIM_BASE_LAYER+"."+"Walking@loop";

	protected static int StandingState = Animator.StringToHash (ANIM_STANDING_LOOP);
	protected static int WalkingState = Animator.StringToHash (ANIM_WALKING_LOOP);

	protected static string ANIM_TRIGGER_STANDING_NAME = "Standing";
	protected static string ANIM_TRIGGER_WALKING_NAME = "Walking";


	protected Vector3 VirtualCameraPos;
	protected GameObject AimTarget;// should set by initFirstSon. public is for first test
	protected Vector3 AimTargetExecuteSize;//executed unity size
	protected bool onProximityPreset=false;
	protected bool onProximity=false;
	protected bool initProximity=false;
	protected bool onPointState=false;
	protected bool onPointStateInit=false;

	protected float alphaVal=0.0f;
	public Material material_texture1;//even model is in test state  
	public Material material_texture2;
	public Material material_greenM;
	public Material material_hadaM;
	protected bool onFadeIn=false;

	protected AudioSource sound;
	protected Animator animator;
	protected Rigidbody rigid;
	protected bool loadFirst = true;
	protected ReactionCharacterController pinchingCharacter;

	public GameObject cakePiecePref;

	private bool onAction=false;


	void Start () {
		animator = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody> ();
		sound = GetComponent<AudioSource> ();

		onStart ();

		setAlpha (0);
	}
	
	void Update () {
		if (!onAction) {
			return;
		}
		if (loadFirst) {
			commonInit ();
		}

		rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
		rigid.useGravity = true;
		rigid.isKinematic = false;

		currentAnimationState = animator.GetCurrentAnimatorStateInfo (BodyAnimationLayor);

		if (onFadeIn) {
			if (fadeIn ()) {
				return;
			} else {
				onFadeIn = false;
			}
		}

		if (onProximityPreset) {
			if (pinchingCharacter) {
				if (pinchingCharacter.isOnGround ()) {
					onProximityPreset = false;
					onProximity = true;
					initProximity = true;
				}
			} else {
				onProximityPreset = false;
				onProximity = false;
			}
		}
		onUpdate ();
	}

	void LateUpdate () {
		if (!onAction) {
			return;
		}
		onLateUpdate ();
	}

	public void setAction(bool active){
		if (active) {
			onAction = true;
		} else {
			onAction = false;
		}
	}

	public void clear(){
		onAction = false;
		pinchingCharacter = null;
	}


	protected virtual void onStart(){
	}

	protected virtual void onUpdate(){
	}

	protected virtual void onLateUpdate(){
	}

	protected void clearVelocityXZ(){
		rigid.velocity = new Vector3 (0, rigid.velocity.y, 0);
		rigid.angularVelocity = Vector3.zero;
	}

	public void doFadeIn(){
		onFadeIn = true;
	}

	protected bool fadeIn(){
		bool onProcess = true;
		alphaVal += Time.deltaTime;
		if (alphaVal >= 1.0f) {
			alphaVal = 1.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	protected bool fadeOut(){
		bool onProcess = true;
		alphaVal -= Time.deltaTime;
		if (alphaVal < 0.0f) {
			alphaVal = 0.0f;
			onProcess = false;
		}
		setAlpha (alphaVal);
		return onProcess;
	}

	private void setAlpha(float alpha){

		if (alpha == 1.0) {

			material_texture1.color 
			= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
			= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
			= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
			= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

			CommonStatic.SetBlendMode(material_texture1,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_texture2,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_greenM,CommonStatic.blendMode.Opaque);
			CommonStatic.SetBlendMode(material_hadaM,CommonStatic.blendMode.Opaque);

		}else if (alpha == 0.0){
			CommonStatic.SetBlendMode(material_texture1,CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode(material_texture2,CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode(material_greenM,CommonStatic.blendMode.Cutout);
			CommonStatic.SetBlendMode(material_hadaM,CommonStatic.blendMode.Cutout);

			material_texture1.color 
			= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
			= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
			= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
			= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

		}else{
			CommonStatic.SetBlendMode(material_texture1,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_texture2,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_greenM,CommonStatic.blendMode.Transparent);
			CommonStatic.SetBlendMode(material_hadaM,CommonStatic.blendMode.Transparent);

			material_texture1.color 
			= new Color (material_texture1.color.r, material_texture1.color.g, material_texture1.color.b, alpha);
			material_texture2.color 
			= new Color (material_texture2.color.r, material_texture2.color.g, material_texture2.color.b, alpha);
			material_greenM.color 
			= new Color (material_greenM.color.r, material_greenM.color.g, material_greenM.color.b, alpha);
			material_hadaM.color 
			= new Color (material_hadaM.color.r, material_hadaM.color.g, material_hadaM.color.b, alpha);

		}
	}

	public void initSon(GameObject aim, Vector3 executesize,Vector3 camerapos){
		AimTarget = aim;
		AimTargetExecuteSize = executesize;
		VirtualCameraPos = camerapos;
	}

	private void commonInit(){
		onPointStateInit = false;
		onPointState = false;

	}

	protected void lookCamera(){
		Vector3 cameraPos = VirtualCameraPos;
		Vector3 cameraX0Y = new Vector3 (cameraPos.x,transform.position.y,cameraPos.z);

		//とりあえず直向
		transform.LookAt(cameraX0Y);

	}

	protected void lookTarget(){
		Vector3 targetX0Y = new Vector3 (AimTarget.transform.position.x,AimTarget.transform.position.y,
			AimTarget.transform.position.z);
		//とりあえず直向
		transform.LookAt(targetX0Y);
	}

	protected Vector3 getRotationXYTragetPos(Vector3 current,float eula){

		Vector2 aimTargetXY = new Vector2 (AimTarget.transform.position.x, AimTarget.transform.position.z);
		Vector2 currentXY = new Vector2 (current.x, current.z);
		float cos = Mathf.Cos (eula * Mathf.PI / 180);
		float sin = Mathf.Sin (eula * Mathf.PI / 180);

		Vector2 nom = currentXY - aimTargetXY;
		Vector3 ret = new Vector3 ((nom.x * cos - nom.y * sin) + aimTargetXY.x, 
			current.y, (nom.x * sin + nom.y * cos) + aimTargetXY.y);
		return ret;
	}

	//=====================================================
	public void onPoint(){
		if (!onPointState) {
			onPointStateInit = true;
			onPointState = true;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!onAction) {
			return;
		}
		//proximity-------
		if (other.gameObject.tag == CommonStatic.REACTINO_CHARACTER_TAG) {
			pinchingCharacter = other.gameObject.GetComponentInParent<ReactionCharacterController> ();
			onProximityPreset = true;


		}
		OnEnterTrigger (other);
	}

	protected virtual void OnEnterTrigger(Collider other) {
	}

	void OnTriggerExit(Collider other) {
		// proximity-------
		if (other.gameObject.tag == CommonStatic.REACTINO_CHARACTER_TAG) {
			pinchingCharacter = null;
		}
	}

	protected virtual void OnExitTrigger(Collider other) {
	}

	//debug
	bool onprox=false;
	public void testSetProximity(){
		if (!onprox) {
			onprox = true;
			GameObject m = GameObject.FindGameObjectWithTag ("me");
			pinchingCharacter = m.GetComponentInChildren<ReactionCharacterController> (true);
			onProximity = true;
			initProximity = true;
			onprox = false;
		}

	}

	public static Quaternion identityQue(){
		return Quaternion.AngleAxis (-180, new Vector3 (0, 1, 0));
	}
}
