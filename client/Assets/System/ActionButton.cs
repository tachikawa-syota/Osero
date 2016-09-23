using UnityEngine;

// ボタンアクション
public class ActionButton : MonoBehaviour {

	// シーンを切り替える
	public void Scene()
	{
		ChangeScene changeScene = new ChangeScene ();
		changeScene.Run ();
	}

	// アプリケーションの終了
	public void Exit()
	{
		Application.Quit ();
	}
}
