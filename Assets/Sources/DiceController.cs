using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {

	GameObject Board;
	MainGameController script;

	Vector3 rotatePoint = Vector3.zero;
	Vector3 rotateAxis = Vector3.zero;
	float cubeAngle = 0f;

	float cubeSizeHalf;
	bool isRotate = false;

	public int X = 0, Z = 0;
	public int diceId = 0; //!サイコロのID
	public int surfaceA = 1;
	public int surfaceB = 2;
	float step = 2f;

	// Use this for initialization
	void Start () {
		Board = GameObject.Find ("Board");
		script = Board.GetComponent<MainGameController>();
		cubeSizeHalf = transform.localScale.x / 2f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isRotate)
		{
			SetTargetPosition ();
		}
	}

	void SetTargetPosition(){

		if (Input.GetKey (KeyCode.RightArrow)) {
			if (X+1 < script.board.GetLength(0) && script.board[X+1, Z] == -1)
			{
				script.board[X, Z] = -1;
				X += 1;
				script.board[X, Z] = diceId;
				rotatePoint = transform.position + new Vector3(cubeSizeHalf, -cubeSizeHalf, 0f);
				rotateAxis = new Vector3 (0, 0, -1);
				StartCoroutine (MoveCube());
			}
			return;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			if (0 <= X-1 && script.board[X-1, Z] == -1)
			{
				script.board[X, Z] = -1;
				X -= 1;
				script.board[X, Z] = diceId;
				rotatePoint = transform.position + new Vector3(-cubeSizeHalf, -cubeSizeHalf, 0f);
				rotateAxis = new Vector3 (0, 0, 1);
				StartCoroutine (MoveCube());
			}
			return;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			if (Z+1 < script.board.GetLength(1) && script.board[X, Z+1] == -1)
			{
				script.board[X, Z] = -1;
				Z += 1;
				script.board[X, Z] = diceId;
				rotatePoint = transform.position + new Vector3(0f, -cubeSizeHalf, cubeSizeHalf);
				rotateAxis = new Vector3 (1, 0, 0);
				StartCoroutine (MoveCube());
			}
			return;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			if (0 <= Z-1 && script.board[X, Z-1] == -1)
			{
				script.board[X, Z] = -1;
				Z -= 1;
				script.board[X, Z] = diceId;
				rotatePoint = transform.position + new Vector3(0f, -cubeSizeHalf, -cubeSizeHalf);
				rotateAxis = new Vector3 (-1, 0, 0);
				StartCoroutine (MoveCube());
			}
			return;
		}
	}

	IEnumerator MoveCube(){
		isRotate = true;

		float sumAngle = 0f;
		while (sumAngle < 90f) {
			cubeAngle = 15f;
			sumAngle += cubeAngle;

			if (sumAngle > 90f)
			{
				cubeAngle -= sumAngle - 90f;
			}
			transform.RotateAround (rotatePoint, rotateAxis, cubeAngle);

			yield return null;
		}

		isRotate = false;

		yield break;
	}

    /**
     * さいころの各状態と入力からさいころの次の状態を求める
     * @param int a 面Aの数字
     * @param int b 面Bの数字
     * @param string direction 入力方向(up/down/left/right)
     * @return int 2桁の数字(56なら面Aが5, 面Bが6)
     */
     int computeNextDice(int a, int b, string direction)
    {
        int nextA = 0, nextB = 0;
        switch (a)
        {
            case 1:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 6;
                        break;
                    case "down":
                        nextA = 7-b;
                        nextB = 1;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 2:
                                nextA = 3;
                                break;
                            case 3:
                                nextA = 5;
                                break;
                            case 4:
                                nextA = 2;
                                break;
                            case 5:
                                nextA = 4;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
                        //ここから

                        break; 
                }
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;

        }

        return nextA * 10 + nextB;
    }

}
