using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScoreText : MonoBehaviour {

    OnlineGameController objOnlineGameController;
    public Text text;


    // Use this for initialization
    void Start()
    {
        objOnlineGameController = GameObject.Find("OnlineGameController").GetComponent<OnlineGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = objOnlineGameController.enemyScore.ToString();
    }

}
