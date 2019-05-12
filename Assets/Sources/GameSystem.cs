using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class GameSystem : MonoBehaviour
{

    public GameObject objNowLoading;

    void Start()
    {
        //Advertisement.Initialize ("41f00082-510f-4b97-829f-89280c5074ad", true);
    }

    public void OnePlayerGameStart()
    {
        objNowLoading.SetActive(true);
        FadeManager.Instance.LoadScene("1PlayerGame", 0.3f);
    }

    public void OnlineMenuStart()
    {
        FadeManager.Instance.LoadScene("OnlineMenu", 0.3f);
        BgmManager.Instance.Play("online"); //BGM
    }

    public void TutorialStart()
    {
        objNowLoading.SetActive(true);
        FadeManager.Instance.LoadScene("Tutorial", 0.3f);
    }

    public void PuzzleStart(int stage)
    {
        objNowLoading.SetActive(true);
        FadeManager.Instance.LoadScene("stage"+stage.ToString(), 0.3f);
        BgmManager.Instance.Play("puzzle"); //BGM

    }

    public void ReturnTitle()
    {
        GameObject Board = GameObject.Find("Board");
        Destroy(Board);
        BgmManager.Instance.Stop();
        ShowRewardedAd();
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                FadeManager.Instance.LoadScene("TopMenu", 0.3f);
                BgmManager.Instance.Play("opening"); //BGM
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                FadeManager.Instance.LoadScene("TopMenu", 0.3f);
                BgmManager.Instance.Play("opening"); //BGM
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }

}
