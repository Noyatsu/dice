using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatusTextController : MonoBehaviour {
    GameObject Board;
    MainGameController script;

    int lastFlame = 0;
    public Text text;


    // Use this for initialization
    void Start () {
        Board = GameObject.Find("Board");
        script = Board.GetComponent<MainGameController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (lastFlame > 0)
        {
            lastFlame--;
        }
        else if (lastFlame == 0)
        {
            text.text = "";
        }
    }

    public void setText(string msg)
    {
        lastFlame = 100;
        text.text = msg;
    }
}
