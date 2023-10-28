using System;

namespace SSTraveler.Utility.ReactiveProperty
{
    /// <summary>
    /// 通知を受け取る(各イベントを持っておく)オブジェクト
    /// </summary>
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onComplete;

        public Observer(Action<T> onNext = null, Action<Exception> onError = null, Action onCompleted = null)
        {
            _onNext = onNext;
            _onError = onError;
            _onComplete = onCompleted;
        }

        public void OnNext(T value) => _onNext?.Invoke(value);
        public void OnError(Exception error) => _onError?.Invoke(error);
        public void OnCompleted() => _onComplete?.Invoke();
    }
}
