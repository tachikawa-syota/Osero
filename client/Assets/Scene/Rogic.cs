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
	private int m_pieceType = 0;
    private int m_whiteStone = 0;
	private int m_blackStone = 0;
    private string m_gameState = "play";

	// 初期座標
	private Vector2 initPiece1 = new Vector2 (3, 3);
	private Vector2 initPiece2 = new Vector2 (3, 4);
	private Vector2 initPiece3= new Vector2 (4, 3);
	private Vector2 initPiece4 = new Vector2 (4, 4);

	public GUIStyle m_labelStyleScore;
	public GUIStyle m_labelStylePieceType;
	public GUIStyle m_labelStyleGameOver;

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
		m_pieceType = 2;
		putPiece(initPiece1);
		m_pieceType = 1; 
		putPiece(initPiece2);
		m_pieceType = 1; 
		putPiece(initPiece3);
		m_pieceType = 2; 
		putPiece(initPiece4);
	
    }

    void Update()
	{

	//	renderUI.OnGUI (m_whiteStone, m_blackStone, m_pieceType, m_gameState);
    }

	bool flag=false;
	public bool fromServer()
	{
		return flag;
	}

	public void SetFlag(bool bFlag)
	{
		flag = bFlag;
	}


    // 石を板に置く
	public int putPiece(Vector2 key)
	{
		// 座標補正
		if (key.x > 7) {
			key.x = 7;
		} else if (key.x < 0) {
			key.x = 0;
		} else if (key.y < 0) {
			key.y = 0;
		} else if (key.y > 7) {
			key.y = 7;
		}

		// 範囲外
        if (key.x < 0 || key.y < 0 || key.x > 7 || key.y > 7)
        {
			return -1;
        }
		// すでに盤にコマが置かれている
		if (board[(int)key.x,(int)key.y] != 0)
        {
			return -1;
        }

		// 石の種類セット
		board[(int)key.x,(int)key.y] = m_pieceType;
		// めくりフラグ
        bool changeFlag = updateBoard(key, true);
		// サーバーからきたデータはFlagをONにする
		if (fromServer ()) {
			changeFlag = true;
			flag = false;
		}

        // 初期座標
        bool initialFlag = false;
		if (key == initPiece1 || key == initPiece2 || key == initPiece3 || key == initPiece4)
        {
            initialFlag = true;
        }
		if (changeFlag || initialFlag)
        {
            calcStatus();
			Quaternion rotation = new Quaternion (0.0f, 0.0f, 0.0f, 1.0f);
			Vector3 position = new Vector3(key.x + 0.5f, 1, key.y + 0.5f);
    
			// 石の種類によって向きを変える
			if (m_pieceType == 1)
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
			m_pieceType = m_pieceType == 1 ? 2 : 1;

            // 置くところが無いかチェック
            if (!checkEnablePut() && !initialFlag)
            {
				m_pieceType = m_pieceType == 1 ? 2 : 1;
                // 攻守交代2回してどこも置けなかったらそのゲームは終了
                if (!checkEnablePut() && !initialFlag)
                {
                    Debug.Log("game over");
					m_gameState = "gameover";

		//			ActionButton actionButton = new ActionButton ();
		//			actionButton.Scene ();
				}
            }
        }
        else
        {
            Debug.Log("cannot put here");
			board [(int)key.x, (int)key.y] = 0;
        }
			
		return 0;
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
			if (board[ix,iy] > 0 && board[ix,iy] != m_pieceType)
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
		m_whiteStone = _white;
        m_blackStone = _black;
    }

	// UIの描画(ここにRogic内で記述しないとエラーが出る)
	void OnGUI()
	{
		Rect rectScore = new Rect (0, 0, Screen.width, Screen.height);

		GUI.Label (rectScore, "WHITE:" + m_whiteStone + "\nBLACK:" + m_blackStone, m_labelStyleScore);
		Rect rectPiece = new Rect (0, 100, Screen.width, Screen.height);
		string piece = m_pieceType == 1 ? "black" : "white";

		GUI.Label (rectPiece, piece, m_labelStylePieceType);

		Rect rect_gameover = new Rect (0, Screen.height / 2 - 25, Screen.width, 50);

		if (m_gameState == "gameover") {
			string result = "";
			if (m_whiteStone > m_blackStone) {
				result = "white win!";
			} else if (m_whiteStone < m_blackStone) {
				result = "black win!";
			} else {
				result = "draw...";
			}

			GUI.Label (rect_gameover, result, m_labelStyleGameOver);

			int halfWidth = Screen.width / 2;
			int halfHeight = Screen.height / 2;
		
			// ボタンが入力されたら
			if (GUI.Button (new Rect (halfWidth - 70, halfHeight - 30, 70, 30), "OK"))
			{
				ChangeScene changeScene = new ChangeScene ();
				changeScene.Run ();
			}

		}
	}
};

