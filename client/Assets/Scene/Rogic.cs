using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
// Jsonを使えるようにする
using Newtonsoft.Json;

public class Rogic : MonoBehaviour
{
	// ゲームオブジェクト
	public GameObject piecePrefab;

	// 盤
	private int[,] board = new int[8,8];
	   
    // 石のリスト
	private GameObject[,] pieceList = new GameObject[8, 8];
	// 石の種類
	private int pieceType = 0;
    private int white = 0;
    private int black = 0;
    private string gamestatus = "play";
    public GUIStyle labelStyleScore;
	public GUIStyle labelStylePieceType;
	public GUIStyle labelStyleGameOver;

	// 初期座標
	private Vector2 initPiece1 = new Vector2 (3, 3);
	private Vector2 initPiece2 = new Vector2 (3, 4);
	private Vector2 initPiece3= new Vector2 (4, 3);
	private Vector2 initPiece4 = new Vector2 (4, 4);

	// 初期化
    void Awake()
    {
		for(int i = 0;i<8;i++){
			for(int j=0;j<8;j++){
				board[i,j] = 0;
			}
		}

		// 座標の生成
		Vector2 initPiece1 = new Vector2 (3, 3);
		Vector2 initPiece2 = new Vector2 (3, 4);
		Vector2 initPiece3 = new Vector2 (4, 3);
		Vector2 initPiece4 = new Vector2 (4, 4);

		// 石の生成
		pieceType = 2;
		putPiece(initPiece1);
		pieceType = 1; 
		putPiece(initPiece2);
		pieceType = 1; 
		putPiece(initPiece3);
		pieceType = 2; 
		putPiece(initPiece4);
    }

    void Update()
    {

    }


    // 石を板に置く
	IEnumerator putPiece(Vector2 key)
	{
		Debug.Log (key);
		if (key.x == 8) {
			key.x = 0;
		} else if (key.x == -1) {
			key.x = 7;
		} else if (key.y == -1) {
			key.y = 0;
		} else if (key.y == 8) {
			key.y = 7;
		} else {
			yield return -1;
		}

        if (key.x < 0 || key.y < 0 || key.x > 7 || key.y > 7)
        {
			yield return -1;
        }
		if (board[(int)key.x,(int)key.y] != 0)
        {
			yield return -1;
        }

		// 石の種類セット
		board[(int)key.x,(int)key.y] = pieceType;
		// めくりフラグ
        bool changeFlag = updateBoard(key, true);

        // initial position
        bool initialFlag = false;
		if (key == initPiece1 || key == initPiece2 || key == initPiece3 || key == initPiece4)
        {
            initialFlag = true;
        }
        if (changeFlag || initialFlag)
        {
            calcStatus();
			Quaternion rotation = transform.rotation;
            Vector3 position = new Vector3(key.x + 0.5f, 1, key.y + 0.5f);
            if (pieceType == 1)
            {
				rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
            }
            else
            {
				rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
            }

            // 石の生成
			pieceList[(int)key.x,(int) key.y] = (GameObject)Instantiate(piecePrefab, position, rotation);

            // 置く石の種類
            pieceType = pieceType == 1 ? 2 : 1;

            // 置くところが無いかチェック
            if (!checkEnablePut() && !initialFlag)
            {
                pieceType = pieceType == 1 ? 2 : 1;
                // 攻守交代2回してどこも置けなかったらそのゲームは終了
                if (!checkEnablePut() && !initialFlag)
                {
                    Debug.Log("game over");
                    gamestatus = "gameover";
           //         yield return new WaitForSeconds(2.0f);
                    while (!Input.GetButtonDown("Fire1") || Input.touches.Length > 0) yield return 0;
//                    Application.LoadLevel("GameMain");
					SceneManager.LoadScene ("GameMain");
				}
            }
        }
        else
        {
            Debug.Log("cannot put here");
			board [(int)key.x, (int)key.y] = 0;
        }

        // for debug
        for (int i = 0; i < 8; i++)
        {
            string _s = "";
            for (int j = 0; j < 8; j++)
            {
                _s = _s + " " + board[i,j];
            }
//            Debug.Log(_s);
        }

		yield return 0;
    }

