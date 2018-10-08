using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SumScoreTextController : MonoBehaviour {
    MainGameController script;
    public Text text;
	// Use this for initialization
	void Start () {
        script = GameObject.Find("Board").GetComponent<MainGameController>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = script.sumScore.ToString();
	}
}
