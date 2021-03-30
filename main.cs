using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// C3  = 0 => StringBuilder
// C17 = 0 => Знайти найбільшу кількість речень заданого тексту, в яких є однакові слова.

namespace Lab_3
{
    class Program
    {
        static StringBuilder getNextSentence(ref StringBuilder strb)
        {
            StringBuilder res = new StringBuilder();
            for(int i = 0; i < strb.Length; i++)
            {
                char curEl = strb[i];
                if (curEl == '.' || curEl == '!' || curEl == '?')
                {
                    strb.Remove(0, i+1);
                    res.Append(curEl); 
                    break;
                }
                else
                {
                    res.Append(curEl);
                }
            }

            return res;
        }

        static void countWords(StringBuilder str, ref Dictionary<string, int> outDict)
        {
            Dictionary<string, int> uniqueWords = new Dictionary<string, int>();

            StringBuilder activeWord = new StringBuilder();

            while (true) // может true заменить на что-нибудь вменяемое?
            {
                activeWord = getNextWord(ref str);

                if (Convert.ToString(activeWord) == "")
                {
                    break;
                }
                else
                {
                    try                                         // оптимизация кода - не, не слышали...
                    {
                        uniqueWords[Convert.ToString(activeWord)] = 1;
                    }
                    catch (KeyNotFoundException)
                    {
                        uniqueWords.Add(Convert.ToString(activeWord), 1);
                    }
                }
            }

            foreach (KeyValuePair<string, int> kvp in uniqueWords)
            {
                try
                {
                    outDict[kvp.Key]++;
                }
                catch (KeyNotFoundException)
                {
                    outDict.Add(kvp.Key, 1);
                }
            }
        }

        static StringBuilder getNextWord(ref StringBuilder str) // Works well(enough)
        {
            StringBuilder word = new StringBuilder();
            string terminators = " , . / \\ @ ! ? : \n \r \t | ; [] - \" \' \u201c \u201d"; // maybe better add more spesial symbols

            bool wordStarted = false;
            for( int i = 0; i < str.Length; i++)
            {
                char curChar = str[i];
                curChar = Char.ToLower(curChar);
                bool charIsTerminator = terminators.Contains(curChar);

                if (charIsTerminator && wordStarted == false)
                {
                    continue;
                }
                else if(charIsTerminator && wordStarted == true)
                {
                    str.Remove(0, i + 1);
                    break;
                }
                else
                {
                    wordStarted = true;
                    word.Append(curChar);
                }
            }

            return word;
        }

        static void Main(string[] args)
        {
            const string filePath = @"E:\Additional\C#\Lab_3\A farewell to arms.txt"; //Полный путь к файлу из которого считываем 
            //const string filePath = @"E:\Additional\C#\Lab_3\sample3.txt";
            StreamReader sr = new StreamReader(filePath, System.Text.Encoding.Default);

            Dictionary<string, int> wordsFreq = new Dictionary<string, int>();
            StringBuilder text;
            
            try
            {
                text = new StringBuilder();

                using (sr)
                {
                    text.Append(sr.ReadToEnd());
                }

                StringBuilder sentence = new StringBuilder(" ");
                int sentenceCounter = 0;

                Console.Write("Working on sentence: ");
                while (true)
                {
                    sentence = getNextSentence(ref text);
                    Console.SetCursorPosition(21, Console.CursorTop);
                    Console.Write($"{sentenceCounter}");

                    if (Convert.ToString(sentence) == "")
                    {
                        break;
                    }
                    else
                    {
                        sentenceCounter++;
                        countWords(sentence, ref wordsFreq);
                    }
                }

                bool onlyMostPopular = false;
                var wordsFreqSorted = from entry in wordsFreq orderby entry.Value ascending select entry; // Linq - суперсила!

                if(onlyMostPopular == false)
                {
                    float threshold = 0.01F; // Min % to be in the output (min = 0.0 ; max 1.0)
                    int passedCounter = 0;
                    float filter = threshold * sentenceCounter;

                    Console.Write(String.Format("\n{0, 20} == {1, -20:N0}\n", $"Threshold", $"{threshold * 100}%"));
                    Console.Write(String.Format("{0, 20}    {1, -20:N0}\n", "Word", "Frequency"));
                    foreach (KeyValuePair<string, int> kvp in wordsFreqSorted)
                    {
                        if (kvp.Value >= filter)
                        {
                            passedCounter++;
                            Console.WriteLine(String.Format("{0, 20} -- {1, -20:N0}", $"\"{kvp.Key}\"", $"{kvp.Value} / {sentenceCounter}"));
                        }
                    }

                    if(passedCounter == 0)
                    {
                        Console.WriteLine($"None of the words surpassed threshold of {threshold * 100}%.");
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("\n\nMost popular word  is \"{0}\" in {1} sentences of {2}", $"{wordsFreqSorted.Last().Key}", $"{wordsFreqSorted.Last().Value}", $"{sentenceCounter}."));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