    // 置ける場所があるかどうか検索
    bool checkEnablePut()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x,y] == 0 && updateBoard(new Vector2(x, y), false))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 盤面の更新、updateFlag が false なら
    // その場所に置けるかどうかのチェックだけ。
    bool updateBoard(Vector2 key, bool updateFlag)
    {
        int ix = 0; int iy = 0;
        ArrayList[] revList = new ArrayList[8];
        
		bool changeFlag = false;
        // horizon
		ix = (int)key.x + 1; iy = (int)key.y;
        revList[0] = new ArrayList();
        while (true)
        {
            if (ix >= 8)
            {
                revList[0].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
				revList[0].Add(new Vector2(ix, iy));
            }
            else if (revList[0].Count > 0 && board[ix,iy] != 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[0].Clear();
                break;
            }
            ix += 1;
        }

		ix = (int)key.x - 1; iy = (int)key.y;
        revList[1] = new ArrayList();
        while (true)
        {
            if (ix < 0)
            {
                revList[1].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[1].Add(new Vector2(ix, iy));
            }
			else if (revList[1].Count > 0 && board[ix,iy] != 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[1].Clear();
                break;
            }
            ix -= 1;
        }

        // vertical
		ix = (int)key.x; iy = (int)key.y + 1;
        revList[2] = new ArrayList();
        while (true)
        {
            if (iy >= 8)
            {
                revList[2].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[2].Add(new Vector2(ix, iy));
            }
            else if (revList[2].Count > 0 && board[ix,iy] != 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[2].Clear();
                break;
            }
            iy += 1;
        }

		ix = (int)key.x; iy = (int)key.y - 1;
        revList[3] = new ArrayList();
        while (true)
        {
            if (iy < 0)
            {
                revList[3].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[3].Add(new Vector2(ix, iy));
            }
            else if (revList[3].Count > 0 && board[ix,iy] != 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[3].Clear();
                break;
            }
            iy -= 1;
        }

        // cross
		ix = (int)key.x + 1; iy = (int)key.y + 1;
        revList[4] = new ArrayList();
        while (true)
        {
            if (ix >= 8 || iy >= 8)
            {
                revList[4].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[4].Add(new Vector2(ix, iy));
            }
			else if (revList[4].Count > 0 && board[ix,iy] > 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[4].Clear();
                break;
            }
            iy += 1; ix += 1;
        }

        revList[5] = new ArrayList();
		ix = (int)key.x + 1; iy = (int)key.y - 1;
        while (true)
        {
            if (ix >= 8 || iy < 0)
            {
                revList[5].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[5].Add(new Vector2(ix, iy));
            }
			else if (revList[5].Count > 0 && board[(int)ix,(int)iy] > 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[5].Clear();
                break;
            }
            ix += 1; iy -= 1;
        }


        revList[6] = new ArrayList();
		ix = (int)key.x - 1; iy = (int)key.y + 1;
        while (true)
        {
            if (ix < 0 || iy >= 8)
            {
                revList[6].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[6].Add(new Vector2(ix, iy));
            }
            else if (revList[6].Count > 0 && board[ix,iy] > 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[6].Clear();
                break;
            }
            ix -= 1; iy += 1;
        }

        revList[7] = new ArrayList();
		ix = (int)key.x - 1; iy = (int)key.y - 1;
        while (true)
        {
            if (ix < 0 || iy < 0)
            {
                revList[7].Clear();
                break;
            }
            if (board[ix,iy] > 0 && board[ix,iy] != pieceType)
            {
                revList[7].Add(new Vector2(ix, iy));
            }
            else if (revList[7].Count > 0 && board[ix,iy] > 0)
            {
                changeFlag = true;
                break;
            }
            else
            {
                revList[7].Clear();
                break;
            }
            ix -= 1; iy -= 1;
        }

        if (changeFlag)
        {
            if (updateFlag)
            {
                foreach (ArrayList val in revList)
                {
                    foreach (Vector2 v in val)
                    {
						pieceList[(int)v.x, (int)v.y].transform.rotation *= Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
						board[(int)v.x,(int)v.y] = (board[(int)v.x,(int)v.y] == 1) ? 2 : 1;
                    }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    void calcStatus()
    {
        int _white = 0;
        int _black = 0;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x,y] == 1)
                {
                    _black += 1;
                }
                else if (board[x,y] == 2)
                {
                    _white += 1;
                }
            }
        }
        white = _white;
        black = _black;
    }

	// UIの描画
    void OnGUI()
    {
        Rect rect_score = new Rect(0, 0, Screen.width, Screen.height);
        GUI.Label(rect_score, "WHITE:" + white + "\nBLACK:" + black, labelStyleScore);

        Rect rect_piece = new Rect(0, 0, Screen.width, Screen.height);
		string piece = pieceType == 1 ? "black" : "white";
        GUI.Label(rect_piece, piece, labelStylePieceType);

        Rect rect_gameover = new Rect(0, Screen.height / 2 - 25, Screen.width, 50);
        if (gamestatus == "gameover")
        {
			string result = "";
            if (white > black)
            {
                result = "white win!";
            }
            else if (white < black)
            {
                result = "black win!";
            }
            else
            {
                result = "draw...";
            }
            GUI.Label(rect_gameover, result, labelStyleGameOver);
        }
	}
};