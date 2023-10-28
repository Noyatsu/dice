using System.Collections;
using System.Collections.Generic;
using SSTraveler.Game;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SSTraveler.Ui
{
    public class LevelText : MonoBehaviour
    {
        private GameObject _board;
        private MainGameController _script;

        [FormerlySerializedAs("text")] public Text Text;


        // Use this for initialization
        private void Start()
        {
            _board = GameObject.Find("Board");
            _script = _board.GetComponent<MainGameController>();

        }

        // Update is called once per frame
        private void Update()
        {
            Text.text = "Level " + _script.Level;
        }
    }
}