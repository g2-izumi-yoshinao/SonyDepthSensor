using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData {

	static CharacterData _instance = null;

	public static CharacterData Instance{
		get{
			if (_instance == null) 
			{
				_instance = new CharacterData ();
				_instance.Init ();
			}

			return _instance;
		}
	}

	public GameObject FirstSon{ get; set;}
	public GameObject SecondSon{ get; set;}
	public GameObject ThirdSon{ get; set;}

	public void Init()
	{
		FirstSon  = null;
		SecondSon = null;
		ThirdSon  = null;
	}

	public void Destroy()
	{
		GameObjectSafeDestroy (FirstSon);
		GameObjectSafeDestroy (SecondSon);
		GameObjectSafeDestroy (ThirdSon);

		_instance = null;
	}

	void GameObjectSafeDestroy (GameObject obj)
	{
		if (obj != null)
			GameObject.Destroy (obj);

		obj = null;
	}
}
