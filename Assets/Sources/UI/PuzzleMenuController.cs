using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMenuController : MonoBehaviour
{
    [SerializeField] int _worldNum;
    public static int worldNum, stageNum;

    public static int getStageIdx()
    {
        return 8 * (worldNum - 1) + stageNum - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        worldNum = _worldNum;

        GameObject button;
        if (worldNum > 1) {
            if (PlayerPrefs.GetInt("puzzle"+(worldNum-1).ToString()+"-8", 0) == 0) {
                button = GameObject.Find("1");
                button.GetComponent<Button>().interactable = false;
            }
        }
        for (int i = 1; i < 8; i++) {
            button = GameObject.Find((i+1).ToString());
            if (PlayerPrefs.GetInt("puzzle"+worldNum.ToString()+"-"+i.ToString(), 0) == 0) {
                button.GetComponent<Button>().interactable = false;
            }
            else {
                button.GetComponent<Button>().interactable = true;
            }
            Debug.Log(worldNum.ToString()+"-"+i.ToString());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackButton()
    {
        FadeManager.Instance.LoadScene("TopMenu", 0.3f);
        BgmManager.Instance.Play("opening"); //BGM
    }

    public void StageSelect(int stage)
    {
        stageNum = stage;
        FadeManager.Instance.LoadScene("PuzzleGame", 0.3f);
    }
}
