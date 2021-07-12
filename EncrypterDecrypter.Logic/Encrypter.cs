using EncoderDecoder.Logic.Model;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EncoderDecoder.Logic.Controller
{
    public class Encrypter
    {
        private string Text { get; }
        private string PathKey { get; }
        private string PathEncodedText { get; }
        private string EncodedText;
        private char[] ArrayOfsymb;
        public char [] masOfText;
        public Encrypter(string Text, string PathKey, string PathEncodedText)
        {
            this.Text = Text;
            this.PathKey = PathKey;
            this.PathEncodedText = PathEncodedText;
            ArrayOfsymb = new char[ArrayOfSymbols.symbols.Length];
        }
        public bool Encrypting()
        {
            try
            {
                // Заполнение пустого массива символами
                for (int i = 0; i < ArrayOfSymbols.symbols.Length; i++)
                {
                    ArrayOfsymb[i] = ArrayOfSymbols.symbols[i];
                }
                Random rnd = new Random();
                // задание элементам массива рандомных индексов
                for (int i = ArrayOfsymb.Length - 1; i >= 1; i--)
                {
                    int j = rnd.Next(i + 1);
                    var temp = ArrayOfsymb[j];
                    ArrayOfsymb[j] = ArrayOfsymb[i];
                    ArrayOfsymb[i] = temp;
                }
                masOfText = Text.ToCharArray(); // Разбиение введённой строки на символы
                // подставление индексов массива вместо букв и символов в введённой строке
                for (int s = 0; s < masOfText.Length; s++)
                {
                    for (int j = 0; j < ArrayOfsymb.Length; j++)
                    {
                        if (masOfText[s] == ArrayOfsymb[j])
                        {
                            EncodedText = EncodedText + " " + j.ToString();
                            break;
                        }
                    }
                }
                SaveEncodedText();
                SerializeKey(ArrayOfsymb, PathKey);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void SerializeKey(char[] arr, string Path)
        {
            var fm = new BinaryFormatter();
            using (var f = new FileStream(Path, FileMode.OpenOrCreate))
            {
                fm.Serialize(f, arr);
            }
        }
        public void SaveEncodedText()
        {
            using (StreamWriter sw = new StreamWriter(PathEncodedText, false))
            {
                sw.Write(EncodedText);
            }
        }
        public string GetEncodedText()
        {
            return EncodedText;
        }
    }
}