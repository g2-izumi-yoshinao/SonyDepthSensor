using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class testCap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void ClickCap(){

		Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "images"));
		string fileName = "images/"+System.DateTime.Now.ToString ().Replace("/","_") + ".png";
		Application.CaptureScreenshot (fileName);

		var filePath = Path.Combine(Application.persistentDataPath, fileName);

		Debug.Log (filePath);
	
//		string destfile = "c:\\prj\\sdcreen";
//		FileUtil.CopyFileOrDirectory (sourcefile,destfile);
//		FileUtil.DeleteFileOrDirectory (sourcefile);
	}

}
