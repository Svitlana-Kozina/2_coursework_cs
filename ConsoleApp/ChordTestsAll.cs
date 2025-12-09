using RootFinderLib.Models;
using System;
using System.Collections.Generic;

namespace ConsoleApp.Tests
{
    public static class ChordTestsAll
    {
        public static List<ChordTestResult> RunAll()
        {
            string[] testFiles =
            {
                "test_multi_root.xml", "test_one_root.xml", "test_no_roots.xml", "test_multi_root_3r.xml"
            };

            Console.WriteLine("\n=== Running XML Tests ==================================================================================\n");

            var results = new List<ChordTestResult>();

            foreach (var file in testFiles)
            {
                results.Add(ChordTests.RunSingle(file));
            }

            Console.WriteLine("\n=== XML testing completed ==============================================================================\n\n");

            return results;
        }
    }
}
