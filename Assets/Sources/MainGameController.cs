using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : MonoBehaviour
{
    public int level = 1; //!< ゲームのレベル
    public int score = 0; //!< ゲームのスコア
    public int stage = 1; //!< ゲームのステージ(0-6)
    private int stageBefore = 1; //!< 前フレームのゲームのステージ

    public int boardSize = 7; //!< 盤面のサイズ
    public int[,] board = new int[7, 7]; //!< さいころのIDを格納
    public int[,] board_num = new int[7, 7]; //!< さいころの面を格納
    public List<GameObject> dices = new List<GameObject>(); //!< さいころオブジェクト格納用リスト
    List<GameObject> vanishingDices = new List<GameObject>(); //!<消えるサイコロオブジェクト格納用リスト
    double timeElapsed = 0.0; //!< イベント用フレームカウント
    int maxDiceId = 0; //!< 現在のさいころIDの最大値
    public bool isRotate_dice = false; //!< さいころが回転中かどうか
    public bool isRotate_charactor = false; //!< キャラクターが移動中かどうか

    GameObject Dice, DiceBase, Aqui, VanishingDice, StatusText, ScreenText;
    AquiController objAquiController;
    DiceController objDiceController;
    StatusTextController objStatusText;
    ScreenTextController objScreenText;

    public Material[] _material;
    public Material[] _skyboxMaterial;

    private AudioSource sound_one;
    private AudioSource sound_levelup;
    private AudioSource sound_vanish;

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
        
        DiceBase =(GameObject)Resources.Load("Dice");
        Dice = GameObject.Find("Dice");
        dices.Add(Dice);  //リストにオブジェクトを追加

        Aqui = GameObject.Find("Aqui");
        objAquiController = Aqui.GetComponent<AquiController>();
        objDiceController = Dice.GetComponent<DiceController>();

        StatusText = GameObject.Find("StatusText");
        objStatusText = StatusText.GetComponent<StatusTextController>();
        ScreenText = GameObject.Find("ScreenText");
        objScreenText = ScreenText.GetComponent<ScreenTextController>();


        //さいころをいくつか追加
        for (int i = 0; i < 10; i++)
        {
            randomDiceGenerate();
        }

        BgmManager.Instance.Play((stage+1).ToString()); //BGM

        //AudioSourceコンポーネントを取得し、変数に格納
        AudioSource[] audioSources = GetComponents<AudioSource>();
        sound_one = audioSources[0];
        sound_levelup = audioSources[1];
        sound_vanish = audioSources[2];
    }

    // Update is called once per frame
    void Update()
    {
        // キー入力一括制御
        if (isRotate_dice == false && isRotate_charactor == false)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if(objDiceController.SetTargetPosition(2)) {
                    VanishDice(objDiceController.X, objDiceController.Z);
                }
                objAquiController.SetTargetPosition(2);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (objDiceController.SetTargetPosition(0))
                {
                    VanishDice(objDiceController.X, objDiceController.Z);
                }
                objAquiController.SetTargetPosition(0);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                if (objDiceController.SetTargetPosition(1))
                {
                    VanishDice(objDiceController.X, objDiceController.Z);
                }
                objAquiController.SetTargetPosition(1);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (objDiceController.SetTargetPosition(3))
                {
                    VanishDice(objDiceController.X, objDiceController.Z);
                }
                objAquiController.SetTargetPosition(3);
            }

            //動いたときにダイスが変わる場合
            if (objAquiController.x != objDiceController.X || objAquiController.z != objDiceController.Z)
            {
                if (board[objAquiController.x, objAquiController.z] != -1) // 移動先にサイコロが存在するならば
                {
                    Dice = dices[board[objAquiController.x, objAquiController.z]];
                    objDiceController = Dice.GetComponent<DiceController>();
                    objDiceController.isSelected = true; //選択
                }
            }

        }


        timeElapsed += Time.deltaTime;
       
        if(level > 20) { //現状レベル20で速さは打ち止め
            if (timeElapsed >= 1.5)
            {
                randomDiceGenerate();
                timeElapsed = 0.0f;
            }
        }
        // さいころ追加 スタートは3.5秒ごと、ゴールは1.5秒ごと
        else if (timeElapsed >= (3.5f-0.1f*level))
        {
            randomDiceGenerate();
            timeElapsed = 0.0f;
        }

        //ゲームステージの設定
        if (stage != stageBefore)
        {
            changeStage(stage);
        }
    }

    // ステージを変える
    void changeStage(int nextStage)
    {
        this.GetComponent<Renderer>().sharedMaterial = _material[nextStage]; //盤面
        RenderSettings.skybox = _skyboxMaterial[nextStage]; //背景
        BgmManager.Instance.Play((nextStage+1).ToString()); //BGM
        if (nextStage == 6)
        {
            objStatusText.setText("Stage Changed! (ステージボーナスは無し!)");
        }
        else
        {
            objStatusText.setText("Stage Changed! (ステージボーナスは " + (nextStage + 1) + " の面)");
        }
    }

    void randomDiceGenerate()
    {
        // 配置する座標を決定

        int count = 0;
        int[,] chusen = new int[boardSize*boardSize,2];
        for (int j = 0; j < boardSize; j++){
            for (int k = 0; k < boardSize; k++){
                if(board[j,k]==-1){
                    chusen[count,0] = j;
                    chusen[count,1] = k;
                    count++; //空白の座標をchusenに保存
                }
            }
        }

        if (count == 0) {
            //全てのさいころがisVanishingかチェック
            bool gameoverFlag = true;
            foreach (GameObject tempDice in dices)
            {
                try
                {
                    if (tempDice.GetComponent<DiceController>().isVanishing)
                    {
                        gameoverFlag = false;
                        break;
                    }
                }
                catch (MissingReferenceException)
                {
                    
                }
            }

            //ゲームオーバーの時
            if(gameoverFlag)
            {
                BgmManager.Instance.Stop();
                objScreenText.setText("Game Over!");
                objAquiController.deathMotion();
            }

            return; 
        } //全部埋まってた場合

        int choose = Random.Range(0, count); //配置する場所をランダムに決定
        int x = chusen[choose,0];
        int z = chusen[choose,1];


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
            GameObject objDice = (GameObject)Instantiate(DiceBase, position, Quaternion.Euler(xi, yi, zi));
            DiceController objDiceController = objDice.GetComponent<DiceController>();
            objDiceController.isSelected = false;
            objDiceController.X = x;
            objDiceController.Z = z;
            objDiceController.surfaceA = a;
            objDiceController.surfaceB = b;
            objDiceController.diceId = maxDiceId;
            dices.Add(objDice); //リストにオブジェクトを追加
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

    //サイコロ消える
    void VanishDice(int x, int z)
    {
        if (board_num[x, z] == 1)
        {
            bool flag = false;

            if(x < boardSize - 1 && board[x+1,z] != -1){
                DiceController right = dices[board[x + 1, z]].GetComponent<DiceController>();
                if(right.isVanishing == true && board_num[x + 1, z] != 1) 
                {
                    flag = true;
                }
            }

            if (x > 0 && board[x - 1, z] != -1)
            {
                DiceController left = dices[board[x - 1, z]].GetComponent<DiceController>();
                if (left.isVanishing == true && board_num[x - 1, z] != 1)
                {
                    flag = true;
                }
            }

            if (z < boardSize - 1 && board[x, z + 1] != -1)
            {
                DiceController up = dices[board[x, z + 1]].GetComponent<DiceController>();
                if (up.isVanishing == true && board_num[x, z + 1] != 1)
                {
                    flag = true;
                }
            }


            if (z > 0  && board[x, z - 1] != -1)
            {
                DiceController down = dices[board[x, z - 1]].GetComponent<DiceController>();
                if (down.isVanishing == true && board_num[x, z - 1] != 1)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                Debug.Log("ハッピーワン！");
                int sum = 0;
                vanishingDices.Clear(); //カウントしたダイスのリストを初期化
                for (int i = 0; i < boardSize; i++){ //1のダイスを検索
                    for (int j = 0; j < boardSize; j++) {
                        if(board_num[i,j]==1){
                            vanishingDices.Add(dices[board[i, j]]); //削除リストへ追加
                            sum++; //数を記録
                        }
                    }
                }
                vanishingDices.Remove(dices[board[x, z]]); //足元のダイスのみ削除リストから減らす
                int count = 0;
                while (count < sum-1)
                {
                    StartCoroutine(sinkingDice(vanishingDices[count]));
                    vanishingDices[count].GetComponent<DiceController>().isVanishing = true;
                    count++;
                }
                score += count; //スコア計算(仮)
                objStatusText.setText("+" + count + " (ハッピーワン!)");
                //ステージボーナス
                if (board_num[x, z] == stage + 1)
                {
                    score += count;
                    objScreenText.setText("ステージボーナス! +" + count*10);
                }
                ComputeLevel(); //レベル計算
                sound_one.PlayOneShot(sound_one.clip);
            }
        }
        else
        {
            int count = 1; //隣接サイコロ数

            //カウントしたダイスのリストを初期化
            vanishingDices.Clear();
            vanishingDices.Add(dices[board[x, z]]);

            //隣接する同じ目のダイス数の計算
            count = CountDice(x, z, count);

            //Debug.Log("隣接するダイス数:" + count);

            //消す処理
            if (count >= board_num[x, z])
            {
                vanishingDices.Add(dices[board[x, z]]);
                DiceController temp;
                for (int j = 0; j < count; j++)
                {
                    temp = vanishingDices[j].GetComponent<DiceController>();
                    // board[temp.X, temp.Z] = -1;
                    // board_num[temp.X, temp.Z] = -1;
                    StartCoroutine(sinkingDice(vanishingDices[j]));
                    temp.isVanishing = true;
                }
                score += count * board_num[x, z]; //スコア計算(仮)
                objStatusText.setText("+" + count * board_num[x, z]);

                //ステージボーナス
                if (board_num[x, z] == stage + 1)
                {
                    score += board_num[x, z]*count;
                    objScreenText.setText("ステージボーナス! +" + board_num[x, z]*count);
                }
                ComputeLevel(); //レベル計算
                sound_vanish.PlayOneShot(sound_vanish.clip);
            }
        }

    }

    // ダイスをしずめるアニメ
    IEnumerator sinkingDice(GameObject dc) {
        DiceController temp = dc.GetComponent<DiceController> ();
        if (temp.isVanishing == true)
        {
            yield break;
        }
        while (isRotate_dice == true) {
            yield return new WaitForEndOfFrame ();
        }
        Vector3 position = dc.transform.position;
        for (int i = 1; i < 480; i++) {
            position.y = 0.5f - i * 1f / 480f;
            ChangeColorOfGameObject(dc, new Color(1.0f, 1.0f, 1.0f, 1.0f - i / 480f));
            dc.transform.position = position;
            yield return null;
        }
        board[temp.X, temp.Z] = -1;
        board_num[temp.X, temp.Z] = -1;
        Destroy(dc);
        yield break;
    }

    //親オブジェクトを入力すると親と子オブジェクトの色を変更してくれる
    private void ChangeColorOfGameObject(GameObject targetObject, Color color)
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

    int CountDice(int x, int z, int cnt) {
        bool flag = false; //脱出用

        while (flag == false)
        {
            flag = true;
            if (x < boardSize - 1 && board_num[x + 1, z] == board_num[x, z] && !vanishingDices.Contains(dices[board[x + 1, z]]))
            {
                cnt++;
                vanishingDices.Add(dices[board[x + 1, z]]);
                flag = false;
                cnt = CountDice(x + 1, z, cnt);
            }

            if (x > 0 && board_num[x - 1, z] == board_num[x, z] && !vanishingDices.Contains(dices[board[x - 1, z]]))
            {
                cnt++;
                vanishingDices.Add(dices[board[x - 1, z]]);
                flag = false;
                cnt = CountDice(x - 1, z, cnt);
            }
            if (z < boardSize - 1 && board_num[x, z + 1] == board_num[x, z] && !vanishingDices.Contains(dices[board[x, z + 1]]))
            {
                cnt++;
                vanishingDices.Add(dices[board[x, z + 1]]);
                flag = false;
                cnt = CountDice(x, z + 1, cnt);
            }
            if (z > 0 && board_num[x, z - 1] == board_num[x, z] && !vanishingDices.Contains(dices[board[x, z - 1]]))
            {
                cnt++;
                vanishingDices.Add(dices[board[x, z - 1]]);
                flag = false;
                cnt = CountDice(x, z - 1, cnt);
            }
        }

            return cnt;
    }

    void ComputeLevel () {
        int a = 12; //おおよそ1レベルの上昇に必要なスコア
        int b = a; //前の必要経験値を記録する
        int lv = 1;

        //レベルの変化
        while (true) {
            b = (int)((a * lv + b * 1.1)/ 2);
            if (score > b) {
                lv++;
            } else {
                break;
            }
        }

        if (level != lv)
        {
            sound_levelup.PlayOneShot(sound_levelup.clip);
        }

        // ステージの計算
        level = lv;
        stageBefore = stage;
        stage = level % 21 / 3 + 1;
        if (stage == 7)
        {
            stage -= 7;
        }

    }


}

