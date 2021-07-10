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
            var OpenFile = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> Success = OpenFile.ShowDialog();
            OpenFile.DefaultExt = ".txt";
            OpenFile.Filter = "Text documents (.txt)|*.txt";
            try
            {
                if (Success.HasValue && Success.Value)
                {
                    Textbox.Text = "";
                    Textbox.Text = File.ReadAllText(OpenFile.FileName);
                }
            }
            catch
            {
                MessageBox.Show("Неверный формат файла!");
            }
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
                if (result == true)
                {
                    Textbox.Text = "";
                    //Textbox.Text = encr.GetEncodedText();
                    MessageBox.Show($"Создан текстовый файл и ключ с названием {Path}");
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
                if (result == true)
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
            var OpenFile = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> Success = OpenFile.ShowDialog();
            OpenFile.DefaultExt = ".bin";
            OpenFile.Filter = "Text documents (.bin)|*.bin";
            try
            {
                if (Success.HasValue && Success.Value)
                {
                    var fm = new BinaryFormatter();
                    using (FileStream fs = new FileStream(OpenFile.FileName, FileMode.Open))
                    {
                        ArrayOfSymb = (char[])fm.Deserialize(fs);
                    }
                    textboxKey.Text = OpenFile.SafeFileName;
                    PathKey = OpenFile.FileName;
                    buttonEncrypting.IsEnabled = false;
                    buttonDecrypting.IsEnabled = true;
                }
            }
            catch
            {
                MessageBox.Show("Неверный формат ключа!");
            }
        }
        private void buttonClearTextboxKey_Click(object sender, RoutedEventArgs e)
        {
            textboxKey.Text = "";
            buttonEncrypting.IsEnabled = true;
            buttonDecrypting.IsEnabled = false;
        }
    }
}
