using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {

	GameObject Board;
	MainGameController script;

	Vector3 MOVEX = new Vector3(1, 0, 0);
	Vector3 MOVEZ = new Vector3(0, 0, 1);

	public int diceId = 0; //!サイコロのID
	public int surfaceA = 1;
	public int surfaceB = 2;
	float step = 2f;
	Vector3 target;
	Vector3 prevPos;
	Animator animator;

	// Use this for initialization
	void Start () {
		target = transform.position;
		animator = GetComponent<Animator> ();
		Board = GameObject.Find ("Board");
		script = Board.GetComponent<MainGameController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position == target)
		{
				SetTargetPosition ();
		}
		Move ();
	}

	void SetTargetPosition(){
		prevPos = target;

		if (Input.GetKey (KeyCode.RightArrow)) {
			if (script.board)
			{
					
			}
			target = transform.position + MOVEX;
			SetAnimationParam (1);
			return;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			target = transform.position - MOVEX;
			SetAnimationParam (2);
			return;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			target = transform.position + MOVEZ;
			SetAnimationParam (3);
			return;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			target = transform.position - MOVEZ;
			SetAnimationParam (0);
			return;
		}
	}

	void SetAnimationParam(int param){
		animator.SetInteger ("WalkParam", param);
	}

	void Move(){
		transform.position = Vector3.MoveTowards (transform.position, target, step * Time.deltaTime);
	}
}
