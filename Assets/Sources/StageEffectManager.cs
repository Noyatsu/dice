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
        
        [Header("References")]
        [SerializeField] private Renderer _boardRenderer;
        [SerializeField] private Renderer _baseBoxRenderer;
        
        public void SetStage(int nextStage)
        {
            var nextSettings = _settings[nextStage];
            
            _boardRenderer.sharedMaterial.SetTexture("_MainTex", nextSettings.BoardTexture);
            _baseBoxRenderer.sharedMaterial.SetColor("_Color1", nextSettings.Color1);
            _baseBoxRenderer.sharedMaterial.SetColor("_Color2", nextSettings.Color2);
            RenderSettings.skybox = nextSettings.SkyboxMaterial;
            BgmManager.Instance.Play((nextStage + 1).ToString()); //BGM
        }
    }
}