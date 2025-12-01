using System;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using RootFinderLib.Models;

namespace RootFinderLib.Services
{
    /// <summary>
    /// Сервіс для роботи з XML-файлами, що містять дані для задачі
    /// пошуку коренів методом хорд.
    /// Забезпечує зчитування даних у форматі <see cref="DataSet"/>
    /// та збереження модифікованих даних назад у XML.
    /// </summary>
    public static class XmlDataService
    {
        /// <summary>
        /// Завантажує дані з XML-файлу та перетворює їх у структуру <see cref="DataSet"/>.
        /// </summary>
        /// <param name="path">Шлях до XML-файлу.</param>
        /// <returns>Об’єкт <see cref="DataSet"/> із параметрами задачі.</returns>
        /// <exception cref="FileNotFoundException">Файл не знайдено.</exception>
        /// <exception cref="Exception">Некоректні дані у XML-файлі.</exception>
        /// <remarks>        
        /// Коефіцієнти <c>Fx</c> та точки <c>Gx</c> впорядковуються за атрибутом <c>Index</c>.
        /// </remarks>
        public static DataSet Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("XML file not found.");

            XDocument doc = XDocument.Load(path);

            var data = new DataSet();

            // ==== Зчитування FxString (текстового вигляду полінома) ====
            var fxStringElement = doc.Root.Element("FxString");
            data.FxString = fxStringElement != null ? fxStringElement.Value : null;

            // ==== Зчитування коефіцієнтів f(x) ====
            data.FxCoefficients = doc.Root.Element("Fx")
                .Elements("Coeff")
                .Select(e => new
                {
                    Value = double.Parse(e.Attribute("Value").Value),
                    Index = int.Parse(e.Attribute("Index").Value)
                })
                .OrderBy(x => x.Index)
                .Select(x => x.Value)
                .ToList();

            // ==== Зчитування точок для g(x) ====
            data.GxPoints = doc.Root.Element("Gx")
                .Elements("Point")
                .Select(p => new
                {
                    X = double.Parse(p.Attribute("X").Value),
                    Y = double.Parse(p.Attribute("Y").Value),
                    Index = int.Parse(p.Attribute("Index").Value)
                })
                .OrderBy(p => p.Index)
                .Select(p => (p.X, p.Y))
                .ToList();

            // ==== Зчитування параметрів інтервалу ====
            data.X0 = double.Parse(doc.Root.Element("X0").Value);
            data.X1 = double.Parse(doc.Root.Element("X1").Value);
            data.Epsilon = double.Parse(doc.Root.Element("Epsilon").Value);

            // Додатковий вивід для зручності демонстрації роботи
            //Console.WriteLine($"f(x) = {string.Join(", ", data.FxCoefficients)}");
            //Console.WriteLine($"x0 = {data.X0}, x1 = {data.X1}, eps = {data.Epsilon}");

            // Валідація структури даних
            data.Validate();

            return data;
        }


        /// <summary>
        /// Зберігає набір даних <see cref="DataSet"/> у XML-файл.
        /// </summary>
        /// <param name="data">Об’єкт, що містить коефіцієнти та параметри задачі.</param>
        /// <param name="path">Шлях до вихідного XML-файлу.</param>
        /// <remarks>
        /// Файл формується у тому ж форматі, що й вхідні файли тестів:
        /// обов'язково додаються атрибути <c>Index</c> для збереження порядку.
        /// </remarks>
        public static void Save(DataSet data, string path)
        {
            var doc = new XDocument(
                new XElement("DataSet",

                    new XElement("Fx",
                        data.FxCoefficients.Select((c, i) =>
                            new XElement("Coeff",
                                new XAttribute("Value", c),
                                new XAttribute("Index", i)
                            )
                        )
                    ),

                    new XElement("Gx",
                        data.GxPoints.Select((p, i) =>
                            new XElement("Point",
                                new XAttribute("X", p.X),
                                new XAttribute("Y", p.Y),
                                new XAttribute("Index", i)
                            )
                        )
                    ),

                    new XElement("X0", data.X0),
                    new XElement("X1", data.X1),
                    new XElement("Epsilon", data.Epsilon)
                )
            );

            doc.Save(path);
        }
    }
}
