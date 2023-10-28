using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SSTraveler.Game
{
    public class CubeMover : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _speedMinMax = new(-0.2f, 0.2f);
        
        private Transform _transform;
        private Vector3 _startPosition;
        private float _rand;
        private float _offset;
        
        private void Start()
        {
            _transform = transform;
            _startPosition = _transform.position;
            _rand = Random.value;
            _offset = Random.value * 2f * Mathf.PI;
        }

        private void Update()
        {
            _transform.position = _startPosition + new Vector3(0, Mathf.Sin(Time.time * Mathf.Lerp(_speedMinMax.x, _speedMinMax.y, _rand) + _offset), 0);
        }
    }
}