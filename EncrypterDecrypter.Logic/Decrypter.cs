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
                for (int s = 0; s < str.Length; s++)
                {
                    if (str[s] != "")
                    {
                        int LengthEncodedSymb = Convert.ToInt32(str[s]);
                        if (LengthEncodedSymb > LengthArrayOfSymb)
                        {
                            return false;
                        }
                    }
                    for (int j = 0; j < ArrayOfSymb.Length; j++)
                    {
                        if (str[s] == j.ToString())
                        {
                            DecryptedText += ArrayOfSymb[j].ToString();
                            break;
                        }
                    }
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
    }
}