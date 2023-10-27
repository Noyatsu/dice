using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MainGameController : MonoBehaviour
{
    [FormerlySerializedAs("gameType")] public int GameType = 0; //!< ゲームタイプ(0ならエンドレス、1ならオンライン、2ならチュートリアル)
    [FormerlySerializedAs("playerType")] public int PlayerType = 0; //!< 0なら部屋主、1ならそれ以外(オンライン用)

    [FormerlySerializedAs("level")] public int Level = 1; //!< ゲームのレベル
    public static int Score = 0; //!< ゲームのスコア
    [FormerlySerializedAs("sumScore")] public int SumScore = 0; //!< 対戦用合計スコア
    [FormerlySerializedAs("stage")] public int Stage = 1; //!< ゲームのステージ(0-6)
    private int _stageBefore = 1; //!< 前フレームのゲームのステージ

    [FormerlySerializedAs("boardSize")] public int BoardSize = 7; //!< 盤面のサイズ
    public int[,] Board = new int[7, 7]; //!< さいころのIDを格納
    public int[,] BoardNum = new int[7, 7]; //!< さいころの面を格納
    public float[,] BoardY = new float[7, 7]; //!< さいころのY座標を格納
    [FormerlySerializedAs("dices")] public List<GameObject> Dices = new List<GameObject>(); //!< さいころオブジェクト格納用リスト
    private List<GameObject> _vanishingDices = new List<GameObject>(); //!<消えるサイコロオブジェクト格納用リスト
    private double _timeElapsed = 0.0; //!< イベント用フレームカウント
    private int _maxDiceId = 0; //!< 現在のさいころIDの最大値
    [FormerlySerializedAs("isRotate_dice")] public bool IsRotateDice = false; //!< さいころが回転中かどうか
    [FormerlySerializedAs("isRotate_charactor")] public bool IsRotateCharactor = false; //!< キャラクターが移動中かどうか
    private bool _isGameovered = false; //ゲームオーバーしたかどうか
    [FormerlySerializedAs("isStarting")] public bool IsStarting = true; // スタート処理が行われているか

    [FormerlySerializedAs("initDicesNum")] public int InitDicesNum = 20; //!< 初期のさいころの数

    [SerializeField] private GameObject _dicePrefab;

    private GameObject _dice, _aqui, _vanishingDice, _statusText, _screenText, _gobjOgController;
    [FormerlySerializedAs("gobjSendDice")] [SerializeField]
    private GameObject _gobjSendDice;
    [FormerlySerializedAs("gobjSendedDice")] [SerializeField]
    private GameObject _gobjSendedDice;

    private AquiController _objAquiController;
    private DiceController _objDiceController;
    private StatusTextController _objStatusText;
    private ScreenTextController _objScreenText;

    [FormerlySerializedAs("_material")] public Material[] Material;
    [FormerlySerializedAs("_skyboxMaterial")] public Material[] SkyboxMaterial;

    private AudioSource _soundOne;
    private AudioSource _soundLevelup;
    private AudioSource _soundVanish;

    private Vector3 _clickstartPos;

    // Use this for initialization
    private void Awake()
    {
        Score = 0;

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
        
        _dice = GameObject.Find("Dice");
        Dices.Add(_dice);  //リストにオブジェクトを追加
        _aqui = GameObject.Find("Aqui");
        _objAquiController = _aqui.GetComponent<AquiController>();
        _objDiceController = _dice.GetComponent<DiceController>();

        if (GameType == 3)
        {
            Board[0, 0] = -1;
            BoardNum[0, 0] = -1;
            _maxDiceId = 0;
            Dices.Clear();
            Destroy(_dice);
        }

        _statusText = GameObject.Find("StatusText");
        _objStatusText = _statusText.GetComponent<StatusTextController>();
        _screenText = GameObject.Find("ScreenText");
        _objScreenText = _screenText.GetComponent<ScreenTextController>();

        _gobjOgController = GameObject.Find("OnlineGameController");



        //BGM
        if (GameType != 2)
        {
            BgmManager.Instance.Play((Stage + 1).ToString()); //BGM
        }
        else
        {
            BgmManager.Instance.Play("tutorial"); //BGM
        }

        //AudioSourceコンポーネントを取得し、変数に格納
        AudioSource[] audioSources = GetComponents<AudioSource>();
        _soundOne = audioSources[0];
        _soundLevelup = audioSources[1];
        _soundVanish = audioSources[2];
    }

    /*
     * ぷに操作による移動方向を返す
     * @return 移動方向(int)
     */
    private int Puni()
    {
        double threshold = 50.0f; //!< 移動判定の閾値

        // マウスが押された瞬間に押された場所を格納する
        if (Input.GetMouseButtonDown(0))
        {
            _clickstartPos = Input.mousePosition;
        }

        double xDiff = Input.mousePosition.x - _clickstartPos.x;
        double yDiff = Input.mousePosition.y - _clickstartPos.y;

        // マウスが移動したときに移動距離が一定を超えたら(判定は円状)
        if (Input.GetMouseButton(0) && (System.Math.Pow(xDiff, 2) + System.Math.Pow(yDiff, 2)) > (threshold * threshold))
        {
            // 左
            if (xDiff < 0 && yDiff > 0)
            {
                return 0;
            }
            // 上
            else if (yDiff > 0 && xDiff > 0)
            {
                return 1;
            }
            // 下
            else if (yDiff < 0 && xDiff < 0)
            {
                return 3;
            }
            // 右
            else if (xDiff > 0 && yDiff < 0)
            {
                return 2;
            }
        }
        return -1;
    }



    // Update is called once per frame
    private void Update()
    {
        // スタート処理
        if (IsStarting)
        {
            if (GameType < 2)
            {
                //さいころをいくつか追加
                for (int i = 0; i < InitDicesNum; i++)
                {
                    RandomDiceGenerate();
                }
            }
            IsStarting = false;

            if (GameType == 1)
            {
                Time.timeScale = 0f;
            }

        }



        int flick = Puni(); //ぷに検知

        // キー入力一括制御
        if (IsRotateDice == false && IsRotateCharactor == false && _isGameovered == false)
        {
            if (Input.GetKey(KeyCode.RightArrow) || flick == 2)
            {
                if (_dice && _objDiceController.SetTargetPosition(2))
                {
                    VanishDice(_objDiceController.X, _objDiceController.Z);
                }
                _objAquiController.SetTargetPosition(2);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || flick == 0)
            {
                if (_dice && _objDiceController.SetTargetPosition(0))
                {
                    VanishDice(_objDiceController.X, _objDiceController.Z);
                }
                _objAquiController.SetTargetPosition(0);
            }
            else if (Input.GetKey(KeyCode.UpArrow) || flick == 1)
            {
                if (_dice && _objDiceController.SetTargetPosition(1))
                {
                    VanishDice(_objDiceController.X, _objDiceController.Z);
                }
                _objAquiController.SetTargetPosition(1);
            }
            else if (Input.GetKey(KeyCode.DownArrow) || flick == 3)
            {
                if (_dice && _objDiceController.SetTargetPosition(3))
                {
                    VanishDice(_objDiceController.X, _objDiceController.Z);
                }
                _objAquiController.SetTargetPosition(3);
            }
            else if (Input.GetKey(KeyCode.K))
            {
                Score += 1000;
            }

            if (_objAquiController.X != _objDiceController.X || _objAquiController.Z != _objDiceController.Z)
            {
                if (Board[_objAquiController.X, _objAquiController.Z] != -1) // 移動先にサイコロが存在するならば
                {
                    _dice = Dices[Board[_objAquiController.X, _objAquiController.Z]];
                    _objDiceController = _dice.GetComponent<DiceController>();
                    _objDiceController.IsSelected = true; //選択
                }
            }
        }


        _timeElapsed += Time.deltaTime;

        if (GameType != 2)
        {
            if (GameType == 0)
            { //ソロモードの速さ
                if (Level > 21)
                { //現状レベル21で速さは打ち止め
                    if (_timeElapsed >= (1.2 + (0.3 * Mathf.Sin(Mathf.PI * Level / 4))))
                    {
                        RandomDiceGenerate();
                        _timeElapsed = 0.0f;
                    }
                }
                // さいころ追加 スタートは3秒ごと、ゴールは1.25秒ごと
                else if (_timeElapsed >= (3f - (1 / 12f) * Level)) //1/12は(初速-最高速)/レベルで求められた
                {
                    RandomDiceGenerate();
                    _timeElapsed = 0.0f;
                }
            }
            if (GameType == 1)
            { //対戦モードの速さ
                if (Level > 21)
                { //現状レベル21で速さは打ち止め
                    if (_timeElapsed >= 1.5)
                    {
                        RandomDiceGenerate();
                        _timeElapsed = 0.0f;
                    }
                }
                // さいころ追加 スタートは3.5秒ごと、ゴールは1.5秒ごと
                else if (_timeElapsed >= (3.5f - (2 / 21f) * Level))
                {
                    RandomDiceGenerate();
                    _timeElapsed = 0.0f;
                }
            }
        }

    }

    public void SetAqui(int x, float y, int z)
    {
        _objAquiController.SetTarget(x, y, z);
    }

    public void SetAquiDiscrete(int x, float y, int z)
    {
        _objAquiController.SetTargetDiscrete(x, y, z);
    }

    // ステージを変える
    public void ChangeStage(int nextStage)
    {
        this.GetComponent<Renderer>().sharedMaterial = Material[nextStage]; //盤面
        if (GameType == 1)
        {
            GameObject.Find("EnemyBoard").GetComponent<Renderer>().sharedMaterial = Material[nextStage];
        }
        RenderSettings.skybox = SkyboxMaterial[nextStage]; //背景
        BgmManager.Instance.Play((nextStage + 1).ToString()); //BGM
        if (nextStage == 6)
        {
            _objStatusText.SetText("Stage Changed! (ステージボーナスは無し!)");
        }
        else
        {
            _objStatusText.SetText("Stage Changed! (ステージボーナス: " + (nextStage + 1));
        }
    }

    /**
     * ダイスを生成
     * @params x 配置するx座標
     * @params z 配置するz座標
     * @params a 上にする面
     */
    public void DiceGenerate(int x, int z, int a, int b = 0, int type = 0)
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
            Vector3 position = new Vector3(-4.5f + (float)x, -0.5f, -4.5f + (float)z); //位置
            GameObject objDice = Instantiate(_dicePrefab, position, Quaternion.Euler(xi, yi, zi));
            DiceController mobjDiceController = objDice.GetComponent<DiceController>();
            mobjDiceController.IsSelected = false;
            mobjDiceController.X = x;
            mobjDiceController.Z = z;
            mobjDiceController.SurfaceA = a;
            mobjDiceController.SurfaceB = b;
            mobjDiceController.DiceId = _maxDiceId;
            mobjDiceController.IsGenerate = true;
     
            Dices.Add(objDice); //リストにオブジェクトを追加

            if (type == 1)
            {
                ChangeColorOfGameObject(Dices[Board[x, z]], new Color(1.0f, 0.5f, 0.8f, 1.0f));
                _gobjSendedDice.GetComponent<Animator>().SetTrigger("sendedDice");
            }
            BoardNum[x, z] = a;
        }

    }

    public void RandomDiceGenerate(int type = 0)
    {
        // 配置する座標を決定
        int count = 0;
        int[,] chusen = new int[BoardSize * BoardSize, 2];
        for (int j = 0; j < BoardSize; j++)
        {
            for (int k = 0; k < BoardSize; k++)
            {
                if (Board[j, k] == -1)
                {
                    chusen[count, 0] = j;
                    chusen[count, 1] = k;
                    count++; //空白の座標をchusenに保存
                }
            }
        }

        if (GameType == 3 && count == 49)
        {
            GameObject.Find("PuzzleGameController").GetComponent<PuzzleGameController>().YouWin();
        }

        if (count == 0 && _isGameovered == false)
        {
            //全てのさいころがisVanishingかチェック
            bool gameoverFlag = true;

            for (int j = 0; j < BoardSize; j++)
            {
                for (int k = 0; k < BoardSize; k++)
                {
                    if (Dices[Board[j, k]].GetComponent<DiceController>().IsVanishing == true)
                    {
                        gameoverFlag = false;
                        break;
                    }
                }
            }

            //ゲームオーバーの時
            if (gameoverFlag == true && _isGameovered == false)
            {
                _isGameovered = true;
                BgmManager.Instance.Stop();
                _objScreenText.SetText("Game Over!");
                _objAquiController.DeathMotion();

                DontDestroyOnLoad(this);
            }

            return;
        } //全部埋まってた場合

        int choose = Random.Range(0, count); //配置する場所をランダムに決定
        int x = chusen[choose, 0];
        int z = chusen[choose, 1];


        // 配置する面を決定
        int a = Random.Range(1, 6);

        // ダイスを生成
        DiceGenerate(x, z, a, 0, type);
    }

    private void Delay()
    {

    }

    //サイコロ消える
    private void VanishDice(int x, int z)
    {
        if (BoardNum[x, z] == 1) // ワンゾロバニッシュ発生
        {
            bool flag = false;

            if (x < BoardSize - 1 && Board[x + 1, z] != -1)
            {
                DiceController right = Dices[Board[x + 1, z]].GetComponent<DiceController>();
                if (right.IsVanishing == true && BoardNum[x + 1, z] != 1)
                {
                    flag = true;
                }
            }

            if (x > 0 && Board[x - 1, z] != -1)
            {
                DiceController left = Dices[Board[x - 1, z]].GetComponent<DiceController>();
                if (left.IsVanishing == true && BoardNum[x - 1, z] != 1)
                {
                    flag = true;
                }
            }

            if (z < BoardSize - 1 && Board[x, z + 1] != -1)
            {
                DiceController up = Dices[Board[x, z + 1]].GetComponent<DiceController>();
                if (up.IsVanishing == true && BoardNum[x, z + 1] != 1)
                {
                    flag = true;
                }
            }


            if (z > 0 && Board[x, z - 1] != -1)
            {
                DiceController down = Dices[Board[x, z - 1]].GetComponent<DiceController>();
                if (down.IsVanishing == true && BoardNum[x, z - 1] != 1)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                Debug.Log("ワンゾロバニッシュ!!");
                int sum = 0;
                _vanishingDices.Clear(); //カウントしたダイスのリストを初期化
                for (int i = 0; i < BoardSize; i++)
                { //1のダイスを検索
                    for (int j = 0; j < BoardSize; j++)
                    {
                        if (BoardNum[i, j] == 1)
                        {
                            _vanishingDices.Add(Dices[Board[i, j]]); //削除リストへ追加
                            sum++; //数を記録
                        }
                    }
                }
                _vanishingDices.Remove(Dices[Board[x, z]]); //足元のダイスのみ削除リストから減らす
                int count = 0;
                while (count < sum - 1)
                {
                    _vanishingDices[count].GetComponent<DiceController>().IsVanishing = true;
                    count++;
                }
                AddScore(count);
                _objStatusText.SetText("+" + count + " (ワンゾロバニッシュ!!)");
                //ステージボーナス
                if (BoardNum[x, z] == Stage + 1)
                {
                    AddScore(count);
                    _objScreenText.SetText("Stage Bonus! +" + count * 10);
                }
                ComputeLevel(); //レベル計算
                _soundOne.PlayOneShot(_soundOne.clip);
            }
        }
        else
        {
            int count = 1; //隣接サイコロ数

            //カウントしたダイスのリストを初期化
            _vanishingDices.Clear();
            _vanishingDices.Add(Dices[Board[x, z]]);

            //隣接する同じ目のダイス数の計算
            count = CountDice(x, z, count);

            //消す処理
            if (count >= BoardNum[x, z]) //隣接するさいころの数がそのさいころの目以上だったら
            {
                _vanishingDices.Add(Dices[Board[x, z]]);

                DiceController temp;
                for (int j = 0; j < count; j++)
                {
                    temp = _vanishingDices[j].GetComponent<DiceController>();
                    temp.IsVanishing = true;
                }
                AddScore(count * BoardNum[x, z]);
                _objStatusText.SetText("+" + count * BoardNum[x, z]);

                //ステージボーナス
                if (BoardNum[x, z] == Stage + 1)
                {
                    AddScore(BoardNum[x, z] * count);
                    _objScreenText.SetText("Stage Bonus! +" + BoardNum[x, z] * count);
                }
                ComputeLevel(); //レベル計算
                _soundVanish.PlayOneShot(_soundVanish.clip);
            }
        }

    }

    //親オブジェクトを入力すると親と子オブジェクトの色を変更してくれる
    public void ChangeColorOfGameObject(GameObject targetObject, Color color)
    {
        if (targetObject != null)
        {
            //入力されたオブジェクトのRendererを全て取得し、さらにそのRendererに設定されている全Materialの色を変える
            foreach (Renderer targetRenderer in targetObject.GetComponents<Renderer>())
            {
                foreach (Material material in targetRenderer.materials)
                {
                    material.color = color;
                }
            }

            //入力されたオブジェクトの子にも同様の処理を行う
            for (int i = 0; i < targetObject.transform.childCount; i++)
            {
                ChangeColorOfGameObject(targetObject.transform.GetChild(i).gameObject, color);
            }
        }

    }

    private int CountDice(int x, int z, int cnt)
    {
        bool flag = false; //脱出用

        while (flag == false)
        {
            flag = true;
            if (x < BoardSize - 1 && BoardNum[x + 1, z] == BoardNum[x, z] && !_vanishingDices.Contains(Dices[Board[x + 1, z]]))
            {
                cnt++;
                _vanishingDices.Add(Dices[Board[x + 1, z]]);

                flag = false;
                cnt = CountDice(x + 1, z, cnt);
            }

            if (x > 0 && BoardNum[x - 1, z] == BoardNum[x, z] && !_vanishingDices.Contains(Dices[Board[x - 1, z]]))
            {
                cnt++;
                _vanishingDices.Add(Dices[Board[x - 1, z]]);

                flag = false;
                cnt = CountDice(x - 1, z, cnt);
            }
            if (z < BoardSize - 1 && BoardNum[x, z + 1] == BoardNum[x, z] && !_vanishingDices.Contains(Dices[Board[x, z + 1]]))
            {
                cnt++;
                _vanishingDices.Add(Dices[Board[x, z + 1]]);

                flag = false;
                cnt = CountDice(x, z + 1, cnt);
            }
            if (z > 0 && BoardNum[x, z - 1] == BoardNum[x, z] && !_vanishingDices.Contains(Dices[Board[x, z - 1]]))
            {
                cnt++;
                _vanishingDices.Add(Dices[Board[x, z - 1]]);

                flag = false;
                cnt = CountDice(x, z - 1, cnt);
            }
        }

        return cnt;
    }

    public void ResetGame()
    {
        Destroy(_objDiceController);
        Destroy(_dice);

        var clones = GameObject.FindGameObjectsWithTag("dice");
        foreach (var clone in clones)
        {
            foreach (Transform n in clone.transform)
            {
                GameObject.Destroy(n.gameObject);
            }
            Destroy(clone);
            Dices.Remove(clone);
        }

        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                Board[i, j] = -1;
                BoardNum[i, j] = -1;
            }
        }
        Dices.Clear();
        _maxDiceId = -1;
        Score = 0;
        IsRotateDice = false;
        IsRotateCharactor = false;
        _isGameovered = false;
    }

    public void ComputeLevel()
    {
        int a = 150; //おおよそ1レベルの上昇に必要なスコア
        int b = a; //前の必要経験値を記録する
        int lv = 1;

        //レベルの変化
        while (true)
        {
            b = (int)((a * lv + b * 1.08) / 2);
            if (Score > b && GameType == 0)
            {
                lv++;
            }
            else if (SumScore > b && GameType == 1)
            {
                lv++;
            }
            else
            {
                break;
            }
        }

        if (Level != lv)
        {
            _soundLevelup.PlayOneShot(_soundLevelup.clip);
        }

        // ステージの計算
        Level = lv;
        _stageBefore = Stage;
        Stage = Level % 21 / 3 + 1;
        if (Stage == 7)
        {
            Stage -= 7;
        }
        if (_stageBefore != Stage)
        {
            ChangeStage(Stage);
        }

    }

    //スコア計算用
    private void AddScore(int num)
    {
        int prevScoreDiv = Score / 50;
        Score += num;
        SumScore += num;
    }

    private void ShowArraylog()
    {
        string str = "";
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                str += Board[i, j].ToString() + ",";
            }
            str += "\n";
        }
        Debug.Log(str);

    }

}

