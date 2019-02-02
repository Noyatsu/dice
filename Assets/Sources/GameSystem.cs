using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{

    public GameObject objNowLoading;

    public void OnePlayerGameStart()
    {
        objNowLoading.SetActive(true);
        FadeManager.Instance.LoadScene("1PlayerGame", 0.3f);
    }

    public void OnlineMenuStart()
    {
        FadeManager.Instance.LoadScene("OnlineMenu", 0.3f);
    }

    public void ReturnTitle()
    {
        GameObject Board = GameObject.Find("Board");
        Destroy(Board);
        FadeManager.Instance.LoadScene("TopMenu", 0.3f);
    }
}
