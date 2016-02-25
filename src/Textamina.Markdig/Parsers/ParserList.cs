using System.Collections.Generic;

namespace Textamina.Markdig.Parsers
{
    public class ParserList<T> : List<T> where T : class, ICharacterParser
    {
        private T[][] parsersWithOpeningCharacters;
        private T[] globalParsers;

        public T[] GlobalParsers => globalParsers;

        public T[] GetParsersForOpeningCharacter(char openingChar)
        {
            return openingChar < parsersWithOpeningCharacters.Length ? parsersWithOpeningCharacters[openingChar] : null;
        }

        public void Initialize()
        {
            var charCounter = new Dictionary<char, int>();
            int globalCounter = 0;
            int maxChar = 0;
            foreach (var parser in this)
            {
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        if (!charCounter.ContainsKey(openingChar))
                        {
                            charCounter[openingChar] = 0;
                        }
                        charCounter[openingChar]++;
                        if (openingChar > maxChar)
                        {
                            maxChar = openingChar;
                        }
                    }
                }
                else
                {
                    globalCounter++;
                }
            }

            globalParsers = new T[globalCounter];
            parsersWithOpeningCharacters = new T[maxChar+1][];

            foreach (var parser in this)
            {
                if (parser.OpeningCharacters != null && parser.OpeningCharacters.Length != 0)
                {
                    foreach (var openingChar in parser.OpeningCharacters)
                    {
                        if (parsersWithOpeningCharacters[openingChar] == null)
                        {
                            parsersWithOpeningCharacters[openingChar] = new T[charCounter[openingChar]];
                        }
                        var list = parsersWithOpeningCharacters[openingChar];
                        var index = list.Length - charCounter[openingChar];
                        list[index] = parser;
                        charCounter[openingChar]--;
                    }
                }
                else
                {
                    globalParsers[globalParsers.Length - globalCounter] = parser;
                    globalCounter--;
                }
            }
        }
    }
}