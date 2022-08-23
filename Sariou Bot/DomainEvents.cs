using System;
using System.Collections.Generic;
using System.Text;

namespace Sariou_Bot
{
    public static class DomainEvents
    {
        private static List<Delegate> _callbacks;

        public static void Register<T>(Action<T> callback) where T : BaseDomainEvent
        {
            if(_callbacks == null)
                _callbacks = new List<Delegate>();
            _callbacks.Add(callback);
        }

        public static void Raise<T>(T args) where T: BaseDomainEvent
        {
            if (_callbacks is null) return;

            foreach (var callback in _callbacks)
            {
                if(callback is Action<T> action)
                {
                    action(args);
                }
            }
        }

        public static void Clear() 
        { 
            _callbacks?.Clear();    
        }
    }
}
