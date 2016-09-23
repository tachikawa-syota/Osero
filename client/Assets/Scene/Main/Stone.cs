using UnityEngine;
using System.Collections;
// Jsonを使えるようにする
using Newtonsoft.Json;

// 石を置くクラス
public class Stone : MonoBehaviour 
{
	// 更新
	void  Update ()
	{
		// キー入力
		if (Input.GetButtonDown("Fire1")) 
		{
			// ワールド変換
			Vector3 screenPoint= Input.mousePosition;      
			screenPoint.z = 10;
			Vector3 v= Camera.main.ScreenToWorldPoint(screenPoint);
			float key_x= Mathf.Floor(v.x);
			float key_y= Mathf.Floor(v.z);

			Vector2 debugKey;
			debugKey.x = key_x;
			debugKey.y = key_y;
			// デバッグログで確認
			Debug.Log (debugKey);

			// パケットにキー座標情報をセット
			Packet data = new Packet();
			data.keyPos.x = key_x;
			data.keyPos.y = key_y;

			// ソケットクライアントに座標データを転送
			GameObject.FindWithTag("WebSocketClient").SendMessage("SendData",new Vector2(key_x, key_y));
			// 盤上にコマを置く
			GameObject.FindWithTag("GameController").SendMessage("putPiece", new Vector2(key_x, key_y));
		}
	}
}