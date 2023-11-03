using SSTraveler.Game;
using SSTraveler.Utility.ReactiveProperty;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace SSTraveler.Ui
{
    public class LevelText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;
        
        private IGameProcessManager _gameProcessManager;
        
        [Inject]
        public void Construct(IGameProcessManager gameProcessManager)
        {
            _gameProcessManager = gameProcessManager;
        }

        private void Start()
        {
            _gameProcessManager.Level.Subscribe(lv => _levelText.text = $"<size=50>Level </size>{lv}").AddTo(this);
        }
    }
}