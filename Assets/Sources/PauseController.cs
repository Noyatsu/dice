using UnityEngine;
using System.Collections;

public class PauseController : MonoBehaviour
{

    [SerializeField]
    //　ポーズした時に表示するUIのプレハブ
    private GameObject pauseUIPrefab;
    //　ポーズUIのインスタンス
    private GameObject pauseUIInstance;

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
        if (pauseUIInstance == null)
        {
            pauseUIInstance = GameObject.Instantiate(pauseUIPrefab) as GameObject;
            //Time.timeScale = 0f;

        }
        else
        {
            Destroy(pauseUIInstance);
            //Time.timeScale = 1f;
        }
    }
}