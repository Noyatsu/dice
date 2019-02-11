using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{
    //　ポーズUIのインスタンス
    private GameObject pauseUIInstance;
    private bool isPause = false;

    void Start()
    {
        pauseUIInstance = GameObject.Find("PauseUI");
        pauseUIInstance.SetActive(isPause);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            pause();
        }
    }

    public void pause()
    {
        if (!isPause)
        {
            pauseUIInstance.SetActive(true);
            isPause = true;
            Time.timeScale = 0f;

        }
        else
        {
            pauseUIInstance.SetActive(false);
            isPause = false;
            Time.timeScale = 1f;
        }
    }

    public void returnTitle()
    {
        Time.timeScale = 1f;
        FadeManager.Instance.LoadScene("TopMenu", 0.3f);
    }
}