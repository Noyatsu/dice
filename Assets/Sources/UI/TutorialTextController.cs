using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class TutorialTextController : MonoBehaviour
{
    GameObject _board;
    MainGameController _script;

    [FormerlySerializedAs("text")] public Text Text;


    // Use this for initialization
    void Start()
    {
        _board = GameObject.Find("Board");
        _script = _board.GetComponent<MainGameController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string msg)
    {
        Text.text = msg;
    }
}
