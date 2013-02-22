using UnityEngine;
using System.Collections;

public class ChangeDetector2 : MonoBehaviour 
{
	[SerializeField]
	private int index 			= -1;
	private bool firstEnable 	= true;
	private bool firstDisable 	= true;
	
	
	public void SetIndex(int i)
	{
		index = i;
	}
	public void ManualQueue()
	{
		NetworkCommunicator.Queue(index);
	}
	void OnCollisionEnter()
	{
		NetworkCommunicator.Queue(index);
	}
	
	void OnEnable()
	{
		if (firstEnable)
		{
			firstEnable = false;
		}
		else
		{
			NetworkCommunicator.Activate(index);
		}
	}
	
	void OnDisable()
	{
		if (firstDisable)
		{
			firstDisable = false;
		}
		else
		{
			NetworkCommunicator.Deactivate(index);
		}
	}
}
