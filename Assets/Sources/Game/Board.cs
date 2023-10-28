// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace SSTraveler.Game
{
    public class Board : IBoard
    {
        public int Size => _boardSize;
        public int MaxCellNum => _boardSize * _boardSize;
        public Cell this[int i, int j] => GetCell(i, j);
        public IEnumerable<Cell> Cells => _cells;

        private readonly List<Cell> _cells = new(_boardSize * _boardSize);
        private readonly Cell[,] _board = new Cell[_boardSize, _boardSize]; //!< さいころのIDを格納
        private const int _boardSize = 7;

        public Board()
        {
            Reset();
        }

        public void Reset()
        {
            for (int i = 0; i < _boardSize; i++)
            {
                for (int j = 0; j < _boardSize; j++)
                {
                    _board[i, j] = new Cell();
                    _cells.Add(_board[i, j]);
                }
            }
        }

        public void SetDice(int x, int z, DiceController dice)
        {
            var cell = _board[x, z];
            if (dice is null)
            {
                cell.Dice = null;
                return;
            }

            cell.Dice = dice;
            dice.X = x;
            dice.Z = z;
        }
        
        public Cell GetCell(int x, int z)
        {
            return _board[x, z];
        }
    }

    public class Cell
    {
        public int DiceId => Dice ? Dice.DiceId : -1;
        public int DiceNum => Dice ? Dice.SurfaceA : -1;
        public DiceController Dice { get; set; }

        public bool IsEmpty => Dice is null;
        public bool IsExist => Dice is not null;
    }
}