namespace SSTraveler.Game
{
    public interface IMainGameController
    {
        public int GameType { get; }
        public bool IsRotateDice { get; set; }
        public bool IsRotateCharacter { get; set; }
        public bool IsStarting { get; set; }
    }
}