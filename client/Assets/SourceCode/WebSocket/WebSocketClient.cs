using UnityEngine;
using WebSocketSharp;
// Jsonを使えるようにする
using Newtonsoft.Json;

// ウェブソケットクライアント
public class WebSocketClient //: MonoBehaviour 
{
	// Json用文字列
	string strJson;

	// ウェブソケット
	private WebSocket ws;
	// パケット
	private Packet packet;
	// ソケットが開かれたかどうかのフラグ
	bool m_flag = false;

	// 初期化をする
	public void Initialize()
	{
		// 接続
		ws = new WebSocket("ws://192.168.56.101:5678");
		ws.Log.Level = LogLevel.Trace;

		// 開く
		ws.OnOpen += (sender, e) =>
		{
			Debug.Log("opened.");
			// フラグをONにする
			m_flag = true;
		};


		// サーバーからメッセージを取得
		ws.OnMessage += (sender, e) =>
		{
			// デバッグログ表示する
			Debug.Log("onmessage" + e.Data);
			// パケット作成
			Packet pack = new Packet();
			// デシリアライズ
			pack.payload = JsonUtility.FromJson<Payload>(e.Data);
			// 石を置く
			Logic r = new Logic();
			r.PutPiece(new Vector2(pack.payload.x, pack.payload.y));			
		};

		// エラー
		ws.OnError += (sender, e) =>
		{
			Debug.Log("error:" + e.Message);
		};

		// クローズ
		ws.OnClose += (sender, e) =>
		{
			Debug.Log("closed." + e.Code.ToString() + e.Reason.ToString());
		};

		// コネクト
		ws.Connect ();
	}


	// サーバー側にデータを転送する
	public void SendData(Vector2 pos)
	{
		// パケットを取得
		Packet data = new Packet();
		// インスタンスを取得
		data.payload = new Payload();
		data.payload.name = "tachikawa";
		data.method = "ping";

		// パケット作成
		Packet data2 = new Packet();
		data2.method = "keypos";
		data2.payload = new Payload ();
		// キー座標の登録
		data2.payload.x = pos.x;
		data2.payload.y = pos.y;
		data2.payload.name = "tachikawa";
		// シリアライズする
		string method = JsonUtility.ToJson(data);
		strJson = JsonUtility.ToJson(data2);

		// デバッグログを出す
		Debug.Log (method);
		Debug.Log (strJson);
		// サーバーに送る
		this.ws.Send (strJson);
	}

	// ソケットが開かれているかどうかのチェック
	public bool CheckSocket()
	{
		return m_flag;
	}
}