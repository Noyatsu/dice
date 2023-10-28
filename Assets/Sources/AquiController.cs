using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SSTraveler.Game
{
    public class AquiController : MonoBehaviour
    {

        private Animator _anim;
        private GameObject _board;
        private MainGameController _script;

        [FormerlySerializedAs("x")] public int X = 0; //< キャラクターのX座標
        [FormerlySerializedAs("y")] public float Y = 1.1f; //キャラクターのY座標
        [FormerlySerializedAs("z")] public int Z = 0; //< キャラクターのZ座標

        private float _step = 5f; //!< 移動速度
        private int _direction = 2; //!< キャラクターの向き
        private Vector3 _target; //!< 入力受付時、移動後の位置を算出して保存
        private Vector3 _prevPos; //!< 何らかの理由で移動できなかった場合、元の位置に戻すため移動前の位置を保存

        private void Start()
        {
            _board = GameObject.Find("Board");
            _script = _board.GetComponent<MainGameController>();
            _target = transform.position;
            _anim = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKey("up") || Input.GetKey("down") || Input.GetKey("right") || Input.GetKey("left"))
            {
                _anim.SetBool("isWalking", true);
            }
            else
            {
                _anim.SetBool("isWalking", false);
            }

            if (_target.x == transform.position.x && _target.z == transform.position.z)
            {
                _script.IsRotateCharactor = false;
            }

            Move();
        }

        private void LateUpdate()
        {
            if (_script.Dices.Count != 0)
            {
                if (_script.Board[X, Z] != -1)
                {
                    //現在位置の下にダイスがあるとき
                    Y = _script.Dices[_script.Board[X, Z]].transform.position.y + 0.5f;
                }
                else if (_script.IsRotateDice)
                {
                    Y = 1.0f;
                }
                else
                {
                    //ダイスがないとき
                    Y = 0.0f;
                }
            }
            else
            {
                Y = 0.0f;
            }

            _target = new Vector3(-4.5f + X * 1.0f, Y, -4.5f + Z * 1.0f);
        }

        public void DeathMotion()
        {
            //anim.SetTrigger("Dead");
            _anim.Play("Dead");
        }

        // 入力に応じて移動後の位置を算出
        public void SetTargetPosition(int d)
        {

            _script.IsRotateCharactor = true;
            _prevPos = _target;

            //右
            if (d == 2 && X < (_script.BoardSize - 1))
            {
                X++;
                RotateCharactor(2);
            }

            //左
            if (d == 0 && X > 0)
            {
                X--;
                RotateCharactor(0);
            }

            //上
            if (d == 1 && Z < (_script.BoardSize - 1))
            {
                Z++;
                RotateCharactor(1);
            }

            //下
            if (d == 3 && Z > 0)
            {
                Z--;
                RotateCharactor(3);
            }

            //キャラクターのy座標の設定
            if (_script.Dices.Count != 0)
            {
                if (_script.Board[X, Z] != -1)
                {
                    //現在位置の下にダイスがあるとき
                    Y = _script.Dices[_script.Board[X, Z]].transform.position.y + 0.5f;
                }
                else if (_script.IsRotateDice)
                {
                    Y = 1.0f;
                }
                else
                {
                    //ダイスがないとき
                    Y = 0.0f;
                }
            }
            else
            {
                Y = 0.0f;
            }


            // ジャンプアニメ
            if (Mathf.Abs(_prevPos.y - Y) > 0.3f)
            {
                _anim.Play("Jump");
            }

            _target = new Vector3(-4.5f + X * 1.0f, Y, -4.5f + Z * 1.0f);

            // Puzzle用
            if (_script.GameType == 3)
            {
                GameObject.Find("PuzzleGameController").GetComponent<PuzzleGameController>().DecrementRemainTurnNum();
            }
        }

        public void SetTarget(int x, float y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            _target = new Vector3(-4.5f + X * 1.0f, Y, -4.5f + Z * 1.0f);
            return;
        }

        public void SetTargetDiscrete(int x, float y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            transform.position = new Vector3(-4.5f + X * 1.0f, Y, -4.5f + Z * 1.0f);
            _target = new Vector3(-4.5f + X * 1.0f, Y, -4.5f + Z * 1.0f);
            return;
        }

        private void RotateCharactor(int d)
        {
            //入力方向と既存の向きが違う場合
            if (d != _direction)
            {
                int diff = _direction - d;
                float angle;
                if (Mathf.Abs(diff) == 1)
                {
                    angle = 90.0f;
                    if (diff > 0)
                    {
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
                else
                {
                    angle = 180.0f;
                }

                transform.Rotate(new Vector3(0.0f, angle, 0.0f));
                _direction = d;
            }
        }

        // 目的地へ移動する
        private void Move()
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _step * Time.deltaTime);
        }


    }
}