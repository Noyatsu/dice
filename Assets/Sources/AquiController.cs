using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquiController : MonoBehaviour {

    private Animator anim;
    GameObject Board;
    MainGameController script;

    public int x = 0; //< キャラクターのX座標
    public float y = 1.1f; //キャラクターのY座標
    public int z = 0; //< キャラクターのZ座標

    float step = 5f;     //!< 移動速度
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
        if (Input.GetKey("up") || Input.GetKey("down") || Input.GetKey("right") || Input.GetKey("left"))
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
        if (target.x == transform.position.x && target.z == transform.position.z)
        {
            script.isRotate_charactor = false;
        }

        Move();
    }

    // 入力に応じて移動後の位置を算出
    public void SetTargetPosition(int d)
    {

        script.isRotate_charactor = true;
        prevPos = target;

        //右
        if (d == 2 && x < (script.boardSize - 1)) {
            x++;
            RotateCharactor(2);
        }
        //左
        if (d == 0 && x > 0) {
            x--;
            RotateCharactor(0);
        }
        //上
        if (d == 1 && z < (script.boardSize - 1)) {
            z++;
            RotateCharactor(1);
        }
        //下
        if (d == 3 && z > 0) {
            z--;
            RotateCharactor(3);
        }

        //キャラクターのy座標の設定
        if (script.board[x, z] != -1)
        {
            //現在位置の下にダイスがあるとき
            y = script.dices[script.board[x, z]].transform.position.y + 0.5f;
        }
        else if (script.isRotate_dice)
        {
            y = 1.0f;
        }
        else
        {
            //ダイスがないとき
            y = 0.0f;
        }

        // ジャンプアニメ
        if (Mathf.Abs(prevPos.y - y) > 0.3f)
        {
            anim.Play("Jump");
        }
        target = new Vector3(-4.5f + x * 1.0f, y, -4.5f + z * 1.0f);
        return;
    }

    void RotateCharactor(int d)
    {
        //入力方向と既存の向きが違う場合
        if (d != direction) {
            int diff = direction - d;
            float angle;
            if (Mathf.Abs(diff) == 1) {
                angle = 90.0f;
                if (diff > 0) {
                    angle *= -1.0f;
                }
            }
            else if (Mathf.Abs(diff) == 3) {
                angle = 90.0f;
                if (diff < 0) {
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
