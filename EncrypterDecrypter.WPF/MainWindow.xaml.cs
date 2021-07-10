using EncoderDecoder.Logic.Controller;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EncrypterDecrypter.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Guid guid = Guid.NewGuid();
        private string Path;
        private char[] ArrayOfSymb = new char[163];
        private string PathKey;
        public MainWindow()
        {
            InitializeComponent();
            #region Кнопки
            buttonDecrypting.Click += ButtonDecrypting_Click;
            buttonEncrypting.Click += ButtonEncrypting_Click;
            buttonInsertTextFromFile.Click += InsertTextFromFile_Click;
            #endregion
            buttonDecrypting.IsEnabled = false;
        }
        private void InsertTextFromFile_Click(object sender, RoutedEventArgs e)
        {
            var result = OpenFileDialogResult("*.txt");
            if (result != null)
            {
                Textbox.Text = "";
                Textbox.Text = File.ReadAllText(result);
            }
            //else
            //{
            //    MessageBox.Show("Неверный формат файла!");
            //}
        }
        private void ButtonEncrypting_Click(object sender, RoutedEventArgs e)
        {
            Path = GetGuid().ToString();
            if (Textbox.Text == null || Textbox.Text == " " || Textbox.Text == "")
            {
                MessageBox.Show("Нет текста!");
            }
            else
            {
                var encr = new Encrypter(Textbox.Text, $"{Path}.bin", $"{Path}.txt");
                var result = encr.Encrypting();
                if (result != false)
                {
                    Textbox.Text = encr.GetEncodedText();
                    MessageBox.Show($"Создан текстовый файл и ключ с названием {Path}");
                }
                else
                {
                    MessageBox.Show("Ошибка шифрования!");
                }
            }
        }
        private void ButtonDecrypting_Click(object sender, RoutedEventArgs e)
        {
            if (Textbox.Text == null || Textbox.Text == " " || Textbox.Text == "")
            {
                MessageBox.Show("Зашифрованный текст отсутствует!");
            }
            else
            {
                var decr = new Decrypter(Textbox.Text, ArrayOfSymb, PathKey);
                var result = decr.Decrypting();
                if (result != false)
                {
                    Textbox.Text = decr.GetDecryptedText();
                }
                else
                {
                    MessageBox.Show("Ключ не подходит");
                }
            }
        }
        public static object GetGuid()
        {
            return guid = Guid.NewGuid();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var result = OpenFileDialogResult("*.bin");
            if (result != null)
            {
                var fm = new BinaryFormatter();
                using (FileStream fs = new FileStream(result, FileMode.Open))
                {
                    ArrayOfSymb = (char[])fm.Deserialize(fs);
                }
                textboxKey.Text = result.Remove(0, result.Length - 40);
                PathKey = result;
                buttonEncrypting.IsEnabled = false;
                buttonDecrypting.IsEnabled = true;
            }
        }
        private string OpenFileDialogResult(string TypeFile)
        {
            var OpenFile = new Microsoft.Win32.OpenFileDialog();
            OpenFile.DefaultExt = TypeFile;
            OpenFile.Filter = $"documents ({TypeFile})|{TypeFile}";
            if (OpenFile.ShowDialog() == true)
            {
                return OpenFile.FileName;
            }
            return null;
        }
        private void buttonClearTextboxKey_Click(object sender, RoutedEventArgs e)
        {
            textboxKey.Text = "";
            buttonEncrypting.IsEnabled = true;
            buttonDecrypting.IsEnabled = false;
        }
    }
}
