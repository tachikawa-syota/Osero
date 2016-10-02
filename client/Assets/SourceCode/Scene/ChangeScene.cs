using UnityEngine;
// シーンマネージャー
using UnityEngine.SceneManagement;

// シーン切り替えクラス
public class ChangeScene 
{
	// 実行
	public void Run()
	{
		// シーンの名前
		string sceneName = SceneManager.GetActiveScene ().name;

		switch (sceneName) {
		case "Title":
			SceneManager.LoadScene ("GameMain");
			break;

		case "GameMain":
			SceneManager.LoadScene ("Title");
			break;
		}
	}
}
