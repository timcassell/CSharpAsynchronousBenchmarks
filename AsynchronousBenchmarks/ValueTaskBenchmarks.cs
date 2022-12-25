using BenchmarkDotNet.Attributes;
using Cysharp.Threading.Tasks;
using Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace AsynchronousBenchmarks
{
    public class ReusableValueTaskCompletionSource : IValueTaskSource
    {
        private static readonly Stack<ReusableValueTaskCompletionSource> _pool = new Stack<ReusableValueTaskCompletionSource>(2);

        private static int _idGenerator;
        public int _id = Interlocked.Increment(ref _idGenerator);
        private Action<object> _continuation;
        private object _state;

        private ReusableValueTaskCompletionSource() { }

        public static ReusableValueTaskCompletionSource Create()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }
            return new ReusableValueTaskCompletionSource();
        }

        public void GetResult(short token)
        {
            _continuation = null;
            _state = null;
            _pool.Push(this);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return ValueTaskSourceStatus.Pending;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            _continuation = continuation;
            _state = state;
        }

        public void Complete()
        {
            _continuation.Invoke(_state);
        }
    }

    public class ValueTaskAsyncHelper
    {
        public bool hasInvoked;
        private readonly bool _isPending;
        private readonly Stack<ReusableValueTaskCompletionSource> _completionSources;

        public ValueTaskAsyncHelper(bool isPending)
        {
            _isPending = isPending;
            _completionSources = new Stack<ReusableValueTaskCompletionSource>(2);
        }

        public void ResolvePending()
        {
            while (_completionSources.Count > 0)
            {
                _completionSources.Pop().Complete();
            }
        }

        public ValueTask GetBaseValueTask()
        {
            if (!_isPending)
            {
                return new ValueTask();
            }
            var completionSource = ReusableValueTaskCompletionSource.Create();
            _completionSources.Push(completionSource);
            return new ValueTask(completionSource, 0);
        }
    }

    partial class AsyncAwait
    {
        // static to match ContinueWith
        private static ValueTaskAsyncHelper _valueTaskHelper;

        [GlobalSetup(Target = nameof(ValueTask))]
        public void ValueTaskSetup()
        {
            _valueTaskHelper = new ValueTaskAsyncHelper(Pending);
            // Run once in setup to initialize static memory so it isn't counted in the survived memory measurement.
            _valueTaskHelper.hasInvoked = true;
            ValueTask();
            _valueTaskHelper.hasInvoked = false;
        }

        [Benchmark]
        public void ValueTask()
        {
            _ = ValueTask_ExecuteAsync();
            if (!_valueTaskHelper.hasInvoked)
            {
                // Execute again the first time to measure survived memory.
                // Subsequent runs only execute once to measure the execution time.
                _valueTaskHelper.hasInvoked = true;
                _ = ValueTask_ExecuteAsync();
            }
            _valueTaskHelper.ResolvePending();
        }

        private static async ValueTask ValueTask_ExecuteAsync()
        {
            await ValueTask_GetAndConsumeValuesAsync<Struct32, object>();
            await ValueTask_GetAndConsumeValuesAsync<object, Struct32>();
        }

        private static async ValueTask ValueTask_GetAndConsumeValuesAsync<T1, T2>()
        {
            _ = await ValueTask_GetValueAsync<T1>();
            _ = await ValueTask_GetValueAsync<T2>();
        }

        private static async ValueTask<T> ValueTask_GetValueAsync<T>()
        {
            await _valueTaskHelper.GetBaseValueTask();
            return default;
        }
    }

    partial class ContinueWith
    {
        [Benchmark]
        public void ValueTask()
        {
            // ValueTask does not have a ContinueWith or Then API, so we just throw NotImplementedException.
            throw new System.NotImplementedException();
        }
    }
}