using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour{

	public ViewController homeView;
	public ViewController characterSelectView;
	public ViewController defaultCharacterConfirmView;
	public ViewController pictureCaptureView;
	public ViewController pictureConfirmView;
	public ViewController EndingView;
	public ViewController videoUploadView;

	public ViewController InStartView;
	public ViewController InExplanationFirstView;
	public ViewController InExplanationSecondView;
	public ViewController InCharacterSelectView;
	public ViewController InCharacterConfirmView;


	Stack<ViewController> viewStack = null;

	ViewController currentView{
		get{
			return viewStack.Count == 0 ? null : viewStack.Peek ();
		}
	}

	public void Init()
	{
		viewStack = new Stack<ViewController> ();
	}

	void Destroy()
	{	
		foreach (ViewController v in viewStack) {
			
		}
	}

	public void Push (ViewController nextView)
	{
		//hide current view
		if(currentView != null)
			currentView.MoveOut();
		
		viewStack.Push (nextView);


		//show next view
		currentView.MoveIn();
	}

	public void Pop ()
	{
		//remove currentView
		currentView.Remove();

		viewStack.Pop ();

		//show previous view
		currentView.MoveBackIn();
	}
}
