using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMenuController : MonoBehaviour
{
    [SerializeField] int _worldNum;
    public static int WorldNum, StageNum;

    public static int GetStageIdx()
    {
        return 8 * (WorldNum - 1) + StageNum - 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        WorldNum = _worldNum;

        GameObject button;
        if (WorldNum > 1) {
            if (PlayerPrefs.GetInt("puzzle"+(WorldNum-1).ToString()+"-8", 0) == 0) {
                button = GameObject.Find("1");
                button.GetComponent<Button>().interactable = false;
            }
        }
        for (int i = 1; i < 8; i++) {
            button = GameObject.Find((i+1).ToString());
            if (PlayerPrefs.GetInt("puzzle"+WorldNum.ToString()+"-"+i.ToString(), 0) == 0) {
                button.GetComponent<Button>().interactable = false;
            }
            else {
                button.GetComponent<Button>().interactable = true;
            }
            Debug.Log(WorldNum.ToString()+"-"+i.ToString());
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
        StageNum = stage;
        FadeManager.Instance.LoadScene("PuzzleGame", 0.3f);
    }
}
