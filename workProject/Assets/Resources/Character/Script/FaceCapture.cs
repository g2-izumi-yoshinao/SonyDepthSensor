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
	WebCamTexture webcamTexture; //raw texture of webcamdevice
	Texture2D resizedWebcamTexture; //resized raw texture of webcamdevice
	Rect camAreaRect;

	public RawImage rawimage; //texture will be copied to this rawimage
	public Texture2D alphaMask; //*使用カメラでの取得サイズ合わせて作成する (texture size need to be same as size of camera)
	public Texture2D capturedTexture;
//	public GameObject faceRelpace; //this gameobject needs to have MeshRenderer attached

	public int Width = 1280; //* 測定値: webComは設定してもそのサイズならない
	public int Height = 720; //* 縮小綺麗にする時間ないのでマスク側をこのサイズ合わせる
	public int FPS = 30;

	public void Init () 
	{
		WebCamDevice[] devices = WebCamTexture.devices;
//		for (var i = 0; i < devices.Length; i++) {
//			Debug.Log (devices [i].name);
//		}
			
		//webcam texture
		webcamTexture = new WebCamTexture(devices[0].name);
		rawimage.material.SetTexture ("_MaskTex", alphaMask);

		webcamTexture.Play();
//		faceRelpace.SetActive(false);
	}

	void Update()
	{
		//Texture created at last frame needs to be released
		ReleaseIntermediateTexture ();
		
		if (!webcamTexture.isPlaying)
			return;
		
		//update camera rect depending on webcam texture size
		UpdateCameraAreaRect (webcamTexture);

		//trim webcamtexture by the specified rect
		resizedWebcamTexture = TrimTexture (webcamTexture, camAreaRect);

		//set webcamtexture to renderering image
		rawimage.texture              = resizedWebcamTexture;
		rawimage.material.mainTexture = resizedWebcamTexture;
	}

	public void Destroy()
	{
		webcamTexture.Stop();

		if (resizedWebcamTexture != null) 
		{
			Destroy (resizedWebcamTexture);
			resizedWebcamTexture = null;
		}
	}

	//Use RenderTexture for capture
	public Texture2D Capture()
	{
		RenderTexture dst = RenderTexture.GetTemporary (resizedWebcamTexture.width, resizedWebcamTexture.height);

		Graphics.Blit (resizedWebcamTexture, dst);
		Texture2D tex2D = ConvertRenderTexToTex2D (dst);

		RenderTexture.ReleaseTemporary (dst);
		return tex2D; 

		return null;
	}

	#region Private

	void UpdateCameraAreaRect(WebCamTexture webcamTex)
	{	
		int x = webcamTex.width/2  - Constants.CAM_CAP_WIDTH/2  + Constants.CAM_CAP_CENTER[0];
		x = x < 0 ? 0 : x;

		int y = webcamTex.height/2 - Constants.CAM_CAP_HEIGHT/2 + Constants.CAM_CAP_CENTER[1];
		y = y < 0 ? 0 : y;

		int width  = Constants.CAM_CAP_WIDTH;
		int height = Constants.CAM_CAP_HEIGHT;

		camAreaRect = new Rect (x, y, width, height);
	}

	void ReleaseIntermediateTexture()
	{
		if (resizedWebcamTexture != null) {
			Destroy (resizedWebcamTexture);
			resizedWebcamTexture = null;
		}
	}

	Texture2D ConvertRenderTexToTex2D( RenderTexture rt )
	{
		Texture2D texture = new Texture2D( rt.width, rt.height, TextureFormat.ARGB32, false, false );

		RenderTexture.active = rt;
		texture.ReadPixels( new Rect( 0, 0, rt.width, rt.height), 0, 0 );
		texture.Apply();

		//clean
		RenderTexture.active = null;
		return texture;
	}

	Texture2D TrimTexture (WebCamTexture srcTexture, Rect rect)
	{
		int rw = (int)rect.width;
		int rh = (int)rect.width;
		int rx = (int)rect.x;
		int ry = (int)rect.y;

		Texture2D dstTexture = new Texture2D(rw, rh);

		int maxX = rx + rw;
		int maxY = ry + rh;

		for (int x = rx; x < maxX; x++) 
		{
			for (int y = ry; y < maxY; y++) 
			{
				//get color for (x, y) of source texture
				Color targetPixel = srcTexture.GetPixel (x, y);

				//set to destination texture
				int coordX = x - rx;
				int coordY = y - ry;
				dstTexture.SetPixel (coordX, coordY, targetPixel);
			}
		}
		dstTexture.Apply ();
		return dstTexture;
	}

	#endregion
}
