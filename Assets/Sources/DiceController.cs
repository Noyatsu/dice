using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{

    GameObject Board, gobjOGController;
    MainGameController script;

    Vector3 rotatePoint = Vector3.zero;
    Vector3 rotateAxis = Vector3.zero;
    float diceAngle = 0f;

    float diceSizeHalf;
    public bool isSelected = true; //!< 上にキャラクターが乗っているかどうか

    public bool isGenerate = true; // サイコロが出現中かどうか

    public bool isVanishing = false; // サイコロが消滅中かどうか
    public bool isRotating = false; // さいころが回転中かどうか

    public int X = 0, Z = 0;
    public int diceId = 0; //!サイコロのID
    public int surfaceA = 1;
    public int surfaceB = 2;
    float step = 2f;

    //音
    private AudioSource sound_roll;


    // Use this for initialization
    void Start()
    {
        Board = GameObject.Find("Board");
        script = Board.GetComponent<MainGameController>();
        gobjOGController = GameObject.Find("OnlineGameController");
        diceSizeHalf = transform.localScale.x / 2f;

        sound_roll = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        risingDice();
        sinkingDice();
    }

    public bool SetTargetPosition(int d)
    {
        //もし上にキャラクターが乗っていたら
        if (isSelected)
        {
            if (isVanishing == true)
            {
                return false;
            }
            if (isGenerate == true)
            {
                return false;
            }

            if (d == 2)
            {
                if (X + 1 < script.board.GetLength(0) && (script.board[X + 1, Z] == -1
                    || (script.dices[script.board[X + 1, Z]].transform.position.y < 0f && script.dices[script.board[X + 1, Z]].GetComponent<DiceController>().isVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (script.board[X + 1, Z] != -1)
                    {
                        if (script.dices[script.board[X + 1, Z]].GetComponent<DiceController>().isVanishing == true)
                        {
                            Debug.Log(script.dices[script.board[X + 1, Z]]);
                            //そのさいころを削除
                            script.dices[script.board[X + 1, Z]].GetComponent<DiceController>().destroyDice();
                        }
                    }
                    if (script.gameType == 1)
                    {
                        gobjOGController.GetComponent<OnlineGameController>().sendRoll(X, Z, d);
                    }
                    X += 1; //インクリメント

                    //さいころの面を計算
                    int result = ComputeNextDice(surfaceA, surfaceB, "right");
                    surfaceA = result / 10;
                    surfaceB = result - surfaceA * 10;

                    //新たなる位置に代入
                    script.board[X, Z] = diceId;
                    script.board_num[X, Z] = surfaceA;

                    rotatePoint = transform.position + new Vector3(diceSizeHalf, -diceSizeHalf, 0f);
                    rotateAxis = new Vector3(0, 0, -1);
                    StartCoroutine(MoveDice());


                    //過去の位置に-1を代入
                    script.board[X - 1, Z] = -1;
                    script.board_num[X - 1, Z] = -1;
                    sound_roll.PlayOneShot(sound_roll.clip);

                    return true;
                }
            }
            if (d == 0)
            {
                if (0 <= X - 1 && (script.board[X - 1, Z] == -1
                || (script.dices[script.board[X - 1, Z]].transform.position.y < 0f && script.dices[script.board[X - 1, Z]].GetComponent<DiceController>().isVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (script.board[X - 1, Z] != -1)
                    {

                        if (script.dices[script.board[X - 1, Z]].GetComponent<DiceController>().isVanishing == true)
                        {
                            //そのさいころを削除
                            script.dices[script.board[X - 1, Z]].GetComponent<DiceController>().destroyDice();
                        }
                    }
                    if (script.gameType == 1)
                    {
                        gobjOGController.GetComponent<OnlineGameController>().sendRoll(X, Z, d);
                    }
                    X -= 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(surfaceA, surfaceB, "left");
                    surfaceA = result / 10;
                    surfaceB = result - surfaceA * 10;

                    script.board[X, Z] = diceId;
                    script.board_num[X, Z] = surfaceA;
                    rotatePoint = transform.position + new Vector3(-diceSizeHalf, -diceSizeHalf, 0f);
                    rotateAxis = new Vector3(0, 0, 1);
                    StartCoroutine(MoveDice());

                    script.board[X + 1, Z] = -1;
                    script.board_num[X + 1, Z] = -1;
                    sound_roll.PlayOneShot(sound_roll.clip);

                    return true;
                }

            }
            if (d == 1)
            {
                if (Z + 1 < script.board.GetLength(1) && (script.board[X, Z + 1] == -1
                || (script.dices[script.board[X, Z + 1]].transform.position.y < 0f && script.dices[script.board[X, Z + 1]].GetComponent<DiceController>().isVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (script.board[X, Z + 1] != -1)
                    {

                        if (script.dices[script.board[X, Z + 1]].GetComponent<DiceController>().isVanishing == true)
                        {
                            //そのさいころを削除
                            script.dices[script.board[X, Z + 1]].GetComponent<DiceController>().destroyDice();
                        }
                    }
                    if (script.gameType == 1)
                    {
                        gobjOGController.GetComponent<OnlineGameController>().sendRoll(X, Z, d);
                    }
                    Z += 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(surfaceA, surfaceB, "up");
                    surfaceA = result / 10;
                    surfaceB = result - surfaceA * 10;

                    script.board[X, Z] = diceId;
                    script.board_num[X, Z] = surfaceA;
                    rotatePoint = transform.position + new Vector3(0f, -diceSizeHalf, diceSizeHalf);
                    rotateAxis = new Vector3(1, 0, 0);
                    StartCoroutine(MoveDice());

                    script.board[X, Z - 1] = -1;
                    script.board_num[X, Z - 1] = -1;
                    sound_roll.PlayOneShot(sound_roll.clip);

                    return true;
                }

            }
            if (d == 3)
            {
                if (0 <= Z - 1 && (script.board[X, Z - 1] == -1
                || (script.dices[script.board[X, Z - 1]].transform.position.y < 0f && script.dices[script.board[X, Z - 1]].GetComponent<DiceController>().isVanishing == true)))
                {
                    // 隣のさいころが沈み途中の時
                    if (script.board[X, Z - 1] != -1)
                    {

                        if (script.dices[script.board[X, Z - 1]].GetComponent<DiceController>().isVanishing == true)
                        {
                            //そのさいころを削除
                            script.dices[script.board[X, Z - 1]].GetComponent<DiceController>().destroyDice();
                        }
                    }
                    if (script.gameType == 1)
                    {
                        gobjOGController.GetComponent<OnlineGameController>().sendRoll(X, Z, d);
                    }
                    Z -= 1;

                    //さいころの面を計算
                    int result = ComputeNextDice(surfaceA, surfaceB, "down");
                    surfaceA = result / 10;
                    surfaceB = result - surfaceA * 10;

                    script.board[X, Z] = diceId;
                    script.board_num[X, Z] = surfaceA;
                    rotatePoint = transform.position + new Vector3(0f, -diceSizeHalf, -diceSizeHalf);
                    rotateAxis = new Vector3(-1, 0, 0);
                    StartCoroutine(MoveDice());

                    script.board[X, Z + 1] = -1;
                    script.board_num[X, Z + 1] = -1;
                    sound_roll.PlayOneShot(sound_roll.clip);

                    return true;
                }

            }
        }
        return false;
    }



    IEnumerator MoveDice()
    {
        script.isRotate_dice = true;

        float sumAngle = 0f;
        while (sumAngle < 90f)
        {
            diceAngle = 6f * Time.deltaTime * 50f;
            sumAngle += diceAngle;

            if (sumAngle > 90f)
            {
                diceAngle -= sumAngle - 90f;
            }
            transform.RotateAround(rotatePoint, rotateAxis, diceAngle);

            yield return null;
        }

        script.isRotate_dice = false;

        yield break;
    }

    /**
     * さいころの各状態と入力からさいころの次の状態を求める
     * @param int a 面Aの数字
     * @param int b 面Bの数字
     * @param string direction 入力方向(up/down/left/right)
     * @return int 2桁の数字(56なら面Aが5, 面Bが6)
     */
    int ComputeNextDice(int a, int b, string direction)
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

    public void risingDice()
    {
        if (isGenerate)
        {
            Vector3 position = transform.position;
            position.y += Time.deltaTime * 2.0f;
            ChangeColorOfGameObject(this.gameObject, new Color(1.0f, 1.0f, 1.0f, 0.5f + position.y));
            if (position.y >= 0.5f)
            {
                position.y = 0.5f;
                isGenerate = false;
            }
            transform.position = position;
        }
    }
    public void sinkingDice()
    {
        if (isVanishing)
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
                destroyDice();
            }
        }
    }

    public void destroyDice()
    {
        isVanishing = false;
        script.board[X, Z] = -1;
        script.board_num[X, Z] = -1;
        Destroy(this.gameObject);
        if (script.gameType == 1)
        {
            gobjOGController.GetComponent<OnlineGameController>().sendVanish(X, Z);
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

