using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace EncoderDecoder.Logic.Controller
{
    public class Decrypter
    {
        private string DecryptedText;
        private string Text { get; }
        private string PathKey { get; }
        private char[] ArrayOfSymb;
        public Decrypter(string Text, char[] Array, string PathKey)
        {
            this.Text = Text;
            ArrayOfSymb = Array;
            this.PathKey = PathKey;
        }
        public bool Decrypting()
        {
            try
            {
                DeserializeKey(PathKey);
                string[] str = Text.Split();
                int LengthArrayOfSymb = Convert.ToInt32(ArrayOfSymb.Length);
                int LengthEncodedSymb;
                for (int s = 0; s < str.Length; s++)
                {
                    if (str[s] == "")
                    {
                        continue;
                    }
                    if (str[s] != "")
                    {
                        LengthEncodedSymb = Convert.ToInt32(str[s]);
                        if (LengthEncodedSymb > LengthArrayOfSymb)
                        {
                            return false;
                        }
                    }
                    DecryptedText += BinarySearch(ArrayOfSymb, str[s], 0, ArrayOfSymb.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
          
        }
        private void DeserializeKey(string Path)
        {
            var fm = new BinaryFormatter();
            using (FileStream fs = new FileStream(Path, FileMode.Open))
            {
                ArrayOfSymb = (char[])fm.Deserialize(fs);
            }
        }
        public string GetDecryptedText()
        {
            return DecryptedText;
        }
        private static string BinarySearch(char[] array, string searchedValue, int left, int right)
        {
            int searchedvalue;
            while (left <= right)
            {
                var middle = (left + right) / 2;
                searchedvalue = Convert.ToInt32(searchedValue);
                if (searchedvalue == middle)
                {
                    return array[middle].ToString();
                }
                if (searchedvalue < middle)
                {
                    right = middle - 1;
                }
                else
                {
                    left = middle + 1;
                }
            }
            return "";
        }
    }
}