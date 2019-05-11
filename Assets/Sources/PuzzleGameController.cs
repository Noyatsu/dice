using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGameController : MonoBehaviour
{
    /**
     * {指定ターン数, プレイヤーの初期xyz, サイコロの(x,z,A面の数字,B面の数字(0にするとランダム))xサイコロの数だけ書く}
     * 全てstring型
     */
    string[,] ttData = {
        {"1", "3,1.0,2", "3,2,2,1,2,3,4,1,3,3,4,2,4,3,4,5"},
        {"3", "4,1.0,2", "4,2,5,1,2,2,6,3,3,3,3,5"},
        {"5", "3,1.0,1", "2,1,6,4,3,2,4,1,3,1,1,2,4,2,4,6"},
        {"7", "2,1.0,1", "2,1,2,3,4,1,6,2,4,2,2,4,2,3,6,2,3,3,6,3,4,3,6,5,3,4,4,2"},
        {"7", "4,1.0,2", "4,2,1,2,4,4,3,1,2,4,6,5,2,2,4,1,3,3,2,6"},
        {"3", "2,1.0,2", "2,2,2,6,4,2,2,4"},
        {"5", "5,1.0,1", "5,1,1,2,1,3,2,6"},
        {"5", "4,1.0,3", "4,3,2,1,2,3,4,1,3,3,4,2,2,2,5,4"},
      　/*{"4", "2,0.0,1", "2,2,2,0,4,2,2,3"},
        {"4", "3,0.0,1", "3,2,5,3,4,3,2,3,2,4,3,0,3,4,3,0"},
        {"2", "3,0.0,1", "2,3,5,0,3,2,1,5,4,3,5,0,5,3,5,0,6,3,5,0"},*/
    };



    [SerializeField] GameObject objBoard, gobjStageText, gobjRemainText, gobjYouWin, gobjYouLose;
    MainGameController objMGController;
    int stageIdx, ttsize, remainTurnNum;
    bool winFlag = false, loseFlag = false;

    // Use this for initialization
    void Start()
    {
        objMGController = objBoard.GetComponent<MainGameController>();

        ttsize = ttData.GetLength(0);
        stageIdx = PuzzleMenuController.getStageIdx(); // ステージIDを取得
        setStage();
    }

    public void setStage()
    {
        gobjYouLose.SetActive(false);
        winFlag = false;
        loseFlag = false;

        if (stageIdx >= ttsize)
        {
            stageIdx = ttsize - 1;
        }
        // テキスト設定
        gobjStageText.GetComponent<Text>().text = (1 + stageIdx / 8).ToString() + " - " + (stageIdx % 8 + 1).ToString();

        // BGM再生/背景設定
        objMGController.changeStage(stageIdx / 8);

        // 指定ターン数
        remainTurnNum = int.Parse(ttData[stageIdx, 0]);
        gobjRemainText.GetComponent<Text>().text = remainTurnNum.ToString();

        // キャラを移動
        if (ttData[stageIdx, 1] != "")
        {
            string[] aquiPos = ttData[stageIdx, 1].Split(',');
            objMGController.setAqui(int.Parse(aquiPos[0]), float.Parse(aquiPos[1]), int.Parse(aquiPos[2]));
        }

        // サイコロを生やす
        if (ttData[stageIdx, 2] != "")
        {
            objMGController.resetGame();
            if (ttData[stageIdx, 2] != "-1")
            {
                string[] dicePos = ttData[stageIdx, 2].Split(',');
                for (int i = 0; i < dicePos.Length / 4; i++)
                {
                    objMGController.diceGenerate(int.Parse(dicePos[4 * i]), int.Parse(dicePos[4 * i + 1]), int.Parse(dicePos[4 * i + 2]), int.Parse(dicePos[4 * i + 3]));
                }
            }
        }

    }

    public void decrementRemainTurnNum()
    {
        if (remainTurnNum > 0)
        {
            remainTurnNum--;
            gobjRemainText.GetComponent<Text>().text = remainTurnNum.ToString();

            int flag = 0;
            foreach (GameObject gobj in objMGController.dices)
            {
                if (!gobj.GetComponent<DiceController>().isVanishing)
                {
                    flag++;
                }
            }

            if (!loseFlag && flag == 0)
            {
                youWin();
            }

            if (!winFlag && remainTurnNum <= 0)
            {
                youLose();
            }
        }
        showArraylog();

    }


    void showArraylog()
    {
        string str = "";
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                str += objMGController.board[i, j].ToString() + ",";
            }
            str += "\n";
        }
        Debug.Log(str);

    }


    public void youWin()
    {
        gobjYouWin.SetActive(true);
        winFlag = true;
    }

    public void youLose()
    {
        gobjYouLose.SetActive(true);
        loseFlag = true;
    }

    public void gotoTopmenu()
    {
        BgmManager.Instance.Play("puzzle"); //BGM
        FadeManager.Instance.LoadScene("stage1", 0.3f);
    }

    public void nextStage()
    {
        gobjYouWin.SetActive(false);
        stageIdx++;
        setStage();
    }


    // Update is called once per frame
    void Update()
    {

    }
}
