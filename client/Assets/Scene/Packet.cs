using UnityEngine;
using System;
using System.Collections;

// パケットクラス
[Serializable]
public class Packet
{
	// 入力されたキー座標
	[SerializeField]
	public string method;

	[SerializeField]
	public Payload payload = new Payload();

	public Vector2 keyPos = new Vector2 ();
	// Use this for initialization
	void Start () {

		/*
		keyPos= new Vector2(0,0);
		ping = "ping";
		method = "ping";
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

[Serializable]
public class Payload
{
	[SerializeField]
	public string name;
}
