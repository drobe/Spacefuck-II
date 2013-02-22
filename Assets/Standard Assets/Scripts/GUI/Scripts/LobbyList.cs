using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyList : CustomGUI
{
	static private LobbyList instance	= null;
	
	[SerializeField]
	private bool title					= true;
	[SerializeField]
	private float titleHeight			= 20f;
	[SerializeField]
	private string titleLabel			= "Server List";
	
	[SerializeField]
	private float buttonScale			= 0.25f;
	[SerializeField]
	private string buttonLabel			= "Join";
	
	public GUISkin skin 				= null;
	[SerializeField]
	private bool[] normalizedPOS		= new bool[2];
	[SerializeField]
	private bool[] inverseOrigin		= new bool[2];
	[SerializeField]
	private bool[] normalizedDimension	= new bool[2];
	[SerializeField]
	private float entryHeight			= 20f;
	[SerializeField]
	private float entrySpacing			= 2f;
	[SerializeField]
	private float scrollBarWidth		= 16f; 
	public Rect rect					= new Rect(0,0,0,0);
	private Rect _rect					= new Rect(0,0,0,0);
	
	[SerializeField]
	private LobbyListItem[] items= new LobbyListItem[0];
	[SerializeField]
	private float pruningAge 			= 10.0f;
	[SerializeField]
	private float pruneCycle 			= 1.0f;
	private float _nextPrune			= 0.0f;
	static private float NOW			= 0;
	
	
	private Vector2 scrollPosition		= new Vector2(0,0);
	
	static private bool TooOld(LobbyListItem a)
	{
		return (NOW > a.age);
	}
	
	void Awake()
	{
		instance = this;
	}
	
	void Update()
	{
		NOW = Time.realtimeSinceStartup;
		if (Time.realtimeSinceStartup > _nextPrune)
		{
			_nextPrune = NOW + pruneCycle;
			
			List<LobbyListItem> pruneList = new List<LobbyListItem>(items);
			pruneList.RemoveAll(TooOld);
			items = pruneList.ToArray();
		}
	}
	
	public static void Add(string s, int slotsUsed, int slotsTotal)
	{
		foreach (LobbyListItem l in instance.items)
		{
			if (l.s == s)
			{
				l.ss = slotsUsed.ToString() + "/" + slotsTotal.ToString();
				l.age = NOW + instance.pruningAge;
				return;
			}
		}
		List<LobbyListItem> itemL = new List<LobbyListItem>(instance.items);
		
		LobbyListItem item = new LobbyListItem();
		item.s = s;
		item.ss = slotsUsed.ToString() + "/" + slotsTotal.ToString();
		item.age = NOW + instance.pruningAge;
		
		itemL.Add(item);
		instance.items = itemL.ToArray();
	}
	
	void OnGUI()
	{
		GUI.skin = skin;
		
		int[] d = new int[]{Screen.width,Screen.height};
		int[] center = new int[]{0,0};
		
		if (inverseOrigin[0])
		{
			center[0] = d[0];
		}
		if (inverseOrigin[1])
		{
			center[1] = d[1];
		}
		_rect = new Rect(center[0] + rect.x,center[1] + rect.y,rect.width,rect.height);
		
		if (normalizedPOS[0])
		{
			_rect.x = center[0] + (rect.x * d[0]);
		}
		if (normalizedPOS[1])
		{
			_rect.y = center[1] + (rect.y * d[1]);
		}
		
		if (normalizedDimension[0])
		{
			_rect.width = rect.width * d[0];
		}
		if (normalizedDimension[1])
		{
			_rect.height= rect.height * d[1];
		}
		if (_rect.height < 0)
		{
			_rect.height = d[1] + rect.height;
		}
	
		
		GUI.Box(_rect,"");
		
		if (title)
		{
			Rect titleRect = _rect;
			titleRect.height = titleHeight;
			GUI.Label(titleRect,titleLabel);
			
			_rect.y 			+= titleHeight;
			_rect.height 		-= titleHeight;
		}
		float yModifier 	= 0;
		Rect viewRect 		= _rect;
		viewRect.width 		-= scrollBarWidth;
		viewRect.height		= Mathf.Max(_rect.height, (items.Length * entryHeight) +  ((items.Length -1) * entrySpacing));
		
		scrollPosition 		= GUI.BeginScrollView(_rect,scrollPosition,viewRect,false, true);
		
		foreach(LobbyListItem item in items)
		{
			if (!item.valid)
			{
				Rect tempRect 	= _rect;
				tempRect.width 	= (tempRect.width * (1.0f - buttonScale)) - scrollBarWidth;
				tempRect.height = entryHeight;
				tempRect.y 		+= yModifier;
				item.rect1 		= tempRect;
				
				tempRect 		= _rect;
				tempRect.x 		+= (tempRect.width * (1.0f - buttonScale))  - scrollBarWidth;
				tempRect.y 		+= yModifier;
				tempRect.width *= buttonScale;
				tempRect.height = entryHeight;
				item.rect2 		= tempRect;
				
				item.valid = true;
			}
			GUI.Label(item.rect1,item.Get());
			if (GUI.Button (item.rect2, buttonLabel))
			{
				Spacefuck.InputManager.NetPrep();
				Network.Connect(item.s,6666);
			}
			yModifier += entryHeight + entrySpacing;
		}
		GUI.EndScrollView();
	}
}
