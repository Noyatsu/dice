using System;
using SSTraveler.Game;
using SSTraveler.Utility.ReactiveProperty;
using TMPro;
using UnityEngine;
using Zenject;

namespace SSTraveler.Ui
{
    public class ScoreText : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI _scoreText;

        private int _targetScore = 0;
        private int _currentScore = 0;
        private IGameProcessManager _gameProcessManager;
        
        [Inject]
        public void Construct(IGameProcessManager gameProcessManager)
        {
            _gameProcessManager = gameProcessManager;
        }

        private void Start()
        {
            _gameProcessManager.Score.Subscribe(s => _targetScore = s).AddTo(this);
            _scoreText.text = _currentScore.ToString();
        }

        private void Update()
        {
            if (_currentScore < _targetScore)
            {
                if (_targetScore - _currentScore > 50)
                {
                    _currentScore += 7;
                }
                else
                {
                    _currentScore++;
                }
                _scoreText.text = _currentScore.ToString();
            }
        }
    }
}