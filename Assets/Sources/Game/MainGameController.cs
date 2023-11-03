using System.Collections.Generic;
using System.Linq;
using SSTraveler.Utility.ReactiveProperty;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace SSTraveler.Game
{
    public class MainGameController : MonoBehaviour, IMainGameController
    {
        public int GameType => _gameType;
        public bool IsRotateDice { get; set; }
        public bool IsRotateCharacter { get; set; }
        public bool IsStarting { get; set; } = true;
        public ReactiveProperty<string> StatusText { get; } = new();
        public ReactiveProperty<string> ScreenText { get; } = new();
        
        [SerializeField]
        [FormerlySerializedAs("GameType")] private int _gameType = 0; //!< ゲームタイプ(0ならエンドレス、1ならオンライン、2ならチュートリアル)

        private List<DiceController> _vanishingDices = new(); //!<消えるサイコロオブジェクト格納用リスト
        private double _timeElapsed = 0.0; //!< イベント用フレームカウント
        private bool _isGameovered = false; //ゲームオーバーしたかどうか

        public int InitDicesNum = 20; //!< 初期のさいころの数
        
        private GameObject _aqui, _vanishingDice;

        private AquiController _objAquiController;
        private DiceController _currentDice;

        private AudioSource _soundOne;
        private AudioSource _soundLevelup;
        private AudioSource _soundVanish;

        private Vector3 _clickstartPos;
        
        private IBoard _board;
        private IDiceContainer _diceContainer;
        private IGameProcessManager _gameProcessManager;
        private IStageEffectManager _stageEffectManager;
        
        [Inject]
        public void Construct(IBoard board, IDiceContainer diceContainer, IGameProcessManager gameProcessManager,
            IStageEffectManager stageEffectManager)
        {
            _board = board;
            _diceContainer = diceContainer;
            _gameProcessManager = gameProcessManager;
            _stageEffectManager = stageEffectManager;
        }

        private void Awake()
        {
            Application.targetFrameRate = 60;
            _gameProcessManager.ResetScore();
            _gameProcessManager.Stage.Value = 1;
            _gameProcessManager.Stage.Subscribe(_ => _soundLevelup.PlayOneShot(_soundLevelup.clip)).AddTo(this);
            _gameProcessManager.Stage.Subscribe(ChangeStage).AddTo(this);
            _stageEffectManager.SetStage(_gameProcessManager.Stage);
            
            _diceContainer.Init();
            _board.Reset();

            // 初期用配列設定
            _currentDice = DiceGenerate(0, 0, 3);
            _currentDice.IsSelected = true;
            _aqui = GameObject.Find("Aqui");
            _objAquiController = _aqui.GetComponent<AquiController>();

            if (_gameType == 3)
            {
                _board.SetDice(0, 0, null);
                _diceContainer.ReturnInstance(_currentDice);
            }
            
            // BGM
            if (_gameType != 2)
            {
                BgmManager.Instance.Play((_gameProcessManager.Stage + 1).ToString()); //BGM
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
                if (_gameType < 2)
                {
                    //さいころをいくつか追加
                    for (int i = 0; i < InitDicesNum; i++)
                    {
                        RandomDiceGenerate();
                    }
                }

                IsStarting = false;

                if (_gameType == 1)
                {
                    Time.timeScale = 0f;
                }

            }
            
            int flick = Puni(); //ぷに検知

            // キー入力一括制御
            if (IsRotateDice == false && IsRotateCharacter == false && _isGameovered == false)
            {
                if (Input.GetKey(KeyCode.RightArrow) || flick == 2)
                {
                    if (_currentDice && _currentDice.SetTargetPosition(2))
                    {
                        VanishDice(_currentDice.X, _currentDice.Z);
                    }

                    _objAquiController.SetTargetPosition(2);
                }
                else if (Input.GetKey(KeyCode.LeftArrow) || flick == 0)
                {
                    if (_currentDice && _currentDice.SetTargetPosition(0))
                    {
                        VanishDice(_currentDice.X, _currentDice.Z);
                    }

                    _objAquiController.SetTargetPosition(0);
                }
                else if (Input.GetKey(KeyCode.UpArrow) || flick == 1)
                {
                    if (_currentDice && _currentDice.SetTargetPosition(1))
                    {
                        VanishDice(_currentDice.X, _currentDice.Z);
                    }

                    _objAquiController.SetTargetPosition(1);
                }
                else if (Input.GetKey(KeyCode.DownArrow) || flick == 3)
                {
                    if (_currentDice && _currentDice.SetTargetPosition(3))
                    {
                        VanishDice(_currentDice.X, _currentDice.Z);
                    }

                    _objAquiController.SetTargetPosition(3);
                }
                else if (Input.GetKey(KeyCode.K))
                {
                    _gameProcessManager.AddScore(100);
                }

                if (_objAquiController.X != _currentDice.X || _objAquiController.Z != _currentDice.Z)
                {
                    if (!_board.GetCell(_objAquiController.X, _objAquiController.Z).IsEmpty) // 移動先にサイコロが存在するならば
                    {
                        _currentDice = _board[_objAquiController.X, _objAquiController.Z].Dice;
                        _currentDice.IsSelected = true; //選択
                    }
                }
            }


            
            _timeElapsed += Time.deltaTime;

            if (_isGameovered) return;
            if (_gameType != 2)
            {
                if (_gameType == 0)
                {
                    //ソロモードの速さ
                    if (_gameProcessManager.Level.Value > 21)
                    {
                        //現状レベル21で速さは打ち止め
                        if (_timeElapsed >= (1.2 + (0.3 * Mathf.Sin(Mathf.PI * _gameProcessManager.Level.Value / 4))))
                        {
                            RandomDiceGenerate();
                            _timeElapsed = 0.0f;
                        }
                    }
                    // さいころ追加 スタートは3秒ごと、ゴールは1.25秒ごと
                    else if (_timeElapsed >= (3f - (1 / 12f) * _gameProcessManager.Level.Value)) //1/12は(初速-最高速)/レベルで求められた
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
            _stageEffectManager.SetStage(nextStage);
            if (nextStage == 6)
            {
                StatusText.Value = "Stage Changed! (ステージボーナスは無し!)";
            }
            else
            {
                StatusText.Value = $"Stage Changed! (ステージボーナス: {nextStage + 1}";
            }
        }

        /**
         * ダイスを生成
         * @params x 配置するx座標
         * @params z 配置するz座標
         * @params a 上にする面
         */
        public DiceController DiceGenerate(int x, int z, int a, int b = 0, int type = 0)
        {
            if (_board[x, z].IsExist) return null;

            // その座標が空だったらさいころを追加
            Vector3 position = new Vector3(-4.5f + (float)x, -0.5f, -4.5f + (float)z); //位置
            DiceController dice = _diceContainer.GetNewInstance();
            dice.SetInitTransform(position, a, b);
            dice.IsSelected = false;
            dice.X = x;
            dice.Z = z;
            dice.IsGenerate = true;
            _board.SetDice(x, z, dice);

            return dice;

        }

        public void RandomDiceGenerate()
        {
            // 配置する座標を決定
            List<Vector2Int> candidates = new(_board.MaxCellNum);
            for (int j = 0; j < _board.Size; j++)
            {
                for (int k = 0; k < _board.Size; k++)
                {
                    if (_board[j, k].IsEmpty)
                    {
                        candidates.Add(new Vector2Int(j, k));
                    }
                }
            }

            if (_gameType == 3 && candidates.Count == 49)
            {
                GameObject.Find("PuzzleGameController").GetComponent<PuzzleGameController>().YouWin();
            }

            // 全部埋まってた場合
            if (candidates.Count == 0 && _isGameovered == false)
            {
                // 全てのさいころがisVanishingかチェック
                bool gameOverFlag = true;

                for (int j = 0; j < _board.Size; j++)
                {
                    for (int k = 0; k < _board.Size; k++)
                    {
                        if (_board[j, k].Dice.IsVanishing)
                        {
                            gameOverFlag = false;
                            break;
                        }
                    }
                }

                //ゲームオーバーの時
                if (gameOverFlag && _isGameovered == false)
                {
                    _isGameovered = true;
                    BgmManager.Instance.Stop();
                    ScreenText.Value = "Game Over!";
                    _objAquiController.DeathMotion();
                    return;
                }

                return;
            }

            // 配置する場所をランダムに決定
            var newPos = candidates[Random.Range(0, candidates.Count)];

            // 配置する面を決定
            int top = Random.Range(1, 6);

            // ダイスを生成
            DiceGenerate(newPos.x, newPos.y, top);
        }

        // サイコロ消える
        private void VanishDice(int x, int z)
        {
            // ワンゾロバニッシュ発生
            if (_board[x, z].DiceNum == 1)
            {
                bool flag = false;

                if (x < _board.Size - 1 && _board[x + 1, z].IsExist)
                {
                    DiceController right = _board[x + 1, z].Dice;
                    if (right.IsVanishing  && right.SurfaceA != 1)
                    {
                        flag = true;
                    }
                }

                if (x > 0 && _board[x - 1, z].IsExist)
                {
                    DiceController left = _board[x - 1, z].Dice;
                    if (left.IsVanishing && left.SurfaceA != 1)
                    {
                        flag = true;
                    }
                }

                if (z < _board.Size - 1 && _board[x, z + 1].IsExist)
                {
                    DiceController up = _board[x, z + 1].Dice;
                    if (up.IsVanishing && up.SurfaceA != 1)
                    {
                        flag = true;
                    }
                }


                if (z > 0 && _board[x, z - 1].IsExist)
                {
                    DiceController down = _board[x, z - 1].Dice;
                    if (down.IsVanishing && down.SurfaceA != 1)
                    {
                        flag = true;
                    }
                }

                if (flag)
                {
                    // 1のダイスを探す (足元のダイスは除く)
                    _vanishingDices = _board.Cells.Where(c => c.DiceNum == 1).Select(c => c.Dice).ToList();
                    _vanishingDices.Remove(_board[x, z].Dice); // 足元のダイスのみ削除リストから減らす
                    int count = _vanishingDices.Count;
                    if (count <= 0) return;
                    
                    Debug.Log("ワンゾロバニッシュ!!");
                    foreach (var dice in _vanishingDices)
                    {
                        dice.IsVanishing = true;
                    }

                    _gameProcessManager.AddScore(count);
                    StatusText.Value = $"+{count} (ワンゾロバニッシュ!!)";
                        
                    // ステージボーナス
                    if (_board[x, z].DiceNum == _gameProcessManager.Stage + 1)
                    {
                        _gameProcessManager.AddScore(count * 10);
                        ScreenText.Value = $"Stage Bonus! +{count * 10}";
                    }

                    _soundOne.PlayOneShot(_soundOne.clip);
                }
            }
            else
            {
                int count = 1; //隣接サイコロ数

                // カウントしたダイスのリストを初期化
                _vanishingDices.Clear();
                _vanishingDices.Add(_board[x, z].Dice);

                // 隣接する同じ目のダイス数の計算
                count = CountDice(x, z, count);

                // 消す処理 : 隣接するさいころの数がそのさいころの目以上だったら
                if (count >= _board[x, z].DiceNum)
                {
                    _vanishingDices.Add(_board[x, z].Dice);
                    for (int j = 0; j < count; j++)
                    {
                        _vanishingDices[j].IsVanishing = true;
                    }

                    _gameProcessManager.AddScore(count * _board[x, z].DiceNum);
                    StatusText.Value = $"+{count * _board[x, z].DiceNum}";

                    // ステージボーナス
                    if (_board[x, z].DiceNum == _gameProcessManager.Stage + 1)
                    {
                        _gameProcessManager.AddScore(_board[x, z].DiceNum * count);
                        ScreenText.Value = $"Stage Bonus! +{_board[x, z].DiceNum * count}";
                    }
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

        /// <summary>
        /// 隣接する同じ目のさいころの計算
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        private int CountDice(int x, int z, int cnt)
        {
            bool flag = false; //脱出用

            while (flag == false)
            {
                flag = true;
                int searchingDiceNum = _board[x, z].DiceNum;
                if (x < _board.Size - 1 && _board[x + 1, z].DiceNum == searchingDiceNum && !_vanishingDices.Contains(_board[x + 1, z].Dice))
                {
                    cnt++;
                    _vanishingDices.Add(_board[x + 1, z].Dice);

                    flag = false;
                    cnt = CountDice(x + 1, z, cnt);
                }

                if (x > 0 && _board[x - 1, z].DiceNum == searchingDiceNum && !_vanishingDices.Contains(_board[x - 1, z].Dice))
                {
                    cnt++;
                    _vanishingDices.Add(_board[x - 1, z].Dice);

                    flag = false;
                    cnt = CountDice(x - 1, z, cnt);
                }

                if (z < _board.Size - 1 && _board[x, z + 1].DiceNum == searchingDiceNum && !_vanishingDices.Contains(_board[x, z + 1].Dice))
                {
                    cnt++;
                    _vanishingDices.Add(_board[x, z + 1].Dice);

                    flag = false;
                    cnt = CountDice(x, z + 1, cnt);
                }

                if (z > 0 && _board[x, z - 1].DiceNum == searchingDiceNum && !_vanishingDices.Contains(_board[x, z - 1].Dice))
                {
                    cnt++;
                    _vanishingDices.Add(_board[x, z - 1].Dice);

                    flag = false;
                    cnt = CountDice(x, z - 1, cnt);
                }
            }

            return cnt;
        }

        public void ResetGame()
        {
            _diceContainer.ReturnInstance(_currentDice);

            var clones = GameObject.FindGameObjectsWithTag("dice");
            foreach (var clone in clones)
            {
                _diceContainer.ReturnInstance(clone.GetComponent<DiceController>());
            }

            _board.Reset();
            _gameProcessManager.ResetScore();
            IsRotateDice = false;
            IsRotateCharacter = false;
            _isGameovered = false;
        }
    }

}