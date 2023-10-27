using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyScoreText : MonoBehaviour {
    private OnlineGameController _objOnlineGameController;
    [FormerlySerializedAs("text")] public Text Text;


    // Use this for initialization
    private void Start()
    {
        _objOnlineGameController = GameObject.Find("OnlineGameController").GetComponent<OnlineGameController>();
    }

    // Update is called once per frame
    private void Update()
    {
        Text.text = _objOnlineGameController.EnemyScore.ToString();
    }

}
