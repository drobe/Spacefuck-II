/* Outer Arena Controller
 * This is a script that manages Spacefuck GameObjects leaving the outer arena boundaries.
 * All objects that leave the inner boundaires are destroyed. Period.
 * */

using UnityEngine;
using System.Collections;

public class OuterArenaController : Boundary 
{
#if UNITY_EDITOR
	public int count = 0;
#endif
	void OnTriggerEnter(Collider other)
	{
		if (!Network.isClient)
		{
#if UNITY_EDITOR

			count++;
#endif
			other.gameObject.SendMessage("DestroySF",SendMessageOptions.DontRequireReceiver);
		}
	}
}
