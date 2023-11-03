using SSTraveler.Game;
using SSTraveler.Utility.ReactiveProperty;
using TMPro;
using UnityEngine;
using Zenject;

namespace SSTraveler.Ui
{
    public class ScreenTextController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _screenText;
        
        private int _lastFlame = 100;
        private IMainGameController _mainGameController;
        
        [Inject]
        public void Construct(IMainGameController mainGameController)
        {
            _mainGameController = mainGameController;
        }

        private void Start()
        {
            _mainGameController.ScreenText.Subscribe(SetText).AddTo(this);
        }

        private void Update()
        {
            if (_lastFlame > 0)
            {
                _lastFlame--;
            }
            else if (_lastFlame == 0)
            {
                _screenText.text = "";
            }
        }

        public void SetText(string msg)
        {
            _lastFlame = 70;
            _screenText.text = msg;
        }
    }
}