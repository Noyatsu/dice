using System;
using System.Collections;
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
        public DiceController this[int i] => GetInstanceAt(i);
        public IEnumerable<DiceController> DicePool => _dicePool;
        public int ActiveDiceCount => _dicePool.Length - _diceQueue.Count;

        [SerializeField] private GameObject _dicePrefab;
        [SerializeField] private int _poolSize = 49;
        
        private DiceController[] _dicePool;
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
            _dicePool = new DiceController[_poolSize];
            for (var i = 0; i < _poolSize; i++)
            {
                var obj = _container.InstantiatePrefab(_dicePrefab, transform);
                obj.SetActive(false);
                var dice = obj.GetComponent<DiceController>();
                dice.DiceId = i;
                _diceQueue.Enqueue(dice);
                _dicePool[i] = dice;
            }
            _isInit = true;
        }

        public DiceController GetNewInstance()
        {
            if (!_isInit) Init();
            
            var dice = _diceQueue.Dequeue();
            dice.ResetDice();
            dice.gameObject.SetActive(true);
            return dice;
        }
        
        public DiceController GetInstanceAt(int diceId)
        {
            if (!_isInit) Init();
            return _dicePool[diceId];
        }


        public void ReturnInstance(DiceController instance)
        {
            instance.gameObject.SetActive(false);
            _diceQueue.Enqueue(instance);
        }
    }
}