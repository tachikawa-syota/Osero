using UnityEngine;
using WebSocketSharp;
// Jsonを使えるようにする
using Newtonsoft.Json;

/**
* @brief WebSoketClient
*/
public class WebSocketClient : MonoBehaviour 
{
	// ウェブソケット
	private WebSocket ws;
	// パケット
	private Packet packet;

	void Start()
	{
		// 接続
		ws = new WebSocket("ws://127.0.0.1:25678");
		ws.Log.Level = LogLevel.Trace;
//		this.ws.OnMessage += (object sender, MessageEventArgs e) => {
//			print (e.Data);
//		};

		ws.OnOpen += (sender, e) =>
		{
			Debug.Log("opened.");
		};

		ws.OnMessage += (sender, e) =>
		{
			Debug.Log("onmessage.");
		};

		ws.OnError += (sender, e) =>
		{
			Debug.Log("error:" + e.Message);
		};

		ws.OnClose += (sender, e) =>
		{
			Debug.Log("closed." + e.Code.ToString() + e.Reason.ToString());
		};

		// コネクト
		ws.Connect ();
	}

	// サーバー側にデータを転送する
	public void SendData()
	{
		// パケットを取得
		Packet data = new Packet();
		//data.keyPos = new Vector2 (3, 3);
		// インスタンスを取得
		data.payload = new Payload();
		data.payload.name = "tachikawa";
		data.method = "ping";

		// シリアライズする
		string method = JsonUtility.ToJson(data);

		Debug.Log (method);
		// サーバーに送る
		ws.Send (method);
	}

	{
		mothod: "keypos",
		payload: {
			x: 10,
			y: 10,
		}
	}

	void OnGUI() {
		if (GUILayout.Button("Connect")) {
			ws = new WebSocket("ws://localhost:25678/");
			ws.OnMessage += (object sender, MessageEventArgs e) => {
			//	print (e.Data);
			};
			ws.Connect ();
		}


 //       if (GUILayout.Button("Send")) {
		if(Input.GetButtonDown("Fire2")){
			// パケットを取得
			Packet data = new Packet();
			//data.keyPos = new Vector2 (3, 3);
			// インスタンスを取得
			data.payload = new Payload();
			data.payload.name = "tachikawa";
			data.method = "ping";

			// シリアライズする
			string method = JsonUtility.ToJson(data);

			Debug.Log (method);
			// サーバーに送る
			ws.Send (method);
		}

		if (GUILayout.Button("Close")) {
			ws.Close ();
		}
	}
}