using System;                   // Базові типи

namespace RootFinderWpf.Models
{
    // Клас для представлення кореня у списку результатів
    public class RootViewItem
    {
        public int Index { get; set; }      // Номер кореня (1,2,3,...)
        public double Value { get; set; }   // Значення кореня x
    }
}
