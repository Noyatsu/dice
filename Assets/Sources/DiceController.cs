using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DiceController : MonoBehaviour
{
    private GameObject _board, _gobjOgController;
    private MainGameController _script;

    private Vector3 _rotatePoint = Vector3.zero;
    private Vector3 _rotateAxis = Vector3.zero;
    private float _diceAngle = 0f;

    private float _diceSizeHalf;
    [FormerlySerializedAs("isSelected")] public bool IsSelected = true; //!< 上にキャラクターが乗っているかどうか

    [FormerlySerializedAs("isGenerate")] public bool IsGenerate = true; // サイコロが出現中かどうか

    [FormerlySerializedAs("isVanishing")] public bool IsVanishing = false; // サイコロが消滅中かどうか
    [FormerlySerializedAs("isRotating")] public bool IsRotating = false; // さいころが回転中かどうか

    public int X = 0, Z = 0;
    [FormerlySerializedAs("diceId")] public int DiceId = 0; //!サイコロのID
    [FormerlySerializedAs("surfaceA")] public int SurfaceA = 1;
    [FormerlySerializedAs("surfaceB")] public int SurfaceB = 2;
    private float _step = 2f;

    //音
    private AudioSource _soundRoll;


    // Use this for initialization
    private void Start()
    {
        _board = GameObject.Find("Board");
        _script = _board.GetComponent<MainGameController>();
        _gobjOgController = GameObject.Find("OnlineGameController");
        _diceSizeHalf = transform.localScale.x / 2f;

        _soundRoll = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        RisingDice();
        SinkingDice();
    }

    public bool SetTargetPosition(int d)
    {
        //もし上にキャラクターが乗っていたら
        if (IsSelected)
        {
            if (IsVanishing == true)
            {
                return false;
            }
            if (IsGenerate == true)
            {
                return false;
            }

            if (d == 2)
            {
                if (X + 1 < _script.Board.GetLength(0) && (_script.Board[X + 1, Z] == -1
                    || (_script.Dices[_script.Board[X + 1, Z]].transform.position.y < 0f && _script.Dices[_script.Board[X + 1, Z]].GetComponent<DiceController>().IsVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (_script.Board[X + 1, Z] != -1)
                    {
                        if (_script.Dices[_script.Board[X + 1, Z]].GetComponent<DiceController>().IsVanishing == true)
                        {
                            Debug.Log(_script.Dices[_script.Board[X + 1, Z]]);
                            //そのさいころを削除
                            _script.Dices[_script.Board[X + 1, Z]].GetComponent<DiceController>().DestroyDice();
                        }
                    }
                    if (_script.GameType == 1)
                    {
                        _gobjOgController.GetComponent<OnlineGameController>().SendRoll(X, Z, d);
                    }
                    X += 1; //インクリメント

                    //さいころの面を計算
                    int result = ComputeNextDice(SurfaceA, SurfaceB, "right");
                    SurfaceA = result / 10;
                    SurfaceB = result - SurfaceA * 10;

                    //新たなる位置に代入
                    _script.Board[X, Z] = DiceId;
                    _script.BoardNum[X, Z] = SurfaceA;

                    _rotatePoint = transform.position + new Vector3(_diceSizeHalf, -_diceSizeHalf, 0f);
                    _rotateAxis = new Vector3(0, 0, -1);
                    StartCoroutine(MoveDice());


                    //過去の位置に-1を代入
                    _script.Board[X - 1, Z] = -1;
                    _script.BoardNum[X - 1, Z] = -1;
                    _soundRoll.PlayOneShot(_soundRoll.clip);

                    return true;
                }
            }
            if (d == 0)
            {
                if (0 <= X - 1 && (_script.Board[X - 1, Z] == -1
                || (_script.Dices[_script.Board[X - 1, Z]].transform.position.y < 0f && _script.Dices[_script.Board[X - 1, Z]].GetComponent<DiceController>().IsVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (_script.Board[X - 1, Z] != -1)
                    {

                        if (_script.Dices[_script.Board[X - 1, Z]].GetComponent<DiceController>().IsVanishing == true)
                        {
                            //そのさいころを削除
                            _script.Dices[_script.Board[X - 1, Z]].GetComponent<DiceController>().DestroyDice();
                        }
                    }
                    if (_script.GameType == 1)
                    {
                        _gobjOgController.GetComponent<OnlineGameController>().SendRoll(X, Z, d);
                    }
                    X -= 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(SurfaceA, SurfaceB, "left");
                    SurfaceA = result / 10;
                    SurfaceB = result - SurfaceA * 10;

                    _script.Board[X, Z] = DiceId;
                    _script.BoardNum[X, Z] = SurfaceA;
                    _rotatePoint = transform.position + new Vector3(-_diceSizeHalf, -_diceSizeHalf, 0f);
                    _rotateAxis = new Vector3(0, 0, 1);
                    StartCoroutine(MoveDice());

                    _script.Board[X + 1, Z] = -1;
                    _script.BoardNum[X + 1, Z] = -1;
                    _soundRoll.PlayOneShot(_soundRoll.clip);

                    return true;
                }

            }
            if (d == 1)
            {
                if (Z + 1 < _script.Board.GetLength(1) && (_script.Board[X, Z + 1] == -1
                || (_script.Dices[_script.Board[X, Z + 1]].transform.position.y < 0f && _script.Dices[_script.Board[X, Z + 1]].GetComponent<DiceController>().IsVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (_script.Board[X, Z + 1] != -1)
                    {

                        if (_script.Dices[_script.Board[X, Z + 1]].GetComponent<DiceController>().IsVanishing == true)
                        {
                            //そのさいころを削除
                            _script.Dices[_script.Board[X, Z + 1]].GetComponent<DiceController>().DestroyDice();
                        }
                    }
                    if (_script.GameType == 1)
                    {
                        _gobjOgController.GetComponent<OnlineGameController>().SendRoll(X, Z, d);
                    }
                    Z += 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(SurfaceA, SurfaceB, "up");
                    SurfaceA = result / 10;
                    SurfaceB = result - SurfaceA * 10;

                    _script.Board[X, Z] = DiceId;
                    _script.BoardNum[X, Z] = SurfaceA;
                    _rotatePoint = transform.position + new Vector3(0f, -_diceSizeHalf, _diceSizeHalf);
                    _rotateAxis = new Vector3(1, 0, 0);
                    StartCoroutine(MoveDice());

                    _script.Board[X, Z - 1] = -1;
                    _script.BoardNum[X, Z - 1] = -1;
                    _soundRoll.PlayOneShot(_soundRoll.clip);

                    return true;
                }

            }
            if (d == 3)
            {
                if (0 <= Z - 1 && (_script.Board[X, Z - 1] == -1
                || (_script.Dices[_script.Board[X, Z - 1]].transform.position.y < 0f && _script.Dices[_script.Board[X, Z - 1]].GetComponent<DiceController>().IsVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (_script.Board[X, Z - 1] != -1)
                    {

                        if (_script.Dices[_script.Board[X, Z - 1]].GetComponent<DiceController>().IsVanishing == true)
                        {
                            //そのさいころを削除
                            _script.Dices[_script.Board[X, Z - 1]].GetComponent<DiceController>().DestroyDice();
                        }
                    }
                    if (_script.GameType == 1)
                    {
                        _gobjOgController.GetComponent<OnlineGameController>().SendRoll(X, Z, d);
                    }
                    Z -= 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(SurfaceA, SurfaceB, "down");
                    SurfaceA = result / 10;
                    SurfaceB = result - SurfaceA * 10;

                    _script.Board[X, Z] = DiceId;
                    _script.BoardNum[X, Z] = SurfaceA;
                    _rotatePoint = transform.position + new Vector3(0f, -_diceSizeHalf, -_diceSizeHalf);
                    _rotateAxis = new Vector3(-1, 0, 0);
                    StartCoroutine(MoveDice());

                    _script.Board[X, Z + 1] = -1;
                    _script.BoardNum[X, Z + 1] = -1;
                    _soundRoll.PlayOneShot(_soundRoll.clip);

                    return true;
                }

            }
        }
        return false;
    }


    private IEnumerator MoveDice()
    {
        _script.IsRotateDice = true;

        float sumAngle = 0f;
        while (sumAngle < 90f)
        {
            _diceAngle = 6f * Time.deltaTime * 50f;
            sumAngle += _diceAngle;

            if (sumAngle > 90f)
            {
                _diceAngle -= sumAngle - 90f;
            }
            transform.RotateAround(_rotatePoint, _rotateAxis, _diceAngle);

            yield return null;
        }

        _script.IsRotateDice = false;

        yield break;
    }

    /**
     * さいころの各状態と入力からさいころの次の状態を求める
     * @param int a 面Aの数字
     * @param int b 面Bの数字
     * @param string direction 入力方向(up/down/left/right)
     * @return int 2桁の数字(56なら面Aが5, 面Bが6)
     */
    private int ComputeNextDice(int a, int b, string direction)
    {
        int nextA = 0, nextB = 0;
        switch (a)
        {
            case 1:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
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
                        switch (b)
                        {
                            case 2:
                                nextA = 4;
                                break;
                            case 3:
                                nextA = 2;
                                break;
                            case 4:
                                nextA = 5;
                                break;
                            case 5:
                                nextA = 3;
                                break;
                        }
                        nextB = b;
                        break;
                }
                break;
            case 2:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 1:
                                nextA = 4;
                                break;
                            case 3:
                                nextA = 1;
                                break;
                            case 4:
                                nextA = 6;
                                break;
                            case 6:
                                nextA = 3;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
                        switch (b)
                        {
                            case 1:
                                nextA = 3;
                                break;
                            case 3:
                                nextA = 6;
                                break;
                            case 4:
                                nextA = 1;
                                break;
                            case 6:
                                nextA = 4;
                                break;
                        }
                        nextB = b;
                        break;
                }
                break;
            case 3:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 1:
                                nextA = 2;
                                break;
                            case 2:
                                nextA = 6;
                                break;
                            case 5:
                                nextA = 1;
                                break;
                            case 6:
                                nextA = 5;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
                        switch (b)
                        {
                            case 1:
                                nextA = 5;
                                break;
                            case 2:
                                nextA = 1;
                                break;
                            case 5:
                                nextA = 6;
                                break;
                            case 6:
                                nextA = 2;
                                break;
                        }
                        nextB = b;
                        break;
                }
                break;
            case 4:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 1:
                                nextA = 5;
                                break;
                            case 2:
                                nextA = 1;
                                break;
                            case 5:
                                nextA = 6;
                                break;
                            case 6:
                                nextA = 2;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
                        switch (b)
                        {
                            case 1:
                                nextA = 2;
                                break;
                            case 2:
                                nextA = 6;
                                break;
                            case 5:
                                nextA = 1;
                                break;
                            case 6:
                                nextA = 5;
                                break;
                        }
                        nextB = b;
                        break;
                }
                break;
            case 5:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 1:
                                nextA = 3;
                                break;
                            case 3:
                                nextA = 6;
                                break;
                            case 4:
                                nextA = 1;
                                break;
                            case 6:
                                nextA = 4;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
                        switch (b)
                        {
                            case 1:
                                nextA = 4;
                                break;
                            case 3:
                                nextA = 1;
                                break;
                            case 4:
                                nextA = 6;
                                break;
                            case 6:
                                nextA = 3;
                                break;
                        }
                        nextB = b;
                        break;
                }
                break;
            case 6:
                switch (direction)
                {
                    case "up":
                        nextA = b;
                        nextB = 7 - a;
                        break;
                    case "down":
                        nextA = 7 - b;
                        nextB = a;
                        break;
                    case "left":
                        switch (b)
                        {
                            case 2:
                                nextA = 4;
                                break;
                            case 3:
                                nextA = 2;
                                break;
                            case 4:
                                nextA = 5;
                                break;
                            case 5:
                                nextA = 3;
                                break;
                        }
                        nextB = b;
                        break;
                    case "right":
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
                }
                break;

        }
        return nextA * 10 + nextB;
    }

    public void RisingDice()
    {
        if (IsGenerate)
        {
            Vector3 position = transform.position;
            position.y += Time.deltaTime * 2.0f;
            //ChangeColorOfGameObject(this.gameObject, new Color(1.0f, 1.0f, 1.0f, 0.5f + position.y));
            if (position.y >= 0.5f)
            {
                position.y = 0.5f;
                IsGenerate = false;
            }
            transform.position = position;
        }
    }
    public void SinkingDice()
    {
        if (IsVanishing)
        {
            Vector3 position = transform.position;
            position.y -= Time.deltaTime * 0.125f;
            transform.position = position;
            if (position.y > 0f)
            {
                ChangeColorOfGameObject(this.gameObject, new Color(0.75f + position.y / 2f, 0.75f + position.y / 2f, 0.75f + position.y / 2f, 0.75f + position.y / 2f));
            }
            else
            {
                ChangeColorOfGameObject(this.gameObject, new Color(0.5f + position.y, 0.5f + position.y, 1.0f + position.y, 0.5f + position.y));
            }
            if (position.y <= -0.5f)
            {
                DestroyDice();
            }
        }
    }

    public void DestroyDice()
    {
        IsVanishing = false;
        _script.Board[X, Z] = -1;
        _script.BoardNum[X, Z] = -1;
        Debug.Log($"Destroy: {gameObject.name}");
        Destroy(this.gameObject);
        if (_script.GameType == 1)
        {
            _gobjOgController.GetComponent<OnlineGameController>().SendVanish(X, Z);
        }
    }

    private void ChangeColorOfGameObject(GameObject targetObject, Color color)
    {
        if (targetObject != null)
        {
            //入力されたオブジェクトのRendererを全て取得し、さらにそのRendererに設定されている全Materialの色を変える
            foreach (Renderer targetRenderer in targetObject.GetComponents<Renderer>())
            {
                foreach (Material material in targetRenderer.materials)
                {
                    material.color = color;
                }
            }

            //入力されたオブジェクトの子にも同様の処理を行う
            for (int i = 0; i < targetObject.transform.childCount; i++)
            {
                ChangeColorOfGameObject(targetObject.transform.GetChild(i).gameObject, color);
            }
        }

    }
}

