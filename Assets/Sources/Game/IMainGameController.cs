using SSTraveler.Utility.ReactiveProperty;

namespace SSTraveler.Game
{
    public interface IMainGameController
    {
        public int GameType { get; }
        public bool IsRotateDice { get; set; }
        public bool IsRotateCharacter { get; set; }
        public bool IsStarting { get; set; }
        
        public ReactiveProperty<string> StatusText { get; }
        public ReactiveProperty<string> ScreenText { get; }
    }
}