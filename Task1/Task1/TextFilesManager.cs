using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class TextFilesManager
    {
        private static Random random = new Random();

        // creates lines of randomly generated
        // parts and writes them to files
        public static void CreateFiles()
        {
            var lines = new string[100000];
            var files = new string[100];
            string randomDate;
            string randomLatin;
            string randomRus;
            string randomPositiveEvenInteger;
            string randomPositiveDouble;

            // creates directory if it doesn't exist
            Directory.CreateDirectory("GeneratedFiles");

            for (var i = 0; i < files.Length; i++)
            {
                for (var j = 0; j < lines.Length; j++)
                {
                    randomDate = getRandomDate();
                    randomLatin = getRandomLatin();
                    randomRus = getRandomRus();
                    randomPositiveEvenInteger = getRandomPositiveEvenInteger();
                    randomPositiveDouble = getRandomPositiveDouble();
                    lines[j] = $"{randomDate}||{randomLatin}||{randomRus}||{randomPositiveEvenInteger}||{randomPositiveDouble}||";
                }
                File.WriteAllLines($@"GeneratedFiles/{i + 1}.txt", lines);
                Console.WriteLine($"File {i + 1} created"); // kind of logging
            }
        }

        // generates random date for the last 5 years
        private static string getRandomDate()
        {
            var startDate = DateTime.Now.AddYears(-5);
            var range = (DateTime.Today - startDate).Days;
            var randomDate = startDate.AddDays(random.Next(range)).ToString("dd.MM.yyyy");
            return randomDate;
        }

        // generates string of random latin letters
        private static string getRandomLatin()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // generates string of random cyrillic letters
        private static string getRandomRus()
        {
            const string chars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // generates string of random positive even int number
        private static string getRandomPositiveEvenInteger()
        {
            return (2 * random.Next(1 / 2, 100000001 / 2)).ToString();
        }

        // generates string of random positive double number
        private static string getRandomPositiveDouble()
        {
            return (random.NextDouble() * (20 - 1) + 1).ToString("0.00000000");
        }

        // merges existing files in one file and deletes rows that contain certain combination of characters
        public static void MergeFilesAndDeleteStringWithCharacterCombination()
        {
            Console.WriteLine("Enter character combination:");
            var combination = Console.ReadLine();

            // create directory if not exist
            var dir = Directory.CreateDirectory("MergedFiles");
            using StreamWriter mergedFile = new(@$"{dir}\mergedFiles.txt");
            var files = Directory.GetFiles("GeneratedFiles");
            var deletedLinesCounter = 0;

            foreach (var file in files)
            {
                foreach (var line in File.ReadLines(file))
                {
                    if (line.Contains(combination))
                    {
                        deletedLinesCounter++;
                    }
                    else
                    {
                        mergedFile.WriteLine(line);
                    }
                    
                }
            }
            Console.WriteLine("Merge successful!");
            Console.WriteLine($"Deleted lines amount: {deletedLinesCounter}");
        }

    }
}
