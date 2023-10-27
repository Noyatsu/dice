﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SumScoreTextController : MonoBehaviour {
	private MainGameController _script;
    [FormerlySerializedAs("text")] public Text Text;
	// Use this for initialization
	private void Start () {
        _script = GameObject.Find("Board").GetComponent<MainGameController>();
	}
	
	// Update is called once per frame
	private void Update () {
        Text.text = _script.SumScore.ToString();
	}
}
