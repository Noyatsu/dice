using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SSTraveler.Utility.ReactiveProperty
{
    public static class ReactivePropertyUtility
    {
        /// <summary>
        /// GameObjectのDestroyに合わせてDisposeを実行する
        /// </summary>
        public static IDisposable AddTo(this IDisposable disposable, Component mono) => AddTo(disposable, mono.gameObject);

        /// <summary>
        /// GameObjectのDestroyに合わせてDisposeを実行する
        /// </summary>
        public static IDisposable AddTo(this IDisposable disposable, GameObject gameObject)
        {
            var disposer = gameObject.GetComponent<OnDestroyDisposer>();

            if (disposer == null) {
                disposer = gameObject.AddComponent<OnDestroyDisposer>();
            }

            // DisposerにDestroy時にDispose()するDisposableを登録
            disposer.RegisterDisposable(disposable);

            return disposable;
        }
        
        /// <summary>
        /// 購読する
        /// </summary>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onError = null, Action onComplete = null)
        {
            return observable.Subscribe(new Observer<T>(onNext, onError, onComplete));
        }
        
        /// <summary>
        /// UnityEventをObservableとして扱う
        /// </summary>
        public static IObservable<Unit> AsObservable(this UnityEvent evt)
        {
            return new UnityEventObservable(evt);
        }
        
        /// <summary>
        /// UnityEventをObservableとして扱う
        /// </summary>
        public static IObservable<T> AsObservable<T>(this UnityEvent<T> evt)
        {
            return new UnityEventObservable<T>(evt);
        }

    }
}
