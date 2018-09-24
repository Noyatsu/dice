using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour {

	public void OnePlayerGameStart() {
		SceneManager.LoadScene ("1PlayerGame");
	}

	public void ReturnTitle() {
		GameObject Board = GameObject.Find ("Board");
		Destroy(Board);
		SceneManager.LoadScene ("TopMenu");
	}
}
