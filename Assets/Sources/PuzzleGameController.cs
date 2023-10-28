using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SSTraveler.Game
{

    public class PuzzleGameController : MonoBehaviour
    {
        // 現在用意できている最後のステージ
        private int _maxStageIdx = 7; // (worldnum-1)*8 + (stageNum-1) で計算, 例えば2-3なら(2-1)*8 + (3-1) = 10

        /**
         * {指定ターン数, プレイヤーの初期xyz, サイコロの(x,z,A面の数字,B面の数字(0にするとランダム))xサイコロの数だけ書く}
         * 全てstring型
         */
        private string[,] _ttData =
        {
            // stage1
            { "1", "3,1.0,2", "3,2,2,1,2,3,4,1,3,3,4,2,4,3,4,5" },
            { "3", "4,1.0,2", "4,2,5,1,2,2,6,3,3,3,3,5" },
            { "5", "3,1.0,1", "2,1,6,4,3,2,4,1,3,1,1,2,4,2,4,6" },
            { "7", "2,1.0,1", "2,1,2,3,4,1,6,2,4,2,2,4,2,3,6,2,3,3,6,3,4,3,6,5,3,4,4,2" },
            { "7", "4,1.0,2", "4,2,1,2,4,4,3,1,2,4,6,5,2,2,4,1,3,3,2,6" },
            { "3", "2,1.0,2", "2,2,2,6,4,2,2,4" },
            { "5", "5,1.0,1", "5,1,1,2,1,3,2,6" },
            { "5", "4,1.0,3", "4,3,2,1,2,3,4,1,3,3,4,2,2,2,5,4" },

            // stage2


            /*{"4", "2,0.0,1", "2,2,2,0,4,2,2,3"},
             {"4", "3,0.0,1", "3,2,5,3,4,3,2,3,2,4,3,0,3,4,3,0"},
             {"2", "3,0.0,1", "2,3,5,0,3,2,1,5,4,3,5,0,5,3,5,0,6,3,5,0"},*/
        };




        [FormerlySerializedAs("objBoard")] [SerializeField]
        private GameObject _objBoard;

        [FormerlySerializedAs("gobjStageText")] [SerializeField]
        private GameObject _gobjStageText;

        [FormerlySerializedAs("gobjRemainText")] [SerializeField]
        private GameObject _gobjRemainText;

        [FormerlySerializedAs("gobjYouWin")] [SerializeField]
        private GameObject _gobjYouWin;

        [FormerlySerializedAs("gobjYouLose")] [SerializeField]
        private GameObject _gobjYouLose;

        private MainGameController _objMgController;
        private int _stageIdx, _ttsize, _remainTurnNum;
        private bool _winFlag = false, _loseFlag = false;
        private string _strStage = ""; //1-1みたいな
        
        private IDiceContainer _diceContainer;
        private IBoard _board;
        
        [Inject]
        public void Construct(IDiceContainer diceContainer, IBoard board)
        {
            _diceContainer = diceContainer;
            _board = board;
        }
        

        // Use this for initialization
        private void Start()
        {
            _objMgController = _objBoard.GetComponent<MainGameController>();

            _ttsize = _ttData.GetLength(0);
            _stageIdx = PuzzleMenuController.GetStageIdx(); // ステージIDを取得
            SetStage();
        }

        public void SetStage()
        {
            _gobjYouLose.SetActive(false);
            _winFlag = false;
            _loseFlag = false;

            if (_stageIdx >= _ttsize)
            {
                _stageIdx = _ttsize - 1;
            }

            // テキスト設定
            _strStage = (1 + _stageIdx / 8).ToString() + " - " + (_stageIdx % 8 + 1).ToString();
            _gobjStageText.GetComponent<Text>().text = _strStage;

            // BGM再生/背景設定
            _objMgController.ChangeStage(_stageIdx / 8);

            // 指定ターン数
            _remainTurnNum = int.Parse(_ttData[_stageIdx, 0]);
            _gobjRemainText.GetComponent<Text>().text = _remainTurnNum.ToString();

            // キャラを移動
            if (_ttData[_stageIdx, 1] != "")
            {
                string[] aquiPos = _ttData[_stageIdx, 1].Split(',');
                _objMgController.SetAquiDiscrete(int.Parse(aquiPos[0]), float.Parse(aquiPos[1]), int.Parse(aquiPos[2]));
            }

            // サイコロを生やす
            if (_ttData[_stageIdx, 2] != "")
            {
                _objMgController.ResetGame();
                if (_ttData[_stageIdx, 2] != "-1")
                {
                    string[] dicePos = _ttData[_stageIdx, 2].Split(',');
                    for (int i = 0; i < dicePos.Length / 4; i++)
                    {
                        _objMgController.DiceGenerate(int.Parse(dicePos[4 * i]), int.Parse(dicePos[4 * i + 1]), int.Parse(dicePos[4 * i + 2]), int.Parse(dicePos[4 * i + 3]));
                    }
                }
            }

        }

        public void DecrementRemainTurnNum()
        {
            if (_remainTurnNum > 0)
            {
                _remainTurnNum--;
                _gobjRemainText.GetComponent<Text>().text = _remainTurnNum.ToString();

                int flag = 0;
                foreach (var dice in _diceContainer.DicePool)
                {
                    if (dice.gameObject.activeSelf && !dice.IsVanishing)
                    {
                        flag++;
                    }
                }

                if (!_loseFlag && flag == 0)
                {
                    YouWin();
                }

                if (!_winFlag && _remainTurnNum <= 0)
                {
                    YouLose();
                }
            }

            ShowArraylog();

        }


        private void ShowArraylog()
        {
            string str = "";
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    str += _board[i, j].DiceNum + ",";
                }

                str += "\n";
            }

            Debug.Log(str);

        }


        public void YouWin()
        {
            _gobjYouWin.SetActive(true);
            _winFlag = true;

            // セーブ puzzle1-1 = 1 みたいな
            if (_stageIdx != _maxStageIdx)
            {
                PlayerPrefs.SetInt("puzzle" + _strStage.Replace(" ", ""), 1);
                Debug.Log(_strStage);
            }
            else
            {
                GameObject.Find("NextButton").GetComponent<Button>().interactable = false;
            }
        }

        public void YouLose()
        {
            _gobjYouLose.SetActive(true);
            _loseFlag = true;

            // セーブ
            //PlayerPrefs.SetInt("puzzle" + strStage.Replace(" ", ""), 0);
        }

        public void GotoTopmenu()
        {
            BgmManager.Instance.Play("puzzle"); //BGM
            FadeManager.Instance.LoadScene("stage" + (1 + _stageIdx / 8).ToString(), 0.3f);
        }

        public void NextStage()
        {
            _gobjYouWin.SetActive(false);
            _stageIdx++;
            SetStage();
        }
    }
}