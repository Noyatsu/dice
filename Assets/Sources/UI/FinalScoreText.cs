using System.Collections;
using System.Collections.Generic;
using SSTraveler.Game;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SSTraveler.Ui
{
	public class FinalScoreText : MonoBehaviour
	{
		private GameObject _board;
		private MainGameController _script;

		[FormerlySerializedAs("text")] public Text Text;


		// Use this for initialization
		private void Start()
		{
			// Board = GameObject.Find("Board");
			// script = Board.GetComponent<MainGameController>();
			// text.text = "スコア: " + script.score;
			// Text.text = "スコア: " + MainGameController.Score;
		}

		// Update is called once per frame
		private void Update()
		{

		}
	}
}