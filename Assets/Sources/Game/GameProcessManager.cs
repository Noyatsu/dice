using SSTraveler.Utility.ReactiveProperty;

namespace SSTraveler.Game
{
    /// <summary>
    /// レベルとスコアを管理する
    /// </summary>
    public class GameProcessManager : IGameProcessManager
    {
        public ReactiveProperty<int> Level { get; } = new();
        public ReactiveProperty<int> Score { get; } = new();
        public ReactiveProperty<int> Stage { get; } = new();
        
        public void AddScore(int score)
        {
            Score.Value += score;
            ComputeLevel();
        }
        
        public void ResetScore()
        {
            Score.Value = 0;
        }
        
        private void ComputeLevel()
        {
            int oneLevelScore = 150; //おおよそ1レベルの上昇に必要なスコア
            int beforeLevelScore = oneLevelScore; //前の必要経験値を記録する
            int lv = 1;

            //レベルの変化
            while (true)
            {
                beforeLevelScore = (int)((oneLevelScore * lv + beforeLevelScore * 1.08) / 2);
                if (Score.Value > beforeLevelScore)
                {
                    lv++;
                }
                else
                {
                    break;
                }
            }

            // ステージの計算
            Level.Value = lv;
            Stage.Value = (Level % 21 / 3 + 1) % 7;
        }
    }
}