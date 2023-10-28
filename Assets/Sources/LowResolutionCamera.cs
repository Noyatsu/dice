using UnityEngine;
using UnityEngine.Rendering;

namespace SSTraveler.Game
{
    /// <summary>
    /// カメラが映すものを低解像度に設定できるようにするコンポーネント
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class LowResolutionCamera : MonoBehaviour
    {
        /// <summary>
        /// デフォルト解像度
        /// </summary>
        private readonly Vector2 _resolution = new Vector2(750, 1334);

        /// <summary>
        /// 解像度係数
        /// </summary>
        [SerializeField, Range(0.1f, 1)] private float _resolutionWeight = 1f;

        private float _currentResolutionWeight = 1f;

        private RenderTexture _renderTexture;
        private Camera _camera;
        private Camera _subCamera;

        private void Start()
        {
            SetResolution(_resolutionWeight);
        }

        /// <summary>
        /// 解像度を設定
        /// </summary>
        public void SetResolution(float resolutionWeight)
        {
            _resolutionWeight = resolutionWeight;
            _currentResolutionWeight = resolutionWeight;

            // 指定解像度に合わせたレンダーテクスチャーを作成
            _renderTexture = new RenderTexture(
                width: (int)(_resolution.x * _currentResolutionWeight),
                height: (int)(_resolution.y * _currentResolutionWeight),
                depth: 24
            );
            _renderTexture.useMipMap = false;
            _renderTexture.filterMode = FilterMode.Point;
            // カメラのレンダーテクスチャーを設定
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            _camera.targetTexture = _renderTexture;

            // レンダーテクスチャーを表示するサブカメラを設定
            if (_subCamera == null)
            {
                GameObject cameraObject = new GameObject("SubCamera");
                _subCamera = cameraObject.AddComponent<Camera>();
                _subCamera.cullingMask = 0;
                _subCamera.transform.parent = transform;
            }

            CommandBuffer commandBuffer = new CommandBuffer();
            commandBuffer.Blit((RenderTargetIdentifier)_renderTexture, BuiltinRenderTextureType.CameraTarget);
            _subCamera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
        }

        private void Update()
        {
            if (_currentResolutionWeight == _resolutionWeight) return;
            // _resolutionWeightの値が更新された時だけ解像度変更処理を呼ぶ
            // Inspector上で_resolutionWeightを操作するとき用の処理
            SetResolution(_resolutionWeight);
        }
    }
}