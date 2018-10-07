using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour {

    public GameObject objNowLoading;

	public void OnePlayerGameStart() {
        objNowLoading.SetActive(true);
		SceneManager.LoadScene ("1PlayerGame");
	}

    public void OnlineMenuStart()
    {
        SceneManager.LoadScene("OnlineMenu");
    }

    public void ReturnTitle() {
		GameObject Board = GameObject.Find ("Board");
		Destroy(Board);
		SceneManager.LoadScene ("TopMenu");
	}
}
