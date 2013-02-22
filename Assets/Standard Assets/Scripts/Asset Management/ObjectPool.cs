using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour 
{
	static private readonly Vector3 position 	= new Vector3(-10000,0,0);
	
	[System.Serializable]
	public class PoolingDescriptor
	{
		public GameObject pooledObject;
		public int count;
	}
	
	public GameObject[] 		staticPool 		= new GameObject[0];
	public PoolingDescriptor[] 	descriptors 	= new PoolingDescriptor[0];
	
	static private GameObject[] pooledObjects;
	static private ObjectPool instance;
	
	// Use this for initialization
	void Awake () 
	{
		instance = this;
		
		int poolsize 					= staticPool.Length;
		int count 						= 0;
		foreach (PoolingDescriptor pd in descriptors)
		{
			poolsize 					+= pd.count;
		}
		pooledObjects 					= new GameObject[poolsize];
		
		foreach (GameObject go in staticPool)
		{
			pooledObjects[count] 		= go;
			go.transform.parent 		= transform;
	
			ChangeDetector2 cd 	= go.GetComponent<ChangeDetector2>();
			if (cd)
			{
				cd.SetIndex(count);
			}
			ObjectPool.Destroy(go);
			count++;
		}
		foreach (PoolingDescriptor pd in descriptors)
		{
			for(int i = 0; i < pd.count; i++)
			{
				GameObject go 			= (GameObject)Object.Instantiate(pd.pooledObject);
				pooledObjects[count] 	= go;
				go.transform.parent 	= transform;
				
				ChangeDetector2 cd 		= go.GetComponent<ChangeDetector2>();
				if (cd)
				{
					cd.SetIndex(count);
				}
				ObjectPool.Destroy(go);
				count++;
			}
		}
	}
																				
	
	// "Clears" the world state by moving all pooled objects off screen and deactivating them.
	static public void Clear()
	{
		foreach (GameObject go in pooledObjects)
		{
			go.SetActive(false);
			go.transform.position 			= position;
			go.rigidbody.velocity			= Vector3.zero;
			go.rigidbody.angularVelocity	= Vector3.zero;
		}
	}
	
	// "Destroys" an object by deactivating it and tucking it away offscreen.
	static public void Destroy(GameObject go)
	{
		go.SetActive(false);
		go.transform.position 			= position;
		go.rigidbody.velocity			= Vector3.zero;
		go.rigidbody.angularVelocity	= Vector3.zero;
	}
	
	static public GameObject Get(int i)
	{
		return pooledObjects[i];
	}
	
	static public int GetStatic()
	{
		return instance.staticPool.Length;
	}
}
