using UnityEngine;

// ボタンアクション
public class ActionButton : MonoBehaviour {

	// シーンを切り替える
	void Scene()
	{
		ChangeScene changeScene = new ChangeScene ();
		changeScene.Run ();
	}

	// "NormalPlay"ボタン選択
	public void NormalPlay()
	{
		// シーン切り替え
		Scene ();
	}

	// "OnlinePlay"ボタン選択
	public void OnlinePlay()
	{
		// ウェブソケットクライアントを起動する
		WebSocketClient socket = new WebSocketClient();
		socket.Initialize ();
		// シーン切り替え
		Scene ();
	}

	// アプリケーションの終了
	public void Exit()
	{
		Application.Quit ();
	}
}
