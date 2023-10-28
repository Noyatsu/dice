using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace SSTraveler.Game
{
    public class DiceController : MonoBehaviour
    {
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

        //音
        private AudioSource _soundRoll;
        
        private IDiceContainer _diceContainer;
        private IBoard _board;
        private IMainGameController _mainGameController;
        
        [Inject]
        public void Construct(IDiceContainer diceContainer, IBoard board, IMainGameController mainGameController)
        {
            _diceContainer = diceContainer;
            _board = board;
            _mainGameController = mainGameController;
        }

        public void ResetDice()
        {
            _rotatePoint = Vector3.zero;
            _rotateAxis = Vector3.zero;
            _diceAngle = 0f;
            IsSelected = true;
            IsGenerate = true;
            IsVanishing = false;
            IsRotating = false;
            X = 0;
            Z = 0;
            SurfaceA = 1;
            SurfaceB = 2;
            ChangeColorOfGameObject(gameObject, new Color(0.7f, 0.7f, 0.7f, 1f));
        }

        private void Start()
        {
            _diceSizeHalf = transform.localScale.x / 2f;

            _soundRoll = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        private void Update()
        {
            RisingDice();
            SinkingDice();
        }

        public void SetInitTransform(Vector3 worldPos, int a, int b)
        {
             // 側面の決定用乱数
            int i = Random.Range(1, 4);

            // 面によって回転角度を決定
            int xi = 0, yi = 0, zi = 0, ra = 90;
            switch (a)
            {
                case 1:
                    if (b == 0)
                    {
                        int[] num1 = { 2, 3, 4, 5 };
                        b = num1[i];
                    }

                    switch (b)
                    {
                        case 2:
                            break;
                        case 3:
                            yi = ra;
                            break;
                        case 4:
                            yi = ra * 3;
                            break;
                        case 5:
                            yi = ra * 2;
                            break;
                    }

                    break;

                case 2:
                    if (b == 0)
                    {
                        int[] num2 = { 1, 3, 4, 6 };
                        b = num2[i];
                    }

                    switch (b)
                    {
                        case 1:
                            xi = ra;
                            yi = ra * 2;
                            break;
                        case 3:
                            xi = ra;
                            yi = ra;
                            break;
                        case 4:
                            xi = ra;
                            yi = ra * 3;
                            break;
                        case 6:
                            xi = ra;
                            break;
                    }

                    break;

                case 3:
                    if (b == 0)
                    {
                        int[] num3 = { 1, 2, 5, 6 };
                        b = num3[i];
                    }

                    switch (b)
                    {
                        case 1:
                            zi = ra;
                            yi = ra * 3;
                            break;
                        case 2:
                            zi = ra;
                            break;
                        case 5:
                            zi = ra;
                            yi = ra * 2;
                            break;
                        case 6:
                            zi = ra;
                            yi = ra;
                            break;
                    }

                    break;
                case 4:
                    if (b == 0)
                    {
                        int[] num4 = { 1, 2, 5, 6 };
                        b = num4[i];
                    }

                    switch (b)
                    {
                        case 1:
                            zi = ra * 3;
                            yi = ra * 1;
                            break;
                        case 2:
                            zi = ra * 3;
                            break;
                        case 5:
                            zi = ra * 3;
                            yi = ra * 2;
                            break;
                        case 6:
                            zi = ra * 3;
                            yi = ra * 3;
                            break;
                    }

                    break;

                case 5:
                    if (b == 0)
                    {
                        int[] num5 = { 1, 3, 4, 6 };
                        b = num5[i];
                    }

                    switch (b)
                    {
                        case 1:
                            xi = ra * 3;
                            break;
                        case 3:
                            xi = ra * 3;
                            yi = ra;
                            break;
                        case 4:
                            xi = ra * 3;
                            yi = ra * 3;
                            break;
                        case 6:
                            xi = ra * 3;
                            yi = ra * 2;
                            break;
                    }

                    break;
                case 6:
                    if (b == 0)
                    {
                        int[] num6 = { 2, 3, 4, 5 };
                        b = num6[i];
                    }

                    switch (b)
                    {
                        case 2:
                            xi = ra * 2;
                            yi = ra * 2;
                            break;
                        case 3:
                            xi = ra * 2;
                            yi = ra;
                            break;
                        case 4:
                            xi = ra * 2;
                            yi = ra * 3;
                            break;
                        case 5:
                            xi = ra * 2;
                            break;
                    }

                    break;
            }

            SurfaceA = a;
            SurfaceB = b;
            transform.position = worldPos;
            transform.rotation = Quaternion.Euler(xi, yi, zi);
        }
        
        public bool SetTargetPosition(int d)
        {
            // もし上にキャラクターが乗っていたら
            if (IsSelected)
            {
                if (IsVanishing || IsGenerate || !gameObject.activeSelf)
                {
                    return false;
                }
                
                if (d == 2)
                {
                    if (X + 1 < _board.Size && (_board[X + 1, Z].IsEmpty || (_board[X + 1, Z].Dice.transform.position.y < 0f && _board[X + 1, Z].Dice.IsVanishing)))
                    {
                        // 隣のさいころが沈み途中で踏みつぶせるとき
                        if (_board[X + 1, Z].IsExist && _board[X + 1, Z].Dice.IsVanishing)
                        {
                            _board[X + 1, Z].Dice.DestroyDice();
                        }

                        X += 1; //インクリメント

                        // さいころの面を計算
                        int result = ComputeNextDice(SurfaceA, SurfaceB, "right");
                        SurfaceA = result / 10;
                        SurfaceB = result - SurfaceA * 10;

                        // 新たなる位置に代入
                        _board.SetDice(X, Z, this);

                        _rotatePoint = transform.position + new Vector3(_diceSizeHalf, -_diceSizeHalf, 0f);
                        _rotateAxis = new Vector3(0, 0, -1);
                        StartCoroutine(MoveDice());
                        
                        //過去の位置を空に
                        _board.SetDice(X - 1, Z, null);
                        _soundRoll.PlayOneShot(_soundRoll.clip);
                        
                        return true;
                    }
                }

                if (d == 0)
                {
                    if (0 <= X - 1 && (_board[X - 1, Z].IsEmpty || (_board[X - 1, Z].Dice.transform.position.y < 0f && _board[X - 1, Z].Dice.IsVanishing)))
                    {
                        // 隣のさいころが沈み途中の時
                        if (_board[X - 1, Z].IsExist && _board[X - 1, Z].Dice.IsVanishing)
                        {
                            // そのさいころを削除
                            _board[X - 1, Z].Dice.DestroyDice();
                        }

                        X -= 1;

                        //さいころの面を計算
                        int result = ComputeNextDice(SurfaceA, SurfaceB, "left");
                        SurfaceA = result / 10;
                        SurfaceB = result - SurfaceA * 10;

                        _board.SetDice(X, Z, this);
                        _rotatePoint = transform.position + new Vector3(-_diceSizeHalf, -_diceSizeHalf, 0f);
                        _rotateAxis = new Vector3(0, 0, 1);
                        StartCoroutine(MoveDice());

                        _board.SetDice(X + 1, Z, null);
                        _soundRoll.PlayOneShot(_soundRoll.clip);

                        return true;
                    }

                }

                if (d == 1)
                {
                    if (Z + 1 < _board.Size && (_board[X, Z + 1].IsEmpty || (_board[X, Z + 1].Dice.transform.position.y < 0f && _board[X, Z + 1].Dice.IsVanishing)))
                    {
                        // 隣のさいころが沈み途中の時
                        if (_board[X, Z + 1].IsExist && _board[X, Z + 1].Dice.IsVanishing)
                        {
                            _board[X, Z + 1].Dice.DestroyDice();
                        }
                        
                        Z += 1;

                        // さいころの面を計算
                        int result = ComputeNextDice(SurfaceA, SurfaceB, "up");
                        SurfaceA = result / 10;
                        SurfaceB = result - SurfaceA * 10;

                        _board.SetDice(X, Z, this);
                        _rotatePoint = transform.position + new Vector3(0f, -_diceSizeHalf, _diceSizeHalf);
                        _rotateAxis = new Vector3(1, 0, 0);
                        StartCoroutine(MoveDice());

                        _board.SetDice(X, Z - 1, null);
                        _soundRoll.PlayOneShot(_soundRoll.clip);

                        return true;
                    }

                }

                if (d == 3)
                {
                    if (0 <= Z - 1 && (_board[X, Z - 1].IsEmpty || (_board[X, Z - 1].Dice.transform.position.y < 0f && _board[X, Z - 1].Dice.IsVanishing)))
                    {
                        // 隣のさいころが沈み途中の時
                        if (_board[X, Z - 1].IsExist && _board[X, Z - 1].Dice.IsVanishing)
                        {
                            _board[X, Z - 1].Dice.DestroyDice();
                        }

                        Z -= 1;

                        //さいころの面を計算
                        int result = ComputeNextDice(SurfaceA, SurfaceB, "down");
                        SurfaceA = result / 10;
                        SurfaceB = result - SurfaceA * 10;

                        _board.SetDice(X, Z, this);
                        _rotatePoint = transform.position + new Vector3(0f, -_diceSizeHalf, -_diceSizeHalf);
                        _rotateAxis = new Vector3(-1, 0, 0);
                        StartCoroutine(MoveDice());

                        _board.SetDice(X, Z + 1, null);
                        _soundRoll.PlayOneShot(_soundRoll.clip);

                        return true;
                    }

                }
            }

            return false;
        }


        private IEnumerator MoveDice()
        {
            _mainGameController.IsRotateDice = true;

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

            _mainGameController.IsRotateDice = false;

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
            _board.SetDice(X, Z, null);
            _diceContainer.ReturnInstance(this);
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

}