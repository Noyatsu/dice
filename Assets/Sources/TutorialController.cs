using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    string[,] ttData = {
        {"", "", "Six sided travelerへようこそ！ "},
        {"", "", "これは、砂漠にすんでいた女の子\"Zoro\"が六面世界を旅する物語です。"},
        {"", "", "早速、旅の仕方をマスターしましょう。"},
        {"", "", "画面をタップして進みたい方向にスライドするとその方向に移動できます。"},
        {"2,0.0,2", "-1", "早速、やってみてください。\n[マスターしたら次に進みましょう。]"},
        {"2,0.0,1", "2,2,1,2", "地面からさいころへは上がることができますが、一度さいころに乗ると降りることはできません。"},
        {"", "-1", "Zoroは冒険好きですが、高所恐怖症なのかもしれませんね。"},
        {"", "", "さいころを目の数だけ隣接させるとさいころを消すことができます。"},
        {"2,0.0,0", "1,2,2,0,2,1,1,2", "手前のさいころに乗って、1つ奥に動かしてみてください。"},
        {"2,0.0,0", "1,2,2,0,2,1,1,2", "できましたか？沈み途中のさいころからは降りることができます。"},
        {"", "", "また，沈んでいる途中にさいころが青くなります。"},
        {"", "", "青いさいころがある場所には，他のさいころを上から潰すように転がすことができます。"},
        {"2,0.0,0", "1,2,2,0,2,1,1,2,2,3,5,0", "[マスターしたら次に進みましょう。]"},
        {"3,0.0,1", "2,3,5,0,3,2,1,5,4,3,5,0,5,3,5,0,6,3,5,0", "これもできますか？\n[マスターしたら次に進みましょう。]"},
        {"", "", "画面左上のスコアに注目してください。スコアは 目の数×数 だけ加点されます。"},
        {"", "", "今回は 5の目 を 5個 消したので、\n5×5=25点加点されています。"},
        {"", "", "続いて、冒険のキモとなる 連鎖 について説明します。"},
        {"", "", "簡単に言うと、連鎖とは沈んでいる最中のさいころに同じ目を隣接させることです。"},
        {"", "", "実際にやってみましょう。"},
        {"3,0.0,1", "3,2,5,3,4,3,2,3,2,4,3,0,3,4,3,0", "手前の左のさいころに乗って、1つ奥に動かした後、右のさいころを奥に動かしてみましょう。"},
        {"3,0.0,1", "3,2,5,3,4,3,2,3,2,4,3,0,3,4,3,0", "できましたか？\n[マスターしたら次に進みましょう。]"},
        {"", "", "注意点ですが、沈み途中のさいころがその数以上にないと連鎖しません。"},
        {"", "", "連鎖時の得点は、連鎖時点のつながっている数 × その目の数 となるので、"},
        {"", "", "今の場合は3×3+3×4=21点となります。"},
        {"", "", "また、ボーナスステージでは得点が倍になります。"},
        {"1,0.0,0", "0,2,2,0,1,1,1,2", "ここで2を2つ消した場合、スコアは2×2=4となるはずですが"},
        {"", "", "8になっているのはステージボーナスのためです。((2×2)×2=8)"},
        {"", "", "ステージはスコアがたまるごとに変わっていきます。"},
        {"", "", "ところで、ここまで話を聞いてきた方で「1の目は消せないのかな」と思っている方もいるでしょう。"},
        {"3,0.0,1", "-1", "安心して下さい。消すことができます。"},
        {"3,0.0,1", "3,2,5,3,4,3,2,1,2,4,3,0,3,4,3,0,2,0,1,0,3,6,1,0", "1の目を、何かしらの沈み途中のさいころに隣接させるとその1の目以外の1が消えます。"},
        {"3,0.0,1", "3,2,5,3,4,3,2,1,2,4,3,0,3,4,3,0,2,0,1,0,3,6,1,0", "これを「ワンゾロバニッシュ!」といいます。"},
        {"3,0.0,1", "3,2,5,3,4,3,2,1,2,4,3,0,3,4,3,0,2,0,1,0,3,6,1,0", "手前の5を転がして3を消してから、右の1の目を奥に転がしてみましょう。"},
        {"", "", "転がした1の目以外の1が消え始めたのがわかります。\n[マスターしたら次に進みましょう。]"},
        {"", "", "全てのマスがさいころで埋まるとゲームオーバーとなります。"},
        {"", "", "以上が基本的な冒険の方法となります。"},
        {"2,0.0,1", "-1", "ここからは、ワンポイントアドバイスです。"},
        {"2,0.0,1", "2,2,2,0,4,2,2,3", "例えばこのような時に、2を隣接させて消したいですよね？"},
        {"2,0.0,1", "2,2,2,0,4,2,2,3", "このような時には、動かしたいさいころに乗っていったん手前に倒してから、"},
        {"", "", "横に移動し、また元に戻ると平行移動が行えます。"},
        {"2,0.0,1", "2,2,2,0,4,2,2,3", "できましたか？\n[マスターしたら次に進みましょう。]"},
        {"", "", "これでチュートリアルを終わります。"},
        {"", "", "Zoroと一緒に6面世界を冒険してみましょう。"},
    };



    GameObject objTT, objBoard;
    TutorialTextController objTTController;
    MainGameController objMGController;
    int nowDataIdx = 0;
    int maxDataIdx;

    // Use this for initialization
    void Start()
    {
        ttData[0, 2] += PlayerPrefs.GetString("userName") + "さん。";

        objBoard = GameObject.Find("Board");
        objMGController = objBoard.GetComponent<MainGameController>();

        objTT = GameObject.Find("TutorialText");
        objTTController = objTT.GetComponent<TutorialTextController>();

        maxDataIdx = ttData.GetLength(0);
        setNext();
    }

    public void setNext()
    {
        if (nowDataIdx < maxDataIdx)
        {
            objTTController.setText(ttData[nowDataIdx, 2]);

            // キャラを移動
            if (ttData[nowDataIdx, 0] != "")
            {
                string[] aquiPos = ttData[nowDataIdx, 0].Split(',');
                objMGController.setAqui(int.Parse(aquiPos[0]), float.Parse(aquiPos[1]), int.Parse(aquiPos[2]));
            }

            if (ttData[nowDataIdx, 1] != "")
            {
                objMGController.resetGame();
                if (ttData[nowDataIdx, 1] != "-1")
                {
                    string[] dicePos = ttData[nowDataIdx, 1].Split(',');
                    for (int i = 0; i < dicePos.Length / 4; i++)
                    {
                        objMGController.diceGenerate(int.Parse(dicePos[4 * i]), int.Parse(dicePos[4 * i + 1]), int.Parse(dicePos[4 * i + 2]), int.Parse(dicePos[4 * i + 3]));
                    }
                }
            }
            nowDataIdx++;
        }
        else if (nowDataIdx == maxDataIdx)
        {
            gotoTopmenu();
        }
    }

    public void gotoTopmenu()
    {
        FadeManager.Instance.LoadScene("TopMenu", 0.3f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
