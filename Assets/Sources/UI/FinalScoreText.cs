﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FinalScoreText : MonoBehaviour {
    GameObject Board;
    MainGameController script;

    public Text text;


    // Use this for initialization
    void Start () {
        // Board = GameObject.Find("Board");
        // script = Board.GetComponent<MainGameController>();
				// text.text = "スコア: " + script.score;
				text.text = "スコア: " + MainGameController.score;
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
