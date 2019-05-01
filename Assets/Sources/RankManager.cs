using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * ランク機能を実装
 */

public class RankManager : MonoBehaviour
{
    [SerializeField] int initType = 2; // 勝利->1, 敗北->0
    [SerializeField] GameObject rankText, rankImg;

    int nowRank = 0;
    // Start is called before the first frame update
    void Start()
    {
        nowRank = PlayerPrefs.GetInt("rank", 0);

        //起動時処理タイプが設定されていれば実行
        if (initType == 0 || initType == 1)
        {
            changeRank(initType);
            showRank();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void showRank()
    {
        rankText.GetComponent<Text>().text = (nowRank % 100).ToString() + "/100";
        Texture2D texture = Resources.Load("ranks/" + getString()) as Texture2D;
        rankImg.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    // 勝利->1, 敗北->0
    int changeRank(int type)
    {
        int point = 0;
        int i = nowRank / 100;

        if (i < 1) point = 20; //C-
        else if (i < 2) point = 12; //C+
        else if (i < 6) point = 10; //A+
        else if (i < 7) point = 4; //S-
        else if (i < 8) point = 3; //S+
        else if (i < 9) point = 2; //Master

        // 勝ったならプラス，負けたならマイナス
        if (type == 1)
        {
            //勝利
            nowRank += point;
        }
        else if (type == 0)
        {
            //敗北
            nowRank -= (int)(point * 0.8);
        }
        else
        {
            return nowRank;
        }

        // ゼロ以下ならゼロに丸める
        if (nowRank < 0) nowRank = 0;

        // 900以上なら900に丸める
        if (nowRank > 900) nowRank = 900;

        // 記録
        PlayerPrefs.SetInt("rank", nowRank);
        PlayerPrefs.Save();

        Debug.Log(nowRank);

        return nowRank;
    }

    string getString()
    {
        int i = nowRank / 100;

        if (i < 1) return "C-";
        else if (i < 2) return "C+";
        else if (i < 3) return "B-";
        else if (i < 4) return "B+";
        else if (i < 5) return "A-";
        else if (i < 6) return "A+";
        else if (i < 7) return "S-";
        else if (i < 8) return "S+";
        else if (i < 9) return "Master";
        else return "Grand-Master";
    }
}
