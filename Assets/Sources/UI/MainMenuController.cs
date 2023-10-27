using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    GameObject _bgImage;
    [FormerlySerializedAs("StageDice")] [SerializeField] GameObject _stageDice;
    [FormerlySerializedAs("Up")] [SerializeField] GameObject _up;
    [FormerlySerializedAs("Down")] [SerializeField] GameObject _down;
    [FormerlySerializedAs("Left")] [SerializeField] GameObject _left;
    [FormerlySerializedAs("Right")] [SerializeField] GameObject _right;
    [FormerlySerializedAs("rankImg")] [SerializeField] GameObject _rankImg;
    int _stageNum = 1;
    private int _frame = 0;
    // Use this for initialization
    void Start()
    {
        _bgImage = GameObject.Find("bgImage");

        // 初回起動時はチュートリアルへ
        if (!PlayerPrefs.HasKey("userName"))
        {
            BgmManager.Instance.Play("online");
            SceneManager.LoadScene("Init");
        }
        else
        {
            BgmManager.Instance.Play("opening");
        }

        //ランクを表示
        Texture2D texture = Resources.Load("ranks/" + GetString()) as Texture2D;
        _rankImg.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    }

    // Update is called once per frame
    void Update()
    {
        _bgImage.transform.Rotate(0f, 0f, Time.deltaTime * 2.0f);
    }

    public void ShowRanking()
    {
        naichilab.RankingLoader.Instance.ShowRanking();
    }
    string GetString()
    {
        int i = PlayerPrefs.GetInt("rank", 0) / 100;

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


 