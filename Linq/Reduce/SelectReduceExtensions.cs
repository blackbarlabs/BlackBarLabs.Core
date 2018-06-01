﻿using BlackBarLabs.Extensions;
using EastFive.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EastFive.Linq
{
    public static class SelectReduceExtensions
    {
        public static TSelect[] SelectReduce<TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            Func<TItem, Func<TSelect, TSelect[]>, TSelect[]> select)
        {
            return items.SelectReduce(
                (item, next, skip) => select(item, next),
                (TSelect[] selections) => selections);
        }

        public static TResult SelectReduce<TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            Func<TItem, Func<TSelect, TResult>, TResult> select,
            Func<TSelect[], TResult> reduce)
        {
            return items.SelectReduce(
                (item, next, skip) => select(item, next),
                reduce);
        }

        public static TSelect[] SelectReduce<TItem, TSelect>(this IEnumerable<TItem> items,
            Func<TItem, Func<TSelect, TSelect[]>, Func<TSelect[]>, TSelect[]> select,
            int recursionLimit = int.MaxValue)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.SelectReduce(new TSelect[] { }, select,
                (TSelect[] selections) => selections,
                0, recursionLimit);
        }

        public static TResult SelectReduce<TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            Func<TItem, Func<TSelect, TResult>, Func<TResult>, TResult> select,
            Func<TSelect[], TResult> reduce,
            int recursionLimit = int.MaxValue)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.SelectReduce(new TSelect[] { }, select,
                (rs) => reduce(rs.ToArray()),
                0, recursionLimit);
        }

        private static TResult SelectReduce<TItem, TSelect, TResult>(this IEnumerator<TItem> items,
            TSelect[] selections,
            Func<TItem, Func<TSelect, TResult>, Func<TResult>, TResult> select,
            Func<TSelect[], TResult> reduce)
        {
            if (!items.MoveNext())
                return reduce(selections.ToArray());

            return select(items.Current,
                (r) =>
                {
                    return items.SelectReduce(selections.Append(r).ToArray(), select, reduce);
                },
                () =>
                {
                    return items.SelectReduce(selections, select, reduce);
                });
        }

        private static TResult SelectReduce<TItem, TSelect, TResult>(this IEnumerator<TItem> items,
            TSelect[] selections,
            Func<TItem, Func<TSelect, TResult>, Func<TResult>, TResult> select,
            Func<TSelect[], TResult> reduce,
            int depth, int recursionLimit)
        {
            if (depth >= recursionLimit)
            {
                var result = default(TResult);
                var thread = new Thread(
                    () =>
                    {
                        result = SelectReduce(items, selections, select, reduce, 0, recursionLimit);

                    });
                thread.Start();
                thread.Join();
                return result;
            }

            if (!items.MoveNext())
                return reduce(selections.ToArray());

            return select(items.Current,
                (r) =>
                {
                    return items.SelectReduce(selections.Append(r).ToArray(), select, reduce, depth + 1, recursionLimit);
                },
                () =>
                {
                    return items.SelectReduce(selections, select, reduce, depth + 1, recursionLimit);
                });
        }

        public static TResult SelectReduce<TItem, T1, TSelect, TResult>(this IEnumerable<TItem> items,
                T1 item1,
            Func<TItem, T1, Func<TSelect, T1, TResult>, Func<T1, TResult>, TResult> select,
            Func<TSelect[], T1, TResult> reduce,
            int recursionLimit = int.MaxValue)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.SelectReduce(item1, new TSelect[] { }, select,
                (rs, items1New) =>
                    reduce(rs.ToArray(), items1New),
                0, recursionLimit);
        }

        private static TResult SelectReduce<TItem, T1, TSelect, TResult>(this IEnumerator<TItem> items,
                T1 item1,
                TSelect[] selections,
            Func<TItem, T1, Func<TSelect, T1, TResult>, Func<T1, TResult>, TResult> select,
            Func<TSelect[], T1, TResult> reduce)
        {
            if (!items.MoveNext())
                return reduce(selections.ToArray(), item1);

            return select(items.Current, item1,
                (r, item1New) =>
                {
                    return items.SelectReduce(item1New, selections.Append(r).ToArray(), select, reduce);
                },
                (item1New) =>
                {
                    return items.SelectReduce(item1New, selections, select, reduce);
                });
        }

        private static TResult SelectReduce<TItem, T1, TSelect, TResult>(this IEnumerator<TItem> items,
                T1 item1,
                TSelect[] selections,
            Func<TItem, T1, Func<TSelect, T1, TResult>, Func<T1, TResult>, TResult> select,
            Func<TSelect[], T1, TResult> reduce,
            int depth, int recursionLimit)
        {
            if(depth >= recursionLimit)
            {
                var result = default(TResult);
                var thread = new Thread(
                    () =>
                    {
                        result = SelectReduce(items, item1, selections, select, reduce, 0, recursionLimit);

                    });
                thread.Start();
                thread.Join();
                return result;
            }

            if (!items.MoveNext())
                return reduce(selections.ToArray(), item1);

            return select(items.Current, item1,
                (r, item1New) =>
                {
                    return items.SelectReduce(item1New, selections.Append(r).ToArray(), select, reduce, depth+1, recursionLimit);
                },
                (item1New) =>
                {
                    return items.SelectReduce(item1New, selections, select, reduce, depth + 1, recursionLimit);
                });
        }

        public static TResult SelectReduce<TItem, T1, T2, TSelect, TResult>(this IEnumerable<TItem> items,
                T1 item1, T2 item2,
            Func<TItem, T1, T2, Func<TSelect, T1, T2, TResult>, Func<T1, T2, TResult>, TResult> select,
            Func<TSelect[], T1, T2, TResult> reduce)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.SelectReduce(item1, item2, new TSelect[] { }, select,
                (rs, items1New, items2New) =>
                    reduce(rs.ToArray(), items1New, items2New));
        }

        private static TResult SelectReduce<TItem, T1, T2, TSelect, TResult>(this IEnumerator<TItem> items,
                T1 item1, T2 item2,
                TSelect[] selections,
            Func<TItem, T1, T2, Func<TSelect, T1, T2, TResult>, Func<T1, T2, TResult>, TResult> select,
            Func<TSelect[], T1, T2, TResult> reduce)
        {
            if (!items.MoveNext())
                return reduce(selections.ToArray(), item1, item2);

            return select(items.Current, item1, item2,
                (r, item1New, item2New) =>
                {
                    return items.SelectReduce(item1New, item2New, selections.Append(r).ToArray(), select, reduce);
                },
                (item1New, item2New) =>
                {
                    return items.SelectReduce(item1New, item2New, selections, select, reduce);
                });
        }

        public static TResult Reduce<T1, T2, TItem, TResult>(this IEnumerable<TItem> items,
            TResult initial, T1 v1, T2 v2,
            Func<TResult, T1, T2, TItem, Func<T1, T2, TResult, TResult>, TResult> callback)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.Reduce(initial, v1, v2, callback);
        }

        private static TResult Reduce<T1, T2, TItem, TResult>(this IEnumerator<TItem> items,
            TResult initial, T1 v1, T2 v2,
            Func<TResult, T1, T2, TItem, Func<T1, T2, TResult, TResult>, TResult> callback)
        {
            if (!items.MoveNext())
                return initial;

            return callback(initial, v1, v2, items.Current,
                (v1next, v2next, r) => items.Reduce(r, v1next, v2next, callback));
        }

        public static TResult Reduce<TItem, TResult>(this IEnumerable<TItem> items,
           Func<TItem, Func<TResult>, TResult> callback, 
           Func<TResult> onEmpty)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.Reduce(callback, onEmpty);
        }

        private static TResult Reduce<TItem, TResult>(this IEnumerator<TItem> items,
            Func<TItem, Func<TResult>, TResult> callback,
            Func<TResult> onEmpty)
        {
            if (!items.MoveNext())
                return onEmpty();

            return callback(items.Current,
                () => items.Reduce(callback, onEmpty));
        }
        
        public static IEnumerable<TResult> ReduceItems<T1, TItem, TResult>(this IEnumerable<TItem> items,
            T1 v1,
            Func<
                T1, TItem,
                Func<TResult, T1, IEnumerable<TResult>>,  // next
                Func<T1, IEnumerable<TResult>>, // skip
                IEnumerable<TResult>> callback)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.ReduceItems(v1, callback,
                (v1_, items_) => items_);
        }

        private struct ReduceItemStep<TSelection>
        {
            public ManualResetEvent trigger;
            public TSelection selection;
        }

        public static TResult ReduceItems<TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            Func<
                TItem,
                Func<TSelect, TResult>,  // next
                Func<TResult>, // skip
                TResult> callback,
            Func<TSelect[], TResult> complete)
        {
            var block = new ManualResetEvent(false);
            TResult result = default(TResult);

            var steps = items
                .Select(
                    item =>
                    {
                        var step = new ReduceItemStep<TSelect>
                        {
                            trigger = new ManualResetEvent(false),
                        };
                        callback(
                            item,
                            (selection) =>
                            {
                                step.selection = selection;
                                step.trigger.Set();
                                block.WaitOne();
                                return result;
                            },
                            () =>
                            {
                                step.trigger.Set();
                                block.WaitOne();
                                return result;
                            });
                        return step;
                    })
                .Select(step =>
                    {
                        step.trigger.WaitOne();
                        return step.selection;
                    });
            result = complete(steps.ToArray());
            block.Set();
            return result;
        }

        public static TResult ReduceItems<T1, TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            T1 v1,
            Func<
                T1, TItem,
                Func<TSelect, T1, TResult>,  // next
                Func<T1, TResult>, // skip
                TResult> callback,
            Func<T1, IEnumerable<TSelect>, TResult> complete)
        {
            TResult result = default(TResult);
            var block = new ManualResetEvent(false);
            
            return items
                .Aggregate(
                    v1.PairWithValue(new TSelect[] { }),
                    (valueKvp, item) =>
                    {
                        T1 vNext = default(T1);
                        var selected = valueKvp.Value;
                        var trigger = new ManualResetEvent(false);
                        var task = new Task(
                            () => callback(
                                valueKvp.Key,
                                item,
                                (selection, valueNext) =>
                                {
                                    vNext = valueNext;
                                    selected = selected.Append(selection).ToArray();
                                    trigger.Set();
                                    block.WaitOne();
                                    return result;
                                },
                                (valueNext) =>
                                {
                                    vNext = valueNext;
                                    trigger.Set();
                                    block.WaitOne();
                                    return result;
                                }));
                        task.Start();
                        trigger.WaitOne();
                        return vNext.PairWithValue(selected);
                    },
                    final =>
                    {
                        result = complete(final.Key, final.Value);
                        block.Set();
                        return result;
                    });
        }

        public static TResult ReduceItemsX<T1, TItem, TSelect, TResult>(this IEnumerable<TItem> items,
            T1 v1,
            Func<
                T1, TItem,
                Func<TSelect, T1, TResult>,  // next
                Func<T1, TResult>, // skip
                TResult> callback,
            Func<T1, IEnumerable<TSelect>, TResult> complete)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.ReduceItems(v1, callback, complete);
        }

        private static TResult ReduceItems<T1, TItem, TSelect, TResult>(this IEnumerator<TItem> items,
            T1 v1,
            Func<
                T1, TItem,
                Func<TSelect, T1, TResult>,  // next
                Func<T1, TResult>, // skip
                TResult> callback,
            Func<T1, IEnumerable<TSelect>, TResult> complete)
        {
            if (!items.MoveNext())
                return complete(v1, new TSelect[] { });



            return callback(v1, items.Current,
                (r, v1next) => items.ReduceItems(v1next, callback, (v1_, items_) => complete(v1_, items_.Append(r))),
                (v1next) => items.ReduceItems(v1next, callback, complete));
        }

        //private static IEnumerable<TResult> ReduceItems<T1, TItem, TResult>(this IEnumerator<TItem> items,
        //    T1 v1,
        //    Func<
        //        T1, TItem,
        //        Func<TResult, T1, IEnumerable<TResult>>,  // next
        //        Func<T1, IEnumerable<TResult>>, // skip
        //        IEnumerable<TResult>> callback)
        //{
        //    if (!items.MoveNext())
        //        return new TResult[] { };

        //    return callback(v1, items.Current,
        //        (r, v1next) => items.ReduceItems(v1next, callback).Append(r),
        //        (v1next) => items.ReduceItems(v1next, callback));
        //}

        public static IEnumerable<TResult> ReduceItems<T1, T2, TItem, TResult>(this IEnumerable<TItem> items,
            T1 v1, T2 v2,
            Func<
                T1, T2, TItem,
                Func<T1, T2, TResult, IEnumerable<TResult>>,  // next
                Func<T1, T2, IEnumerable<TResult>>, // skip
                IEnumerable<TResult>> callback)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.ReduceItems(v1, v2, callback);
        }

        private static IEnumerable<TResult> ReduceItems<T1, T2, TItem, TResult>(this IEnumerator<TItem> items,
            T1 v1, T2 v2,
            Func<
                T1, T2, TItem,
                Func<T1, T2, TResult, IEnumerable<TResult>>,  // next
                Func<T1, T2, IEnumerable<TResult>>, // skip
                IEnumerable<TResult>> callback)
        {
            if (!items.MoveNext())
                return new TResult[] { };

            return callback(v1, v2, items.Current,
                (v1next, v2next, r) => items.ReduceItems(v1next, v2next, callback).Append(r),
                (v1next, v2next) => items.ReduceItems(v1next, v2next, callback));
        }

        public static TResult First<TITem, TResult>(this IEnumerable<TITem> items,
            Func<TITem, Func<TResult>, TResult> next,
            Func<TResult> onNotFound)
        {
            var enumerator = items.GetEnumerator();
            return enumerator.First(next, onNotFound);
        }

        public static TResult First<TITem, TResult>(this IEnumerator<TITem> items,
            Func<TITem, Func<TResult>, TResult> next,
            Func<TResult> onNotFound)
        {
            if (!items.MoveNext())
                return onNotFound();

            return next(items.Current, () => items.First(next, onNotFound));
        }
    }
}