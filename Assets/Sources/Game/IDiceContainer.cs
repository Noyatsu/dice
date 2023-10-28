namespace SSTraveler.Game
{
    public interface IDiceContainer
    {
        public void Init();
        public DiceController GetInstance();
        public void ReturnInstance(DiceController instance);
    }
}