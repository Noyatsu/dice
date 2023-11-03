using SSTraveler.Game;
using SSTraveler.Utility.ReactiveProperty;
using TMPro;
using UnityEngine;
using Zenject;

namespace SSTraveler.Ui
{
    public class StatusTextController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _statusText;
        
        private int _lastFlame = 0;
        private IMainGameController _mainGameController;
        
        [Inject]
        public void Construct(IMainGameController mainGameController)
        {
            _mainGameController = mainGameController;
        }

        private void Start()
        {
            _mainGameController.StatusText.Subscribe(SetText).AddTo(this);
        }

        private void Update()
        {
            if (_lastFlame > 0)
            {
                _lastFlame--;
            }
            else if (_lastFlame == 0)
            {
                _statusText.text = "";
            }
        }

        public void SetText(string msg)
        {
            _lastFlame = 100;
            _statusText.text = msg;
        }
    }
}