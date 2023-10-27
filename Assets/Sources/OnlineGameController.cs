using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class OnlineGameController : MonoBehaviour
{
    private MainGameController _script;
    private PhotonView _objPhotonViewControl;
    private AudioSource _soundEnemy;
    [FormerlySerializedAs("enemyScore")] public int EnemyScore;

    [FormerlySerializedAs("boardSize")] public int BoardSize = 7; //!< 盤面のサイズ
    public int[,] Board = new int[7, 7]; //!< さいころのIDを格納
    public int[,] BoardNum = new int[7, 7]; //!< さいころの面を格納
    [FormerlySerializedAs("dices")] public List<GameObject> Dices = new List<GameObject>(); //!< さいころオブジェクト格納用リスト
    private int _maxDiceId = 0; //!< 現在のさいころIDの最大値
    private int _damageDice = 0; //相手のスコアによって生成されたダイス数

    private GameObject _dice, _diceBase, _aqui, _vanishingDice, _statusText, _screenText;
    [FormerlySerializedAs("waitingPanel")] public GameObject WaitingPanel;
    [FormerlySerializedAs("myName")] public GameObject MyName;
    [FormerlySerializedAs("enemyName")] public GameObject EnemyName;

    private void Start()
    {
        WaitingPanel.SetActive(true);

        _script = GameObject.Find("Board").GetComponent<MainGameController>();
        _script.GameType = 1; // ゲームタイプを1に設定
        _soundEnemy = GetComponent<AudioSource>();
        _objPhotonViewControl = GetComponent<PhotonView>();

        //配列の初期化
        for (int i = 0; i < Board.GetLength(0); i++)
        {
            for (int j = 0; j < Board.GetLength(1); j++)
            {
                Board[i, j] = -1;
                BoardNum[i, j] = -1;
            }
        }
        //初期用配列設定
        Board[0, 0] = _maxDiceId;
        BoardNum[0, 0] = 1;

        _diceBase = (GameObject)Resources.Load("DiceE");
        _dice = GameObject.Find("DiceE");
        Dices.Add(_dice);  //リストにオブジェクトを追加

        //名前の設定
        MyName.GetComponent<Text>().text = PlayerPrefs.GetString("userName");
        _objPhotonViewControl.RPC("setEnemyName", PhotonTargets.Others, PlayerPrefs.GetString("userName"));

        //Aqui = GameObject.Find("AquiE");
    }

    public void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable changedProperties)
    {
        // ルームプロパティからsumScoreを取得
        object value = null;
        if (changedProperties.TryGetValue("sumScore", out value))
        {
            _script.SumScore = (int)value;
            //レベルを計算
            _script.ComputeLevel();
        }
    }

    public void SendScore(int score)
    {
        _objPhotonViewControl.RPC("setEnemyScore", PhotonTargets.Others, score);
    }

    public void Pause()
    {
        SendLose();
        FadeManager.Instance.LoadScene("YouLose", 0.3f);
    }

    public void SendLose()
    {
        _objPhotonViewControl.RPC("youWin", PhotonTargets.Others, 0);
    }

    public void SendPoint(int x, int z, int a, int b)
    {
        _objPhotonViewControl.RPC("setEnemyBoard", PhotonTargets.Others, x, z, a, b);
    }
    public void SendSink(int x, int z)
    {
        _objPhotonViewControl.RPC("setEnemyDiceSinking", PhotonTargets.Others, x, z);
    }

    public void SendVanish(int x, int z)
    {
        Debug.Log("RPCED(Vanish)");
        _objPhotonViewControl.RPC("setEnemyDiceVanish", PhotonTargets.Others, x, z);

    }

    public void SendRoll(int x, int z, int d)
    {
        _objPhotonViewControl.RPC("setEnemyDiceRoll", PhotonTargets.Others, x, z, d);
    }

    [PunRPC]
    private void SetEnemyName(string eName)
    {
        EnemyName.GetComponent<Text>().text = eName;
    }

    [PunRPC]
    private void SetEnemyScore(int eScore)
    {
        EnemyScore = eScore;
        _soundEnemy.PlayOneShot(_soundEnemy.clip);
        if((EnemyScore / 50) > _damageDice) { //スコアが一定値を超えたら
            int i = (EnemyScore / 50) - _damageDice;
            _damageDice += i;
            for ( ; i>0 ; i--){
            _script.RandomDiceGenerate(1); //超えた分だけダイスを送る
            Debug.Log("相手のスコアによってサイコロが増えました");
            }
        }
    }

    [PunRPC] 
    private void YouWin(int data)
    {
        SceneManager.LoadScene("YouWin");
    }

    [PunRPC]
    private void SetEnemyBoard(int x, int z, int a, int b)
    {
        DiceGenerateE(x, z, a, b);
        if (Time.timeScale == 0f)
        {
            WaitingPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    [PunRPC]
    private void SetEnemyDiceSinking(int x, int z)
    {
        DiceSinkingE(x, z);
    }

    [PunRPC]
    private void SetEnemyDiceVanish(int x, int z)
    {
        DiceVanishE(x, z);
    }

    [PunRPC]
    private void SetEnemyDiceRoll(int x, int z, int d)
    {
        DiceRollE(x, z, d);
    }
    // EnemyBoardControl

    /**
    * ダイスを生成
    * @params x 配置するx座標
    * @params z 配置するz座標
    * @params a 上にする面
    */
    public void DiceGenerateE(int x, int z, int a, int b = 0)
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
        if (Board[x, z] == -1)
        {
            _maxDiceId++;
            Board[x, z] = _maxDiceId;
            Vector3 position = new Vector3(2.5f + (float)x * 0.5f, 0.25f, 1.5f + (float)z * 0.5f); //位置
            GameObject objDice = (GameObject)Instantiate(_diceBase, position, Quaternion.Euler(xi, yi, zi));
            Dices.Add(objDice); //リストにオブジェクトを追加
            BoardNum[x, z] = a;
        }

    }

    /**
     * ダイスを沈み状態にする
     */
    public void DiceSinkingE(int x, int z)
    {
        _script.ChangeColorOfGameObject(Dices[Board[x, z]], new Color(0.3f, 0.3f, 0.7f, 1.0f));
    }

    /**
     * ダイスを削除
     */
    public void DiceVanishE(int x, int z)
    {
        int diceId = Board[x, z];
        GameObject dice = Dices[diceId];
        Board[x, z] = -1;
        Destroy(dice);
    }

    /**
     * ダイスを回転
     */
    public void DiceRollE(int x, int z, int d)
    {
        int diceId = Board[x, z];
        GameObject dice = Dices[diceId];
        Vector3 rotatePoint, rotateAxis;
        float diceSizeHalf = dice.transform.localScale.x / 2f;
        if (d == 2)
        {
            Board[x + 1, z] = diceId;
            Board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(diceSizeHalf, -diceSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, -1);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 0)
        {
            Board[x - 1, z] = diceId;
            Board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(-diceSizeHalf, -diceSizeHalf, 0f);
            rotateAxis = new Vector3(0, 0, 1);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 1)
        {
            Board[x, z + 1] = diceId;
            Board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(0f, -diceSizeHalf, diceSizeHalf);
            rotateAxis = new Vector3(1, 0, 0);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
        if (d == 3)
        {
            Board[x, z - 1] = diceId;
            Board[x, z] = -1;
            rotatePoint = dice.transform.position + new Vector3(0f, -diceSizeHalf, -diceSizeHalf);
            rotateAxis = new Vector3(-1, 0, 0);
            StartCoroutine(MoveDice(diceId, rotatePoint, rotateAxis));

            return;
        }
    }

    private IEnumerator MoveDice(int id, Vector3 rotatePoint, Vector3 rotateAxis)
    {
        GameObject dice = Dices[id];
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