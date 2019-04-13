using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineGameController : MonoBehaviour
{
    MainGameController script;
    private PhotonView objPhotonViewControl;
    private AudioSource sound_enemy;
    public int enemyScore;

    public int boardSize = 7; //!< 盤面のサイズ
    public int[,] board = new int[7, 7]; //!< さいころのIDを格納
    public int[,] board_num = new int[7, 7]; //!< さいころの面を格納
    public List<GameObject> dices = new List<GameObject>(); //!< さいころオブジェクト格納用リスト
    int maxDiceId = 0; //!< 現在のさいころIDの最大値

    GameObject Dice, DiceBase, Aqui, VanishingDice, StatusText, ScreenText;
    public GameObject waitingPanel;

    void Start()
    {
        waitingPanel.SetActive(true);

        script = GameObject.Find("Board").GetComponent<MainGameController>();
        script.gameType = 1; // ゲームタイプを1に設定
        sound_enemy = GetComponent<AudioSource>();
        objPhotonViewControl = GetComponent<PhotonView>();

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

        DiceBase = (GameObject)Resources.Load("DiceE");
        Dice = GameObject.Find("DiceE");
        dices.Add(Dice);  //リストにオブジェクトを追加

        //Aqui = GameObject.Find("AquiE");
    }

    // Update is called once per frame
    void Update()
    {


    }


    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable changedProperties)
    {
        // ルームプロパティからsumScoreを取得
        object value = null;
        if (changedProperties.TryGetValue("sumScore", out value))
        {
            script.sumScore = (int)value;
            //レベルを計算
            script.ComputeLevel();
        }
    }

    public void sendScore(int score)
    {
        objPhotonViewControl.RPC("setEnemyScore", PhotonTargets.Others, score);
    }

    public void sendLose()
    {
        objPhotonViewControl.RPC("youWin", PhotonTargets.Others, 0);
    }

    public void sendPoint(int x, int z, int a, int b)
    {
        objPhotonViewControl.RPC("setEnemyBoard", PhotonTargets.Others, x, z, a, b);
    }
    public void sendSink(int x, int z)
    {
        objPhotonViewControl.RPC("setEnemyDiceSinking", PhotonTargets.Others, x, z);
    }

    public void sendVanish(int x, int z)
    {
        Debug.Log("RPCED(Vanish)");
        objPhotonViewControl.RPC("setEnemyDiceVanish", PhotonTargets.Others, x, z);

    }

    public void sendRoll(int x, int z, int d)
    {
        objPhotonViewControl.RPC("setEnemyDiceRoll", PhotonTargets.Others, x, z, d);
    }

    [PunRPC]
    private void setEnemyScore(int eScore)
    {
        enemyScore = eScore;
        sound_enemy.PlayOneShot(sound_enemy.clip);
    }

    [PunRPC]
    private void youWin(int data)
    {
        SceneManager.LoadScene("YouWin");
    }

    [PunRPC]
    private void setEnemyBoard(int x, int z, int a, int b)
    {
        diceGenerateE(x, z, a, b);
        if (Time.timeScale == 0f)
        {
            waitingPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    [PunRPC]
    private void setEnemyDiceSinking(int x, int z)
    {
        diceSinkingE(x, z);
    }

    [PunRPC]
    private void setEnemyDiceVanish(int x, int z)
    {
        diceVanishE(x, z);
    }

    [PunRPC]
    private void setEnemyDiceRoll(int x, int z, int d)
    {
        diceRollE(x, z, d);
    }
    // EnemyBoardControl

    /**
    * ダイスを生成
    * @params x 配置するx座標
    * @params z 配置するz座標
    * @params a 上にする面
    */
    public void diceGenerateE(int x, int z, int a, int b = 0)
    {
        // 側面の決定用乱数
        int i = Random.Range(1, 4);

        //面によって回転角度を決定
        int xi = 0, yi = 0, zi = 0, ra = 90;
        switch (a)
        {
            case 1:
                if (b == 0)
                {
                    int[] num1 = { 2, 3, 4, 5 };
                    b = num1[i];
                }
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
                if (b == 0)
                {
                    int[] num2 = { 1, 3, 4, 6 };
                    b = num2[i];
                }
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
                if (b == 0)
                {
                    int[] num3 = { 1, 2, 5, 6 };
                    b = num3[i];
                }
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
                if (b == 0)
                {
                    int[] num4 = { 1, 2, 5, 6 };
                    b = num4[i];
                }
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
                if (b == 0)
                {
                    int[] num5 = { 1, 3, 4, 6 };
                    b = num5[i];
                }
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
                if (b == 0)
                {
                    int[] num6 = { 2, 3, 4, 5 };
                    b = num6[i];
                }
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
            Vector3 position = new Vector3(2.5f + (float)x * 0.5f, 0.25f, 1.5f + (float)z * 0.5f); //位置
            GameObject objDice = (GameObject)Instantiate(DiceBase, position, Quaternion.Euler(xi, yi, zi));
            dices.Add(objDice); //リストにオブジェクトを追加
            board_num[x, z] = a;
        }

    }

    /**
     * ダイスを沈み状態にする
     */
    public void diceSinkingE(int x, int z)
    {
        script.ChangeColorOfGameObject(dices[board[x, z]], new Color(0.3f, 0.3f, 0.7f, 1.0f));
    }

    /**
     * ダイスを削除
     */
    public void diceVanishE(int x, int z)
    {
        int diceId = board[x, z];
        GameObject dice = dices[diceId];
        board[x, z] = -1;
        Destroy(dice);
    }

    /**
     * ダイスを回転
     */
    public void diceRollE(int x, int z, int d)
    {
        int diceId = board[x, z];
        GameObject dice = dices[diceId];
        Vector3 rotatePoint, rotateAxis;
        float diceSizeHalf = dice.transform.localScale.x / 2f;
        if (d == 2)
        {
            board[x + 1, z] = diceId;
            board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(diceSizeHalf, -diceSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, -1);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 0)
        {
            board[x - 1, z] = diceId;
            board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(-diceSizeHalf, -diceSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, 1);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 1)
        {
            board[x, z + 1] = diceId;
            board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(0f, -diceSizeHalf, diceSizeHalf);
            rotateAxis = new Vector3(1, 0, 0);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 3)
        {
            board[x, z - 1] = diceId;
            board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(0f, -diceSizeHalf, -diceSizeHalf);
            rotateAxis = new Vector3(-1, 0, 0);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
    }

    IEnumerator MoveDice(int id, Vector3 rotatePoint, Vector3 rotateAxis)
    {
        GameObject dice = dices[id];
        float diceAngle, sumAngle = 0f;
        while (sumAngle < 90f)
        {
            diceAngle = 18f * Time.deltaTime * 50f;
            sumAngle += diceAngle;

            if (sumAngle > 90f)
            {
                diceAngle -= sumAngle - 90f;
            }
            dice.transform.RotateAround(rotatePoint, rotateAxis, diceAngle);

            yield return null;
        }

        yield break;
    }

}