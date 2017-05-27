using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectResourceLoader {

	public static GameObject Load (string path, Vector3 position, float scale = 1.0f, Quaternion rotation = default(Quaternion), Transform parent = null)
	{
		
		GameObject rc = Resources.Load<GameObject> (path);

		if (rc == null) {
			Debug.LogError ("resource load failed : [path]  " + path);
			return null;
		}

		//instantiate GameOjbect
		GameObject obj;
		if(parent == null)
			obj = Object.Instantiate (rc, position, rotation) as GameObject;
		else
			obj = Object.Instantiate (rc, position, rotation, parent) as GameObject;
		
		//set scale
		obj.transform.localScale = new Vector3 (scale, scale, scale);
			
		return obj;
	}
}
