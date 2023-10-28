using System;
using System.Collections.Generic;

namespace SSTraveler.Utility.ReactiveProperty
{
    /// <summary>
    /// 通知を発行するオブジェクト
    /// 値の発行先として登録されたIObserver[T]に対して値を発行する
    /// </summary>
    public class Observable<T> : IObservable<T>
    {
        private List<IObserver<T>> _observers;

        /// <summary>
        /// 購読する
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers ??= new List<IObserver<T>>();

            if (!_observers.Contains(observer)) {
                _observers.Add(observer);
            }

            return new Disposer(_observers, observer);
        }

        /// <summary>
        /// 変更通知を発行
        /// </summary>
        protected void SendNext(T value)
        {
            if (_observers == null) return;
            
            foreach (var observer in _observers) {
                observer.OnNext(value);
            }
        }
        
        /// <summary>
        /// エラー通知を発行
        /// </summary>
        protected void SendError(Exception ex)
        {
            if (_observers == null) return;
            
            foreach (var observer in _observers) {
                observer.OnError(ex);
            }
        }
        
        /// <summary>
        /// 完了通知を発行
        /// </summary>
        protected void SendComplete()
        {
            if (_observers == null) return;

            foreach (var observer in _observers) {
                observer.OnCompleted();
            }
        }
        
        /// <summary>
        /// 購読停止をするもの
        /// </summary>
        private class Disposer : IDisposable
        {
            private readonly List<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;
            
            public Disposer(List<IObserver<T>> observers, IObserver<T> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer)) {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
