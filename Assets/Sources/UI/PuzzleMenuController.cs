using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleMenuController : MonoBehaviour
{
    public static int worldNum, stageNum;

    public static int getStageIdx()
    {
        return 8 * (worldNum - 1) + stageNum - 1;
    }

    // Start is called before the first frame update
    void Start()
    {

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

    public void StageSelect(string worldAndStage)
    {
        string[] arr = worldAndStage.Split('-');
        worldNum = int.Parse(arr[0]);
        stageNum = int.Parse(arr[1]);
        FadeManager.Instance.LoadScene("PuzzleGame", 0.3f);
    }
}
