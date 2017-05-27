using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IViewController {

	//triggered when drawn over by next view 
	void MoveOut();

	//triggered when view is popped and will be destroyed
	void Remove();

	//triggered when moved in as new view
	void MoveIn();

	//triggered when previous view is popped and move back in as old view
	void MoveBackIn();
}
