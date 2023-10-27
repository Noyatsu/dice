using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class PauseController : MonoBehaviour
{
    //　ポーズUIのインスタンス
    [FormerlySerializedAs("pauseUIInstance")] [SerializeField] private GameObject _pauseUIInstance;
    private bool _isPause = false;

    private void Start()
    {
        _pauseUIInstance.SetActive(_isPause);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            Pause();
        }
    }

    public void Pause()
    {
        if (!_isPause)
        {
            _pauseUIInstance.SetActive(true);
            _isPause = true;
            Time.timeScale = 0f;

        }
        else
        {
            _pauseUIInstance.SetActive(false);
            _isPause = false;
            Time.timeScale = 1f;
        }
    }

    public void ReturnTitle()
    {
        Time.timeScale = 1f;
        FadeManager.Instance.LoadScene("TopMenu", 0.3f);
    }
}