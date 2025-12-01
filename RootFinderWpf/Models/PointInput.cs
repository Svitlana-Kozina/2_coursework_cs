using System;                   // Підключаємо базові типи .NET

namespace RootFinderWpf.Models
{
    // Клас для одного вузла g(x) у таблиці (DataGrid)
    public class PointInput
    {
        public double X { get; set; }   // Значення X точки
        public double Y { get; set; }   // Значення Y точки
    }
}
