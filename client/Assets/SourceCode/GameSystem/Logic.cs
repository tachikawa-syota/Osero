using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
// Jsonを使えるようにする
using Newtonsoft.Json;

public class Logic : MonoBehaviour
{
    // ゲームの状態
    public enum GameState
    {
        // ゲーム中
        Play,
        // ゲームオーバー
        GameOver
    }

    // 石の種類
    enum StoneType : byte
    {
        // 無し
        None = 0,
        // 黒石
        Black,
        // 白石
        White
    }

    // エラーの種類
    enum ErrorType : byte
    {
        // 該当無し
        None,
        // 範囲外で置けない
        OutOfRange,
        // 既に石が置かれている
        AlreadyExists
    }

    // ゲームオブジェクト
    public GameObject piecePrefab;

    // 盤
    private StoneType[,] board = new StoneType[8, 8];

    // 石のリスト
    private GameObject[,] pieceList = new GameObject[8, 8];
    // 石の種類
    private StoneType m_pieceType = StoneType.None;
    private int m_whiteStone = 0;
    private int m_blackStone = 0;
    // ゲームの状態
    private GameState m_gameState = GameState.Play;
    // エラー
    private ErrorType m_error = ErrorType.None;

    // 初期座標
    private Vector2 initPiece1 = new Vector2(3, 3);
    private Vector2 initPiece2 = new Vector2(3, 4);
    private Vector2 initPiece3 = new Vector2(4, 3);
    private Vector2 initPiece4 = new Vector2(4, 4);

    // エラーログのラベルスタイル
    private GUIStyle m_styleErrorLog = new GUIStyle();
    // 黒石のスコアラベルスタイル
    private GUIStyle m_styleScoreBlack = new GUIStyle();
    // 白石のスコアラベルスタイル
    private GUIStyle m_styleScoreWhite = new GUIStyle();
    // 手番ラベルスタイル
    private GUIStyle m_stylePieceType = new GUIStyle();
    // Next isフォントラベルスタイル
    private GUIStyle m_styleNext = new GUIStyle();
    // ゲームオーバーラベルスタイル
    private GUIStyle m_styleGameOver = new GUIStyle();
    // 白色
    private GUIStyleState m_textWhite = new GUIStyleState();
    // 黒色
    private GUIStyleState m_textBlack = new GUIStyleState();
    // 赤色
    private GUIStyleState m_textRed = new GUIStyleState();
    // 緑色
    private GUIStyleState m_textGreen = new GUIStyleState();

