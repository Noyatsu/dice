using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class FinalScoreText : MonoBehaviour {
    GameObject _board;
    MainGameController _script;

    [FormerlySerializedAs("text")] public Text Text;


    // Use this for initialization
    void Start () {
        // Board = GameObject.Find("Board");
        // script = Board.GetComponent<MainGameController>();
				// text.text = "スコア: " + script.score;
				Text.text = "スコア: " + MainGameController.Score;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
