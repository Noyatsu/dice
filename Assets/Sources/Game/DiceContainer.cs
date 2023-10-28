using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SSTraveler.Game
{
    /// <summary>
    /// さいころオブジェクトのオブジェクトプール
    /// </summary>
    public class DiceContainer : MonoBehaviour, IDiceContainer
    {
        [SerializeField] private GameObject _dicePrefab;
        [SerializeField] private int _poolSize = 49;

        private readonly Queue<DiceController> _diceQueue = new();
        private bool _isInit = false;
        private DiContainer _container;
        
        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Start()
        {
            Init();
        }
        
        public void Init()
        {
            if (_isInit) return;
            for (var i = 0; i < _poolSize; i++)
            {
                var obj = _container.InstantiatePrefab(_dicePrefab, transform);
                obj.SetActive(false);
                _diceQueue.Enqueue(obj.GetComponent<DiceController>());
            }
            _isInit = true;
        }

        public DiceController GetInstance()
        {
            if (!_isInit) Init();
            
            var dice = _diceQueue.Dequeue();
            dice.ResetDice();
            dice.gameObject.SetActive(true);
            return dice;
        }
        
        public void ReturnInstance(DiceController instance)
        {
            instance.gameObject.SetActive(false);
            _diceQueue.Enqueue(instance);
        }


    }
}