    // 初期化
    void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = StoneType.None;
            }
        }

        // 座標の生成
        Vector2 initPiece1 = new Vector2(3, 3);
        Vector2 initPiece2 = new Vector2(3, 4);
        Vector2 initPiece3 = new Vector2(4, 3);
        Vector2 initPiece4 = new Vector2(4, 4);

        // 石の生成
        m_pieceType = StoneType.Black;
        PutPiece(initPiece1);
        m_pieceType = StoneType.White;
        PutPiece(initPiece2);
        m_pieceType = StoneType.White;
        PutPiece(initPiece3);
        m_pieceType = StoneType.Black;
        PutPiece(initPiece4);


        // GUIフォントの設定 ---------------------------------------

        m_styleErrorLog.fontSize = 35;
        m_styleGameOver.fontSize = 50;
        m_styleNext.fontSize = 24;
        m_stylePieceType.fontSize = 26;
        m_styleScoreBlack.fontSize = 28;
        m_styleScoreWhite.fontSize = 28;
        // テキストカラー(白)
        m_textWhite.textColor = new Color(1.0f, 1.0f, 1.0f);
        // テキストカラー(赤)
        m_textRed.textColor = new Color(1.0f, 0.0f, 0.0f);
        // テキストカラー(緑)
        m_textGreen.textColor = new Color(0.0f, 1.0f, 0.0f);

        m_styleNext.normal = m_textGreen;
        m_styleGameOver.normal = m_textRed;
        m_styleScoreWhite.normal = m_textWhite;
        m_styleErrorLog.normal = m_textRed;
        
        // ここまで--------------------------------------------------
    }


    // 石を板に置く
    public void PutPiece(Vector2 key)
    {
        m_error = ErrorType.None;

        // 座標補正
        if (key.x > 7)
        {
            key.x = 7;
        }
        else if (key.x < 0)
        {
            key.x = 0;
        }
        else if (key.y < 0)
        {
            key.y = 0;
        }
        else if (key.y > 7)
        {
            key.y = 7;
        }

        // 範囲外
        if (key.x < 0 || key.y < 0 || key.x > 7 || key.y > 7)
        {
            m_error = ErrorType.OutOfRange;
            return;
        }
        // すでに盤にコマが置かれている
        if (board[(int)key.x, (int)key.y] != StoneType.None)
        {
            m_error = ErrorType.AlreadyExists;
            return;
        }

        // 石の種類セット
        board[(int)key.x, (int)key.y] = m_pieceType;
        // めくりフラグ
        bool changeFlag = updateBoard(key, true);

        // 初期座標
        bool initialFlag = false;
        if (key == initPiece1 || key == initPiece2 || key == initPiece3 || key == initPiece4)
        {
            initialFlag = true;
        }

        // 初期化時またはめくる時だけ
        if (changeFlag || initialFlag)
        {
            calcStatus();
            // 向き
            Quaternion rotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
            // 座標
            Vector3 position = new Vector3(key.x + 0.5f, 1, key.y + 0.5f);

            // 石の種類によって向きを変える
            if (m_pieceType == StoneType.White)
            {
                rotation = Quaternion.AngleAxis(180, new Vector3(1, 0, 0));
            }
            else
            {
                rotation = Quaternion.AngleAxis(0, new Vector3(1, 0, 0));
            }

            // 石の生成
            pieceList[(int)key.x, (int)key.y] = (GameObject)Instantiate(piecePrefab, position, rotation);

            // 置く石の種類
            m_pieceType = m_pieceType == StoneType.White ? StoneType.Black : StoneType.White;

            // 置くところが無いかチェック
            if (!checkEnablePut() && !initialFlag)
            {
                m_pieceType = m_pieceType == StoneType.White ? StoneType.Black : StoneType.White;
                // 攻守交代2回してどこも置けなかったらそのゲームは終了
                if (!checkEnablePut() && !initialFlag)
                {
                    Debug.Log("game over");
                    m_gameState = GameState.GameOver;
                }
            }
        }
        else
        {
            m_error = ErrorType.OutOfRange;
            board[(int)key.x, (int)key.y] = StoneType.None;
        }
    }

    // 置ける場所があるかどうか検索
    bool checkEnablePut()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] == StoneType.None && updateBoard(new Vector2(x, y), false))
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
        // ひっくり返すデータリスト
        ArrayList[] revList = new ArrayList[8];

        bool changeFlag = false;
        // horizon
        ix = (int)key.x + 1; 
        iy = (int)key.y;

        revList[0] = new ArrayList();
        while (true)
        {
            if (ix >= 8)
            {
                revList[0].Clear();
                break;
            }
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[0].Add(new Vector2(ix, iy));
            }
            else if (revList[0].Count > 0 && board[ix, iy] != 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[1].Add(new Vector2(ix, iy));
            }
            else if (revList[1].Count > 0 && board[ix, iy] != 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[2].Add(new Vector2(ix, iy));
            }
            else if (revList[2].Count > 0 && board[ix, iy] != 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[3].Add(new Vector2(ix, iy));
            }
            else if (revList[3].Count > 0 && board[ix, iy] != 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[4].Add(new Vector2(ix, iy));
            }
            else if (revList[4].Count > 0 && board[ix, iy] > 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[5].Add(new Vector2(ix, iy));
            }
            else if (revList[5].Count > 0 && board[(int)ix, (int)iy] > 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[6].Add(new Vector2(ix, iy));
            }
            else if (revList[6].Count > 0 && board[ix, iy] > 0)
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
            if (board[ix, iy] > 0 && board[ix, iy] != m_pieceType)
            {
                revList[7].Add(new Vector2(ix, iy));
            }
            else if (revList[7].Count > 0 && board[ix, iy] > 0)
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
                        board[(int)v.x, (int)v.y] = (board[(int)v.x, (int)v.y] == StoneType.White) ? StoneType.Black : StoneType.White;
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
                if (board[x, y] == StoneType.White)
                {
                    _black += 1;
                }
                else if (board[x, y] == StoneType.Black)
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
        // 黒石の数のラベル範囲
        Rect rectBlack = new Rect(0, 0, Screen.width, Screen.height);  
        // 白石の数のラベル範囲
        Rect rectWhite = new Rect(0, 40, Screen.width, Screen.height);
        Rect nextRect = new Rect(0, 120, Screen.width, Screen.height);
        Rect rectPiece = new Rect(0, 150, Screen.width, Screen.height);
        Rect errorRect = new Rect(Screen.width / 2 - 300, 0, 500, 500);
  
        string stone = m_pieceType == StoneType.White ? "black" : "white";

        // 手番が白ならフォントを白くする
        if(stone == "white")
        {
            m_stylePieceType.normal = m_textWhite;
        }
        // 黒くする
        else
        {
            m_stylePieceType.normal = m_textBlack;
        }

        // エラーログ
        string errorLog = " ";
        switch (m_error)
        {
            case ErrorType.None:
                // 何もしない
                break;

            case ErrorType.OutOfRange:
                errorLog = "その場所に石を置くことはできません";
                break;

            case ErrorType.AlreadyExists:
                errorLog = "既に石が置かれています";
                break;
        }

        // 描画
        GUI.Label(rectWhite, "WHITE:" + m_whiteStone, m_styleScoreWhite);
        GUI.Label(rectBlack, "BLACK:" + m_blackStone, m_styleScoreBlack);
        GUI.Label(nextRect, "Next is...", m_styleNext);
        GUI.Label(rectPiece, stone, m_stylePieceType);
        GUI.Label(errorRect, errorLog, m_styleErrorLog);

        // ゲームオーバー以外なら処理をしない
        if (m_gameState != GameState.GameOver)
        {
            return;
        }

        string result = "";

        if (m_whiteStone > m_blackStone)
        {
            result = "white win!";
        }
        else if (m_whiteStone < m_blackStone)
        {
            result = "black win!";
        }
        else {
            result = "draw...";
        }

        // ウィンドウサイズ半分
        int halfWidth = Screen.width / 2;
        int halfHeight = Screen.height / 2;

        // リザルトの描画
        GUI.Label(new Rect(halfWidth, 0, 200.0f, 200.0f), result, m_styleGameOver);

        // ボタンが入力されたら
        if (GUI.Button(new Rect(halfWidth - 70, halfHeight - 30, 70, 30), "OK"))
        {
            ChangeScene changeScene = new ChangeScene();
            changeScene.Run();
        }
    }
};