using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyScoreText : MonoBehaviour {

    OnlineGameController _objOnlineGameController;
    [FormerlySerializedAs("text")] public Text Text;


    // Use this for initialization
    void Start()
    {
        _objOnlineGameController = GameObject.Find("OnlineGameController").GetComponent<OnlineGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = _objOnlineGameController.EnemyScore.ToString();
    }

}
