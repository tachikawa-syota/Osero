using UnityEngine;
using System.Collections;

// ゲームメインクラス
public class GameMain : MonoBehaviour
{
	// 初期化
	void Start () {
        // メイン
        SoundManager sound = gameObject.GetComponent<SoundManager>();
        sound.Play(SoundManager.SoundData.MainBGM);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
