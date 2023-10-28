using System;
using System.Collections.Generic;
using UnityEngine;
namespace SSTraveler.Utility.ReactiveProperty
{
    /// <summary>
    /// GameObjectがなくなるときにDisposeするためのコンポーネント 
    /// </summary>
    [DisallowMultipleComponent]
    public class OnDestroyDisposer : MonoBehaviour
    {
        private List<IDisposable> _disposables = new List<IDisposable>();

        /// <summary>
        /// Destroy時にDisposeするDisposableを登録する
        /// </summary>
        /// <param name="disposable"></param>
        public void RegisterDisposable(IDisposable disposable)
        {
            if (!_disposables.Contains(disposable)) {
                _disposables.Add(disposable);
            }
        }
        
        private void OnDestroy()
        {
            foreach (var disposable in _disposables) {
                disposable.Dispose();
            }
        }
    }
}
