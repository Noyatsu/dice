﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialTextController : MonoBehaviour
{
    GameObject Board;
    MainGameController script;

    public Text text;


    // Use this for initialization
    void Start()
    {
        Board = GameObject.Find("Board");
        script = Board.GetComponent<MainGameController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setText(string msg)
    {
        text.text = msg;
    }
}
