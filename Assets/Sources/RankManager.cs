using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/**
 * ランク機能を実装
 */

public class RankManager : MonoBehaviour
{
    [FormerlySerializedAs("initType")] [SerializeField]
    private int _initType = 2; // 勝利->1, 敗北->0
    [FormerlySerializedAs("rankText")] [SerializeField]
    private GameObject _rankText;
    [FormerlySerializedAs("rankImg")] [SerializeField]
    private GameObject _rankImg;

    private int _nowRank = 0;
    // Start is called before the first frame update
    private void Start()
    {
        _nowRank = PlayerPrefs.GetInt("rank", 0);

        //起動時処理タイプが設定されていれば実行
        if (_initType == 0 || _initType == 1)
        {
            ChangeRank(_initType);
            ShowRank();
            if (_initType == 1) {
                BgmManager.Instance.Play("win");
            }
            else {
                BgmManager.Instance.Play("lose");
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void ShowRank()
    {
        _rankText.GetComponent<Text>().text = (_nowRank % 100).ToString() + "/100";
        Texture2D texture = Resources.Load("ranks/" + GetString()) as Texture2D;
        _rankImg.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    // 勝利->1, 敗北->0
    private int ChangeRank(int type)
    {
        int point = 0;
        int i = _nowRank / 100;

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
            _nowRank += point;
        }
        else if (type == 0)
        {
            //敗北
            _nowRank -= (int)(point * 0.8);
        }
        else
        {
            return _nowRank;
        }

        // ゼロ以下ならゼロに丸める
        if (_nowRank < 0) _nowRank = 0;

        // 900以上なら900に丸める
        if (_nowRank > 900) _nowRank = 900;

        // 記録
        PlayerPrefs.SetInt("rank", _nowRank);
        PlayerPrefs.Save();

        Debug.Log(_nowRank);

        return _nowRank;
    }

    private string GetString()
    {
        int i = _nowRank / 100;

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
