using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquiController : MonoBehaviour {

    private Animator anim;
    GameObject Board;
    MainGameController script;

    float moveDegree = 1.0f; //<! 移動量
    float step = 2.5f;     //!< 移動速度
    int direction = 2; //!< キャラクターの向き
    Vector3 target;      //!< 入力受付時、移動後の位置を算出して保存
    Vector3 prevPos;     //!< 何らかの理由で移動できなかった場合、元の位置に戻すため移動前の位置を保存

    void Start()
    {
        Board = GameObject.Find("Board");
        script = Board.GetComponent<MainGameController>();
        target = transform.position;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 移動中かどうかの判定。移動中でなければ入力を受付 transform.position == target && 
        if (transform.position == target && script.isRotate == false) {
            SetTargetPosition();
        }
        Move();

        if (Input.GetKey("up") || Input.GetKey("down") || Input.GetKey("right") || Input.GetKey("left")) {
            anim.SetBool("isWalking", true);
        }
        else {
            anim.SetBool("isWalking", false);
        }
    }

    // 入力に応じて移動後の位置を算出
    void SetTargetPosition()
    {
        prevPos = target;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            target.x = (float)(int)(target.x + 0.5f) + moveDegree - 0.5f;
            SetAnimationParam(2);
            return;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            target.x = (float)(int)(target.x + 0.5f) - moveDegree - 0.5f;
            SetAnimationParam(0);
            return;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            target.z = (float)(int)(target.z + 0.5f) + moveDegree - 0.5f;
            SetAnimationParam(1);
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            target.z = (float)(int)(target.z + 0.5f) - moveDegree - 0.5f;
            SetAnimationParam(3);
            return;
        }
    }

    void SetAnimationParam(int d)
    {
        //入力方向と既存の向きが違う場合
        if (d != direction)
        {
            int diff = direction - d;
            float angle;
            if (Mathf.Abs(diff) == 1) {
                angle = 90.0f;
                if (diff > 0) {
                    angle *= -1.0f;
                }
            }
            else if (Mathf.Abs(diff) == 3)
            {
                angle = 90.0f;
                if (diff < 0)
                {
                    angle *= -1.0f;
                }
            }
            else {
                angle = 180.0f;
            }

            transform.Rotate(new Vector3(0.0f, angle, 0.0f));
            direction = d;
        }
    }

    // 目的地へ移動する
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, step * Time.deltaTime);
    }
}
