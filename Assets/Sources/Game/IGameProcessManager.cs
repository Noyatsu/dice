using SSTraveler.Utility.ReactiveProperty;

namespace SSTraveler.Game
{
    public interface IGameProcessManager
    {
        public ReactiveProperty<int> Level { get; }
        public ReactiveProperty<int> Score { get; }
        public ReactiveProperty<int> Stage { get; }

        public void AddScore(int score);
        public void ResetScore();
    }
}