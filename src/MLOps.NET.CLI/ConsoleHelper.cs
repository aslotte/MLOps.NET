using MLOps.NET.Entities.Impl;
using System;
using System.Collections.Generic;

namespace MLOps.NET.CLI
{
    internal class ConsoleHelper
    {
        internal static void Write<T>(List<T> list) where T:class
        {
            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}