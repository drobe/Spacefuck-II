/* Inner Arena Controller
 * This is a script that manages Spacefuck GameObjects leaving the inner arena boundaries.
 * All objects that leave the inner boundaires are told to explode.
 * */

using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour
{
}

public class InnerArenaController : Boundary
{
#if UNITY_EDITOR
	public int count = 0;
#endif
	void OnTriggerStay(Collider other)
	{
		if (!Network.isClient)
		{
#if UNITY_EDITOR
			count++;
#endif
			if (collider.bounds.Contains(other.bounds.center))
			{
				other.SendMessage("Explode",SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
