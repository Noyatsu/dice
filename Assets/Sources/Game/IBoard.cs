using System.Collections.Generic;

namespace SSTraveler.Game
{
    public interface IBoard
    {
        public int Size { get; }
        public int MaxCellNum { get; }
        public Cell this[int i, int j] { get; }
        public IEnumerable<Cell> Cells { get; }

        public void Reset();

        /// <summary>
        /// セルを取得する
        /// </summary>
        public Cell GetCell(int x, int z);

        /// <summary>
        /// さいころをセルにセットする
        /// 殻にしたい場合はdiceにnullを入れる
        /// </summary>
        public void SetDice(int x, int z, DiceController dice);

    }
}