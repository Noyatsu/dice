using System;
using System.Collections.Generic;
namespace SSTraveler.Utility.ReactiveProperty
{
    /// <summary>
    /// 値に変更があったらそれを通知する
    /// </summary>
    public class ReactiveProperty<T> : Observable<T>
    {
        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                // setされた値が前の値と異なる場合、値を変更して通知する
                if (!EqualityComparer<T>.Default.Equals(_value,  value)) {
                    _value = value;
                    SendNext(_value);
                }
            }
        }

        public ReactiveProperty() { }
        public ReactiveProperty(T value) => _value = value;

        public static implicit operator T(ReactiveProperty<T> reactive)
        {
            return reactive.Value;
        }
    }
}
