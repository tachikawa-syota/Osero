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
        // 右クリックが入力されたら
		if (Input.GetButtonDown("Fire1")) 
		{
            // ワールド変換
			Vector3 screenPoint= Input.mousePosition;      
			screenPoint.z = 10;
			Vector3 v= Camera.main.ScreenToWorldPoint(screenPoint);
			float key_x= Mathf.Floor(v.x);
			float key_y= Mathf.Floor(v.z);

			// パケットにキー座標情報をセット
			Packet data = new Packet();
			data.keyPos.x = key_x;
			data.keyPos.y = key_y;

            // ウェブソケットのインスタンス作成
			WebSocketClient ws = new WebSocketClient ();
            // データをサーバーに転送
			ws.SendData ();

            // GameControllerの"putPiece"関数を呼び出す
			GameObject.FindWithTag("GameController").SendMessage("putPiece", new Vector2(key_x, key_y));
		}
	}
}