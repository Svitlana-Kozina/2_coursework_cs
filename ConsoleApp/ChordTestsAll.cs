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
                "test1.xml", "test2.xml", "test3.xml",
                "test4.xml", "test5.xml", "test6.xml",
                "testMultiRoot.xml", "testNoRoots.xml"
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
