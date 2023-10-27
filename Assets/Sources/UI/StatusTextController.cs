using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class StatusTextController : MonoBehaviour {
    GameObject _board;
    MainGameController _script;

    int _lastFlame = 0;
    [FormerlySerializedAs("text")] public Text Text;


    // Use this for initialization
    void Start () {
        _board = GameObject.Find("Board");
        _script = _board.GetComponent<MainGameController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (_lastFlame > 0)
        {
            _lastFlame--;
        }
        else if (_lastFlame == 0)
        {
            Text.text = "";
        }
    }

    public void SetText(string msg)
    {
        _lastFlame = 100;
        Text.text = msg;
    }
}
