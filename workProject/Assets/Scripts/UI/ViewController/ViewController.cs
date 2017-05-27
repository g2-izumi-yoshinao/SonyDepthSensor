using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour, IViewController {

	//triggered when drawn over by next view 
	public virtual void MoveOut()
	{
		this.gameObject.SetActive (false);
	}

	//triggered when view is popped and will be destroyed
	public virtual void Remove(){
		this.gameObject.SetActive (false);
	}

	//triggered when moved in as new view
	public virtual void MoveIn()
	{
		this.gameObject.SetActive (true);
	}

	//triggered when previous view is popped and move back in as old view
	public virtual void MoveBackIn()
	{
		this.gameObject.SetActive (true);
	}

}
