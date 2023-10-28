//メモリ的にこれで本当にいいのかよくわかりませんでした

using UnityEngine;
using UnityEngine.Events;
namespace SSTraveler.Utility.ReactiveProperty
{
    /// <summary>
    /// UnityEventをObservableに変換
    /// </summary>
    public class UnityEventObservable : Observable<Unit>
    {
        private readonly UnityEvent _event;
        
        public UnityEventObservable(UnityEvent evt)
        {
            _event = evt;
            _event.AddListener(SendNextUnit);
        }

        private void SendNextUnit()
        {
            SendNext(Unit.Default);
        }
        
        ~UnityEventObservable()
        {
            _event.RemoveListener(SendNextUnit);
        }
    }
    
    
    /// <summary>
    /// UnityEventをObservableに変換
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityEventObservable<T> : Observable<T>
    {
        private readonly UnityEvent<T> _event;
        
        public UnityEventObservable(UnityEvent<T> evt)
        {
            _event = evt;
            _event.AddListener(SendNext);
        }

        ~UnityEventObservable()
        {
            _event.RemoveListener(SendNext);
        }
    }
}
