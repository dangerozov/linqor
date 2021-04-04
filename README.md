# Linqor
[![Build Status](https://travis-ci.org/dangerozov/linqor.svg?branch=master)](https://travis-ci.org/dangerozov/linqor)

Linq extensions for ordered sequences. Have better perfomance in comparison to standard extensions. Lazily handle large or even infinite sequences.

**Concat**, **Distinct**, **Except**, **GroupBy**, **GroupJoin**, **Intersect**, **Join** and **Union** are available.

## Usage
Cast `IEnumerable<T>` to `OrderedEnumberable<T, TKey>` by calling `AsOrderedBy<T, TKey>`. Use extensions as usual.

```csharp
new[] { 1, 1, 2, 2, 2 }.AsOrderedBy(x => x).Distinct(); // [ 1, 2 ]
new[] { 1, 3, 5 }.AsOrderedBy(x => x).Concat(new[] { 2, 4, 6 }.AsOrderedBy(x => x)); // [ 1, 2, 3, 4, 5, 6 ]
```
## Difference to LINQ API
No `IComparer<T> comparer` or `Func<T, TKey> keySelector` parameters. `OrderedEnumerable<T, TKey>` ones are used instead.

`GroupJoin` and `Join` require additional `Func<TResult, TKey> resultKeySelector` parameter to construct `OrderedEnumerable<TResult, TKey>`.
