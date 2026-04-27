using System;
using System.Text;
using System.Linq;

namespace MultiCipherApp
{
    interface ICipher
    {
        string Encrypt(string input);
        string Decrypt(string input);
    }

    // --- SZYFR CEZARA ---
    class CaesarCipher : ICipher
    {
        private int shift;

        public CaesarCipher(int shift)
        {
            this.shift = shift;
        }

        public string Encrypt(string input) => Process(input, shift);
        public string Decrypt(string input) => Process(input, -shift);

        private string Process(string text, int shift)
        {
            StringBuilder result = new StringBuilder();

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    char encrypted = (char)((((c - offset) + shift + 26) % 26) + offset);
                    result.Append(encrypted);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }

    // --- SZYFR VIGENERE ---
    class VigenereCipher : ICipher
    {
        private string key;

        public VigenereCipher(string key)
        {
            this.key = key;
        }

        public string Encrypt(string input) => Process(input, true);
        public string Decrypt(string input) => Process(input, false);

        private string Process(string text, bool encrypt)
        {
            StringBuilder result = new StringBuilder();
            int keyIndex = 0;

            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    int keyShift = char.ToLower(key[keyIndex % key.Length]) - 'a';

                    if (!encrypt)
                        keyShift = -keyShift;

                    char encrypted = (char)((((c - offset) + keyShift + 26) % 26) + offset);
                    result.Append(encrypted);

                    keyIndex++;
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }

    // --- SZYFR PRZESTAWIENIOWY KOLUMNOWY ---
    class ColumnarTranspositionCipher : ICipher
    {
        private string key;

        public ColumnarTranspositionCipher(string key)
        {
            this.key = key;
        }

        public string Encrypt(string input)
        {
            int columns = key.Length;
            int rows = (int)Math.Ceiling((double)input.Length / columns);

            char[,] grid = new char[rows, columns];
            int index = 0;

            // wypełnianie tabeli
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    if (index < input.Length)
                        grid[r, c] = input[index++];
                    else
                        grid[r, c] = 'X'; // padding
                }
            }

            // sortowanie kolumn wg klucza
            var order = key
                .Select((ch, i) => new { ch, i })
                .OrderBy(x => x.ch)
                .ToList();

            StringBuilder result = new StringBuilder();

            foreach (var item in order)
            {
                for (int r = 0; r < rows; r++)
                {
                    result.Append(grid[r, item.i]);
                }
            }

            return result.ToString();
        }

        public string Decrypt(string input)
        {
            int columns = key.Length;
            int rows = input.Length / columns;

            char[,] grid = new char[rows, columns];

            var order = key
                .Select((ch, i) => new { ch, i })
                .OrderBy(x => x.ch)
                .ToList();

            int index = 0;

            foreach (var item in order)
            {
                for (int r = 0; r < rows; r++)
                {
                    grid[r, item.i] = input[index++];
                }
            }

            StringBuilder result = new StringBuilder();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    result.Append(grid[r, c]);
                }
            }

            return result.ToString().TrimEnd('X');
        }
    }

    // --- PROGRAM GŁÓWNY ---
    class Program
    {
        static void Main()
        {
            Console.WriteLine("1 - Szyfrowanie");
            Console.WriteLine("2 - Odszyfrowanie");
            Console.Write("Wybierz opcję: ");
            int choice = int.Parse(Console.ReadLine());

            Console.Write("Podaj tekst: ");
            string text = Console.ReadLine();

            Console.Write("Klucz Cezara (liczba): ");
            int caesarKey = int.Parse(Console.ReadLine());

            Console.Write("Klucz Vigenere (tekst): ");
            string vigenereKey = Console.ReadLine();

            Console.Write("Klucz przestawieniowy (tekst): ");
            string transpositionKey = Console.ReadLine();

            ICipher caesar = new CaesarCipher(caesarKey);
            ICipher vigenere = new VigenereCipher(vigenereKey);
            ICipher transposition = new ColumnarTranspositionCipher(transpositionKey);

            if (choice == 1)
            {
                string step1 = caesar.Encrypt(text);
                string step2 = vigenere.Encrypt(step1);
                string finalResult = transposition.Encrypt(step2);

                Console.WriteLine("\nZaszyfrowany tekst:");
                Console.WriteLine(finalResult);
            }
            else if (choice == 2)
            {
                string step1 = transposition.Decrypt(text);
                string step2 = vigenere.Decrypt(step1);
                string finalResult = caesar.Decrypt(step2);

                Console.WriteLine("\nOdszyfrowany tekst:");
                Console.WriteLine(finalResult);
            }
            else
            {
                Console.WriteLine("Niepoprawny wybór.");
            }
        }
    }
}