using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : MonoBehaviour
{

    public int[,] board = new int[10, 10]; //!<さいころのIDを格納
    public int[,] board_num = new int[10, 10]; //!< さいころの面を格納
    double timeElapsed = 0.0; //!< イベント用フレームカウント
    int maxDiceId = 0; //!< 現在のさいころIDの最大値
    public bool isRotate = false; //!< さいころが回転中かどうか

    GameObject Dice;

    // Use this for initialization
    void Start()
    {
        //配列の初期化
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                board[i, j] = -1;
                board_num[i, j] = -1;
            }
        }

        //以下仮
        board[0, 0] = maxDiceId;
        board_num[0, 0] = 1;
        maxDiceId++;
        board[4, 4] = maxDiceId;
        board_num[4, 4] = 1;
        
        Dice = GameObject.Find("Dice");
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        // 10秒ごとにさいころ追加
        if (timeElapsed >= 5.0)
        {
            // 配置する座標を決定
            int x = Random.Range(1, 7);
            int z = Random.Range(1, 7);

            // その座標が空だったらさいころを追加
            if (board[x, z] == -1)
            {
                Vector3 position = new Vector3(-4.5f + (float)x, 0.5f, -4.5f + (float)z); //位置
                GameObject objDice = (GameObject)Instantiate(Dice, position, transform.rotation);
                DiceController objDiceController = objDice.GetComponent<DiceController>();
                objDiceController.isSelected = false;
                objDiceController.X = x;
                objDiceController.Z = z;
                maxDiceId++;
                board[x, z] = maxDiceId;
                board_num[x, z] = 1;
            }

            timeElapsed = 0.0f;
        }
    }
}
