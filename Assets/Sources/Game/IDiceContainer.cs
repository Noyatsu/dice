using System.Collections.Generic;

namespace SSTraveler.Game
{
    public interface IDiceContainer
    {
        public IEnumerable<DiceController> DicePool { get; }
        public DiceController this[int i] { get; }
        public int ActiveDiceCount { get; }
        public void Init();
        public DiceController GetNewInstance();
        public DiceController GetInstanceAt(int diceId);
        public void ReturnInstance(DiceController instance);
    }
}