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

    [SerializeField]
	public Vector2 keyPos = new Vector2 ();

	// Use this for initialization
	void Start () {

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
