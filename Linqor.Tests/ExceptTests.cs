﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static Linqor.Tests.Helpers;

namespace Linqor.Tests
{
    [TestFixture]
    public class ExceptTests
    {
        [TestCaseSource(nameof(GetTestCases))]
        public string[] Except(string[] left, string[] right)
        {
            return Extensions
                .Except(
                    left.AsOrderedBy(NumberInString),
                    right)
                .ToArray();
        }

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            var testCases = new[]
            {
                (new string[] { }, new string[] { }, new string[] { }),
                (new[] { "L0", "L1", "L2" }, new string[] { }, new[] { "L0", "L1", "L2" }),
                (new string[] { }, new[] { "R0", "R1", "R2" }, new string[] { }),

                (new[] { "L0" }, new[] { "R0" }, new string[] { }),
                (new[] { "L0", "L1", "L2" }, new[] { "R0", "R1", "R2" }, new string[] { }),

                (new[] { "L0", "L1", "L2" }, new[] { "R2", "R3", "R4" }, new string[] { "L0", "L1" }),
                (new[] { "L2", "L3", "L4" }, new[] { "R0", "R1", "R2" }, new string[] { "L3", "L4" }),

                (new[] { "L0", "L0", "L1", "L2", "L2" }, new[] { "R0", "R1", "R1", "R2" }, new string[] { }),
                (new[] { "L0", "L0", "L1", "L3", "L3" }, new[] { "R1", "R1", "R2", "R2", "R3" }, new[] { "L0" }),

                (new[] { "L0", "L1", "L2", "L3", "L4", "L5" }, new string[] { "R3", "R4", "R5", "R6", "R7", "R8" }, new string[] { "L0", "L1", "L2" }),
            };

            var linqTestCases = testCases
                .Select(c => (c.Item1, c.Item2, Enumerable
                    .Except(c.Item1, c.Item2, new NumberInStringComparer())
                    .ToArray()));

            return testCases.Concat(linqTestCases)
                .Select((c, index) => new TestCaseData(c.Item1, c.Item2).Returns(c.Item3).SetName($"Except {Helpers.Get2DID(index, testCases.Length)}"));
        }

        [TestCaseSource(nameof(GetErrorCases))]
        public void ExceptError(string[] left, string[] right)
        {
            var result = Extensions
                .Except(
                    left.AsOrderedBy(NumberInString),
                    right);

            Assert.That(() => result.ToArray(), Throws.TypeOf<UnorderedElementDetectedException>());
        }

        private static IEnumerable<TestCaseData> GetErrorCases()
        {
            var exceptionTestCases = new[]
            {
                (new[] { "L0", "L1" }, new[] { "R1", "R0" }),
                (new[] { "L1", "L0" }, new[] { "R0", "R1" }),
            };

            return exceptionTestCases
                .Select(c => new TestCaseData(c.Item1, c.Item2));
        }
    }
}
