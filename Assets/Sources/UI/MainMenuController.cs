using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    GameObject bgImage;
    [SerializeField] GameObject StageDice, Up, Down, Left, Right, rankImg;
    int StageNum = 1;
    private int frame = 0;
    // Use this for initialization
    void Start()
    {
        bgImage = GameObject.Find("bgImage");

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
        Texture2D texture = Resources.Load("ranks/" + getString()) as Texture2D;
        rankImg.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

    }

    // Update is called once per frame
    void Update()
    {
        bgImage.transform.Rotate(0f, 0f, Time.deltaTime * 2.0f);
    }

    public void showRanking()
    {
        naichilab.RankingLoader.Instance.ShowRanking();
    }
    string getString()
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


 