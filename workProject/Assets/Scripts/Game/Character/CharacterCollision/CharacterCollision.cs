using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision {

	GameObject character;

	public CharacterCollision (GameObject character)
	{
		this.character = character;
	}

	public void OnTriggerEnter(Collider other)
	{
//		Debug.Log ("Collision!  [" + other.gameObject.name + "] entered [" + character.name + "]");
	}

	public void OnTriggerExit(Collider other) 
	{
//		Debug.Log ("Collision!  [" + other.gameObject.name + "] exit [" + character.name + "]");
	}
}
