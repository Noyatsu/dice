using System;
using System.Collections.Generic;
using SSTraveler.Game.Data;
using UnityEngine;

namespace SSTraveler.Game
{
    public interface IStageEffectManager
    {
        public void SetStage(int nextStage);
    }
    public class StageEffectManager : MonoBehaviour, IStageEffectManager
    {
        [SerializeField] private List<StageEffectSettings> _settings = new();
        [SerializeField] private float _colorChangeSpeed = 0.003f;
        
        [Header("References")]
        [SerializeField] private Renderer _boardRenderer;
        [SerializeField] private Renderer _baseBoxRenderer;
        
        private Color _targetColor1;
        private Color _targetColor2;
        private Color _currentColor1;
        private Color _currentColor2;

        private void Awake()
        {
            _targetColor1 = _settings[^1].Color1;
            _targetColor2 = _settings[^1].Color2;
            _currentColor1 = _settings[^1].Color1;
            _currentColor2 = _settings[^1].Color2;
        }

        public void SetStage(int nextStage)
        {
            var nextSettings = _settings[nextStage];
            
            _boardRenderer.sharedMaterial.SetTexture("_MainTex", nextSettings.BoardTexture);
            _targetColor1 = nextSettings.Color1;
            _targetColor2 = nextSettings.Color2;
            RenderSettings.skybox = nextSettings.SkyboxMaterial;
            BgmManager.Instance.Play((nextStage + 1).ToString()); //BGM
        }

        private void Update()
        {
            _currentColor1 = Color.Lerp(_currentColor1, _targetColor1, _colorChangeSpeed);
            _currentColor2 = Color.Lerp(_currentColor2, _targetColor2, _colorChangeSpeed);
            _baseBoxRenderer.sharedMaterial.SetColor("_Color1", _currentColor1);
            _baseBoxRenderer.sharedMaterial.SetColor("_Color2", _currentColor2);
        }
    }
}