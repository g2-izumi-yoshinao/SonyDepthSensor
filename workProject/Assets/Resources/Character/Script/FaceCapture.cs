//
//  FaceCapture
//  Created by Yoshinao Izumi on 2017/04/19.
//  Copyright © 2017 Yoshinao Izumi All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceCapture : MonoBehaviour 
{
	private WebCamTexture webcamTexture;
	public RawImage rawimage;
	public Texture2D alphaMask; //*使用カメラでの取得サイズ合わせて作成する
	//public Material  faceMat;
	public GameObject faceRelpace;
	public int Width = 1280; //* 測定値: webComは設定してもそのサイズならない
	public int Height = 720; //* 縮小綺麗にする時間ないのでマスク側をこのサイズ合わせる
	public int FPS = 30;
	void Start () 
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		for (var i = 0; i < devices.Length; i++) {
			Debug.Log (devices [i].name);
		}
		webcamTexture = new WebCamTexture(devices[0].name);
		rawimage.texture = webcamTexture;
		rawimage.material.mainTexture = webcamTexture;
		webcamTexture.Play();

		faceRelpace.SetActive(false);
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.Space) || Input.touchCount > 0) {
			capture ();
		}
	}

	private void capture(){
		Color32[] src = webcamTexture.GetPixels32();
		Color32[] mask = alphaMask.GetPixels32();
		for (int i = 0; i < src.Length; i++) {
			if(mask [i].a==0){
				src [i].a = 0;
			}
		}
		Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
		faceRelpace.GetComponent<MeshRenderer> ().materials [0].mainTexture = texture;
		texture.SetPixels32(src);
		texture.Apply();
		faceRelpace.SetActive(true);
	
	}


}
