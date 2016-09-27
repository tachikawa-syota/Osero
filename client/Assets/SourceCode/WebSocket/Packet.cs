using UnityEngine;
using System;
using System.Collections;

// パケットクラス
[Serializable]
public class Packet
{
	[SerializeField]
	public string method;

	[SerializeField]
	public Payload payload = new Payload();

	[SerializeField]
	public Vector2 keyPos = new Vector2 ();
}

[Serializable]
public class Payload
{
	[SerializeField]
	public float x;

	[SerializeField]
	public float y;

	[SerializeField]
	public string name;
}
