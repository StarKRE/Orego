using System;
using System.Collections;
using System.Collections.Generic;

namespace OregoFramework.Util
{
    public sealed class AsyncEvent : AbstractAsyncEvent<Func<IEnumerator>>
    {
        public IEnumerator Invoke()
        {
            yield return this.ForEach(this.Invoke);
        }

        private IEnumerator Invoke(Func<IEnumerator> asyncListener)
        {
            yield return asyncListener.Invoke();
        }
    }

    public sealed class AsyncEvent<T1> : AbstractAsyncEvent<Func<T1, IEnumerator>>
    {
        public IEnumerator Invoke(T1 t1)
        {
            yield return this.ForEach(asyncListener => this.Invoke(asyncListener, t1));
        }

        private IEnumerator Invoke(Func<T1, IEnumerator> asyncListener, T1 t1)
        {
            yield return asyncListener.Invoke(t1);
        }
    }

    public sealed class AsyncEvent<T1, T2> : AbstractAsyncEvent<Func<T1, T2, IEnumerator>>
    {
        public IEnumerator Invoke(T1 t1, T2 t2)
        {
            yield return this.ForEach(asyncListener => this.Invoke(asyncListener, t1, t2));
        }

        private IEnumerator Invoke(Func<T1, T2, IEnumerator> asyncListener, T1 t1, T2 t2)
        {
            yield return asyncListener.Invoke(t1, t2);
        }
    }

    public sealed class AsyncEvent<T1, T2, T3> : AbstractAsyncEvent<Func<T1, T2, T3, IEnumerator>>
    {
        public IEnumerator Invoke(T1 t1, T2 t2, T3 t3)
        {
            yield return this.ForEach(asyncListener => this.Invoke(asyncListener, t1, t2, t3));
        }

        private IEnumerator Invoke(Func<T1, T2, T3, IEnumerator> asyncListener, T1 t1, T2 t2, T3 t3)
        {
            yield return asyncListener.Invoke(t1, t2, t3);
        }
    }

    public abstract class AbstractAsyncEvent<T>
    {
        private bool isProcessing;

        private readonly List<T> asyncListeners;

        private readonly Queue<Func<IEnumerator>> asyncActionQueue;

        protected AbstractAsyncEvent()
        {
            this.asyncListeners = new List<T>();
            this.asyncActionQueue = new Queue<Func<IEnumerator>>();
        }

        public void AddListener(T asyncListener, int index = -Int.ZERO)
        {
            if (asyncListener != null)
            {
                this.Try(() => this.asyncListeners.Insert(asyncListener, index));
            }
        }

        public void RemoveListener(T asyncListener)
        {
            if (asyncListener != null)
            {
                this.Try(() => this.asyncListeners.Remove(asyncListener));
            }
        }

        public void ClearListeners()
        {
            this.Try(this.asyncListeners.Clear);
        }

        private void Try(Action action)
        {
            if (this.isProcessing)
            {
                this.asyncActionQueue.Enqueue(() => this.InvokeAsync(action));
            }
            else
            {
                action.Invoke();
            }
        }

        private IEnumerator InvokeAsync(Action action)
        {
            action.Invoke();
            yield break;
        }

        protected IEnumerator ForEach(Func<T, IEnumerator> asyncAction)
        {
            if (this.isProcessing)
            {
                this.asyncActionQueue.Enqueue(() => this.ForEach(asyncAction));
                yield break;
            }

            this.isProcessing = true;
            yield return this.ProcessListeners(asyncAction);
            this.isProcessing = false;
            while (this.asyncActionQueue.IsNotEmpty())
            {
                var defferedAction = this.asyncActionQueue.Dequeue();
                yield return defferedAction.Invoke();
            }
        }

        private IEnumerator ProcessListeners(Func<T, IEnumerator> asyncAction)
        {
            var listeners = new List<T>(this.asyncListeners);
            foreach (var listener in listeners)
            {
                yield return asyncAction.Invoke(listener);
            }

            listeners.Clear();
        }
    }
}