using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameController : MonoBehaviour {

	public int[,] board = new int[10, 10];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < board.GetLength(0); i++)
		{
				for (int j = 0; j < board.GetLength(1); j++)
				{
						board[i, j] = -1;
				}
		}
		board[0, 0] = 0;
		board[4, 4] = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
