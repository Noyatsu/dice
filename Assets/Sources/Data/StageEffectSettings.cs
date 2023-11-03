using UnityEngine;

namespace SSTraveler.Game.Data
{
    [CreateAssetMenu(fileName = "StageSettings", menuName = "SSTraveler/StageSettings", order = 0)]
    public class StageEffectSettings : ScriptableObject
    {
        public Texture2D BoardTexture;
        public Material SkyboxMaterial;
        public Color Color1;
        public Color Color2;
        public Color TextColor;
    }
}