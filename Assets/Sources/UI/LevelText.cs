using SSTraveler.Game;
using SSTraveler.Utility.ReactiveProperty;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SSTraveler.Ui
{
    public class LevelText : MonoBehaviour
    {
        private IGameProcessManager _gameProcessManager;

        [FormerlySerializedAs("text")] public Text Text;

        [Inject]
        public void Construct(IGameProcessManager gameProcessManager)
        {
            _gameProcessManager = gameProcessManager;
        }

        private void Start()
        {
            _gameProcessManager.Level.Subscribe(lv => Text.text = "Level " + lv).AddTo(this);
        }
    }
}