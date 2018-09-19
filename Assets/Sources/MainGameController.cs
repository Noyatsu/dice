using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : MonoBehaviour
{

    public int[,] board = new int[10, 10]; //!<さいころのIDを格納
    public int[,] board_num = new int[10, 10]; //!< さいころの面を格納
    List<GameObject> dices = new List<GameObject>(); //!< さいころオブジェクト格納用リスト
    double timeElapsed = 0.0; //!< イベント用フレームカウント
    int maxDiceId = 0; //!< 現在のさいころIDの最大値
    public bool isRotate_dice = false; //!< さいころが回転中かどうか
    public bool isRotate_charactor = false; //!< キャラクターが移動中かどうか

    GameObject Dice, Aqui;
    AquiController objAquiController;
    DiceController objDiceController;

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

        //初期用配列設定
        board[0, 0] = maxDiceId;
        board_num[0, 0] = 1;
        
        Dice = GameObject.Find("Dice");
        dices.Add(Dice);  //リストにオブジェクトを追加

        Aqui = GameObject.Find("Aqui");
        objAquiController = Aqui.GetComponent<AquiController>();
        objDiceController = Dice.GetComponent<DiceController>();

        //さいころをいくつか追加
        for(int i = 0; i < 10; i++)
        {
            randomDiceGenerate();
        }

    }

    // Update is called once per frame
    void Update()
    {
        // キー入力一括制御
        if (isRotate_dice == false && isRotate_charactor == false)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                objAquiController.SetTargetPosition(2);
                objDiceController.SetTargetPosition(2);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                objAquiController.SetTargetPosition(0);
                objDiceController.SetTargetPosition(0);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                objAquiController.SetTargetPosition(1);
                objDiceController.SetTargetPosition(1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                objAquiController.SetTargetPosition(3);
                objDiceController.SetTargetPosition(3);
            }
            //動いたときにダイスが変わる場合
            if (objAquiController.x != objDiceController.X || objAquiController.z != objDiceController.Z)
            {
                objDiceController.isSelected = false; //選択解除
                Dice = dices[board[objAquiController.x, objAquiController.z]];
                Debug.Log(board[objAquiController.x, objAquiController.z]);
                objDiceController = Dice.GetComponent<DiceController>();
                objDiceController.isSelected = true; //選択
            }
        }

        timeElapsed += Time.deltaTime;

        // 5秒ごとにさいころ追加
        if (timeElapsed >= 5.0)
        {
            randomDiceGenerate();
            timeElapsed = 0.0f;
        }
    }

    void randomDiceGenerate()
    {
        // 配置する座標を決定
        int x = Random.Range(0, 10);
        int z = Random.Range(0, 10);

        // 配置する面を決定
        int a = Random.Range(1, 6);
        int b = 0;
        int i = Random.Range(1, 4);

        //面によって回転角度を決定
        int xi = 0, yi = 0, zi = 0, ra = 90;
        switch (a)
        {
            case 1:
                int[] num1 = { 2, 3, 4, 5 };
                b = num1[i];
                switch (b)
                {
                    case 2:
                        break;
                    case 3:
                        yi = ra;
                        break;
                    case 4:
                        yi = ra * 3;
                        break;
                    case 5:
                        yi = ra * 2;
                        break;
                }
                break;

            case 2:
                int[] num2 = { 1, 3, 4, 6 };
                b = num2[i];
                switch (b)
                {
                    case 1:
                        xi = ra;
                        yi = ra * 2;
                        break;
                    case 3:
                        xi = ra;
                        yi = ra;
                        break;
                    case 4:
                        xi = ra;
                        yi = ra * 3;
                        break;
                    case 6:
                        xi = ra;
                        break;
                }
                break;

            case 3:
                int[] num3 = { 1, 2, 5, 6 };
                b = num3[i];
                switch (b)
                {
                    case 1:
                        zi = ra;
                        yi = ra * 3;
                        break;
                    case 2:
                        zi = ra;
                        break;
                    case 5:
                        zi = ra;
                        yi = ra * 2;
                        break;
                    case 6:
                        zi = ra;
                        yi = ra;
                        break;
                }
                break;
            case 4:
                int[] num4 = { 1, 2, 5, 6 };
                b = num4[i];
                switch (b)
                {
                    case 1:
                        zi = ra * 3;
                        yi = ra * 1;
                        break;
                    case 2:
                        zi = ra * 3;
                        break;
                    case 5:
                        zi = ra * 3;
                        yi = ra * 2;
                        break;
                    case 6:
                        zi = ra * 3;
                        yi = ra * 3;
                        break;
                }
                break;

            case 5:
                int[] num5 = { 1, 3, 4, 6 };
                b = num5[i];
                switch (b)
                {
                    case 1:
                        xi = ra * 3;
                        break;
                    case 3:
                        xi = ra * 3;
                        yi = ra;
                        break;
                    case 4:
                        xi = ra * 3;
                        yi = ra * 3;
                        break;
                    case 6:
                        xi = ra * 3;
                        yi = ra * 2;
                        break;
                }
                break;
            case 6:
                int[] num6 = { 2, 3, 4, 5 };
                b = num6[i];
                switch (b)
                {
                    case 2:
                        xi = ra * 2;
                        yi = ra * 2;
                        break;
                    case 3:
                        xi = ra * 2;
                        yi = ra;
                        break;
                    case 4:
                        xi = ra * 2;
                        yi = ra * 3;
                        break;
                    case 5:
                        xi = ra * 2;
                        break;
                }
                break;
        }

        // その座標が空だったらさいころを追加
        if (board[x, z] == -1)
        {
            maxDiceId++;
            board[x, z] = maxDiceId;
            Vector3 position = new Vector3(-4.5f + (float)x, -0.5f, -4.5f + (float)z); //位置
            GameObject objDice = (GameObject)Instantiate(Dice, position, Quaternion.Euler(xi, yi, zi));
            DiceController objDiceController = objDice.GetComponent<DiceController>();
            objDiceController.isSelected = false;
            objDiceController.X = x;
            objDiceController.Z = z;
            objDiceController.surfaceA = a;
            objDiceController.surfaceB = b;
            objDiceController.diceId = maxDiceId;
            dices.Add(objDice); //リストにオブジェクトを追加
            Debug.Log(objDiceController.surfaceA);
            Debug.Log(objDiceController.surfaceB);
            board_num[x, z] = a;
            StartCoroutine(RisingDice(objDice));
        }

    }

    IEnumerator RisingDice(GameObject dc) {
        Vector3 position = dc.transform.position;
        for (int i = 1; i < 21; i++) {
            position.y = -0.5f + i * 1f / 20f;
            dc.transform.position = position;
            yield return null;
        }
        yield break;
    }
}
