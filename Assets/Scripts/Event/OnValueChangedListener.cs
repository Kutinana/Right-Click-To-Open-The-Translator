using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Event
{
    public class OnValueChangedListener<T>
    {
        public delegate void OnValueChangeDelegate(T newVal);
        public event OnValueChangeDelegate OnVariableChange;
    
        private T m_value;
        public T Value
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_value.Equals(value)) return;
                OnVariableChange?.Invoke(value);
                m_value = value;
            }
        }
    }
}