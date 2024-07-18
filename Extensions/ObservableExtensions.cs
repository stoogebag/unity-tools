using System;
using System.Threading.Tasks;
using UniRx;

namespace stoogebag.Extensions
{
    public static class ObservableExtensions
    {
    
        //
        // public static IDisposable SubscribeAsync<TResult>(this IObservable<TResult> source, Func<AsyncResult,  Task> action) =>
        //     source.Select(_ => Observable.FromAsyncPattern(action))
        //         .Concat()
        //         .Subscribe();
        //
        //
        //
        // public static IObservable<TResult> SelectAsync<TSource, TResult>(this IObservable<TSource> source, Func<TSource, Task<TResult>> projectAsync)
        // {
        //     return Observable.Create<TResult>(
        //         observer =>
        //         {
        //             var throttle = new BehaviorSubject<TResult>(default);
        //
        //             var observable = source
        //                 .Zip(throttle, (value, _) => value)
        //                 .SelectMany(value => Observable.Defer(() => Observable.StartAsync(() => projectAsync(value))))
        //                 .Publish();
        //
        //             return new CompositeDisposable(
        //                 observable.Subscribe(throttle),
        //                 observable.Subscribe(observer),
        //                 observable.Connect(),
        //                 throttle
        //             );
        //         }
        //     );
        // }
        
        
        public static IObservable<Tuple<TSource, TSource>>
            PairWithPrevious<TSource>(this IObservable<TSource> source)
        {
            return source.Scan(
                Tuple.Create(default(TSource), default(TSource)),
                (acc, current) => Tuple.Create(acc.Item2, current));
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> source, Func<T, Task> onNext,
            Action<Exception> onError = null, Action onComplete = null)
        {
            //argument error checking omitted for brevity
            T current = default(T);
            bool processing = false;
            bool haveCurrent = false;

            return source
                .Where((v) =>
                {
                    if (processing)
                    {
                        current = v;
                        haveCurrent = true;
                    }

                    return !processing;
                })
                .Subscribe((v) =>
                    {
                        Action<Task> runNext = null;
                        runNext = (task) =>
                        {
                            if (haveCurrent)
                            {
                                haveCurrent = false;
                                onNext(current).ContinueWith(runNext);
                            }
                            else
                            {
                                processing = false;
                            }
                        };
                        processing = true;
                        onNext(v).ContinueWith(runNext);
                    },
                    onError,
                    onComplete);
        }
    }
}