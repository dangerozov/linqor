﻿using System;
using System.Collections.Generic;

namespace Linqor
{
    public static partial class Extensions
    {
        /// <summary>
        /// Correlates the elements of two ordered sequences based on matching keys.
        /// </summary>
        /// <param name="outer">The first ordered sequence to join.</param>
        /// <param name="inner">
        /// The sequence that follows same ordering rules as the first sequence
        /// to join to the first sequence.
        /// </param>
        /// <param name="resultSelector">
        /// A function to extract the join key from each element of the second sequence.
        /// </param>
        /// <param name="resultSelector">
        /// A function to create a result element from two matching elements.
        /// </param>
        /// <param name="resultKeySelector">A function to extract the key from each element of the result sequence.</param>
        public static OrderedEnumerable<TResult, TKey> Join<TOuter, TInner, TKey, TResult>(
            this OrderedEnumerable<TOuter, TKey> outer,
            IEnumerable<TInner> inner,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            Func<TResult, TKey> resultKeySelector)
        {
            return Join(
                    outer,
                    inner.AsOrderedLike(innerKeySelector, outer),
                    outer.KeySelector,
                    innerKeySelector,
                    resultSelector,
                    outer.KeyComparer)
                .AsOrderedLike(resultKeySelector, outer);
        }

        private static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
            IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IComparer<TKey> keyComparer)
        {
            using (var leftEnumerator = outer.GetEnumerator())
            using (var rightEnumerator = inner.GetEnumerator())
            {
                EnumeratorState<TOuter> leftState = leftEnumerator.Next();
                EnumeratorState<TInner> rightState = rightEnumerator.Next();
                
                while (leftState.HasCurrent && rightState.HasCurrent)
                {
                    switch(keyComparer.Compare(outerKeySelector(leftState.Current), innerKeySelector(rightState.Current)))
                    {
                        case -1:
                            leftState = leftEnumerator.Next();
                            break;
                        case 0:
                            yield return resultSelector(leftState.Current, rightState.Current);

                            TKey currentRightKey = innerKeySelector(rightState.Current);
                            List<TInner> elements = new List<TInner> { rightState.Current };
                            foreach(TInner rightItem in rightEnumerator.TakeWhile(current => keyComparer.Compare(currentRightKey, innerKeySelector(current)) == 0, last => rightState = last))
                            {
                                elements.Add(rightItem);
                                yield return resultSelector(leftState.Current, rightItem);
                            }

                            TKey currentLeftKey = outerKeySelector(leftState.Current);
                            foreach (var leftItem in leftEnumerator.TakeWhile(current => keyComparer.Compare(currentLeftKey, outerKeySelector(current)) == 0, last => leftState = last))
                            {
                                foreach(TInner rightItem in elements)
                                {
                                    yield return resultSelector(leftItem, rightItem);
                                }
                            }
                            break;
                        case 1:
                            rightState = rightEnumerator.Next();
                            break;
                    }
                }
            }
        }
    }
}
