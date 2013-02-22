using UnityEngine;
using System.Collections;

public abstract class InputManaged : MonoBehaviour
{
	public abstract void Send(CommandCode[] codes);
}
