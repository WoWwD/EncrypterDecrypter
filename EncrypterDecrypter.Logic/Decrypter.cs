using EncoderDecoder.Logic.Model;
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
            var result = DeserializeKey(PathKey);
            if (result == false)
            {
                return false;
            }
            string[] str = Text.Split();
            for (int s = 0; s < str.Length; s++)
            {
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
        private bool DeserializeKey(string Path)
        {
            try
            {
                var fm = new BinaryFormatter();
                using (FileStream fs = new FileStream(Path, FileMode.Open))
                {
                    ArrayOfSymb = (char[])fm.Deserialize(fs);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string GetDecryptedText()
        {
            return DecryptedText;
        }
    }
}
