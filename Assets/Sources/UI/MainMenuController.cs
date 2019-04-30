using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    GameObject bgImage;
    [SerializeField] GameObject StageDice, Up, Down, Left, Right;
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
}
