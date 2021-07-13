using EncoderDecoder.Logic.Controller;
using EncoderDecoder.Logic.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EncrypterDecrypter.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Guid guid = Guid.NewGuid();
        private char[] ArrayOfSymb = new char[ArrayOfSymbols.symbols.Length];
        private string PathKey, PathFile, Text, Path;
        private int size = 25;
        private BackgroundWorker bw;
        private Encrypter enc;
        private Decrypter dec;
        public MainWindow()
        {
            InitializeComponent();
            #region Кнопки
            buttonDecrypting.Click += ButtonDecrypting_Click;
            buttonEncrypting.Click += ButtonEncrypting_Click;
            buttonInsertTextFromFile.Click += InsertTextFromFile_Click;
            #endregion
            buttonDecrypting.IsEnabled = false;
            progressbar.Visibility = Visibility.Collapsed;
            labelProgressbar.Visibility = Visibility.Collapsed;
        }
        private void InsertTextFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var OpenFile = new Microsoft.Win32.OpenFileDialog();
                OpenFile.Filter = "Documents (*.txt, *.docx, *.doc, *.rtf)|*.txt;*.docx;*.doc;*.rtf";
                if (OpenFile.ShowDialog() == true)
                {
                    Textbox.Text = "";
                    Text = "";
                    if (OpenFile.FileName.Remove(0, OpenFile.FileName.Length - 4).Contains(".txt"))
                    {
                        labelProgressbar.Visibility = Visibility.Visible;
                        labelProgressbar.Content = "Loading";
                        bw = new BackgroundWorker();
                        bw.WorkerReportsProgress = true;
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork_LoadTextFromTxtFile);
                        bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged_LoadTextFromTxtFile);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted_LoadTextFromTxtFile);
                        PathFile = OpenFile.FileName;
                        bw.RunWorkerAsync();
                    }
                    if (OpenFile.FileName.Remove(0, OpenFile.FileName.Length - 4).Contains(".doc") || OpenFile.FileName.Remove(0, OpenFile.FileName.Length - 5).Contains(".docx") ||
                        OpenFile.FileName.Remove(0, OpenFile.FileName.Length - 4).Contains(".rtf"))
                    {
                        labelProgressbar.Visibility = Visibility.Visible;
                        labelProgressbar.Content = "Loading";
                        bw = new BackgroundWorker();
                        bw.WorkerReportsProgress = true;
                        bw.DoWork += new DoWorkEventHandler(bw_DoWork_LoadTextFromWord);
                        bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged_LoadTextFromWord);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted_LoadTextFromWord);
                        PathFile = OpenFile.FileName;
                        bw.RunWorkerAsync();
                    }
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
                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += new DoWorkEventHandler(bw_DoWork_EncryptingText);
                bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged_EncryptingText);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted_EncryptingText);
                bw.RunWorkerAsync();
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
                bw = new BackgroundWorker();
                bw.WorkerReportsProgress = true;
                bw.DoWork += new DoWorkEventHandler(bw_DoWork_DecryptingText);
                bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged_DecryptingText);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted_DecryptingText);
                bw.RunWorkerAsync();
            }
        }
        public static object GetGuid()
        {
            return guid = Guid.NewGuid();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var OpenFile = new Microsoft.Win32.OpenFileDialog();
                OpenFile.DefaultExt = "*.bin";
                OpenFile.Filter = "Documents (*.bin)|*.bin";
                if (OpenFile.ShowDialog() == true)
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

        #region BackgroundWorker
        #region BackgroundWorker for LoadTextFromTxtFile
        private void bw_DoWork_LoadTextFromTxtFile(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Visible;
            });
            Thread.Sleep(100);
            try
            {
                using (StreamReader sr = new StreamReader(PathFile))
                {
                    Text = sr.ReadToEnd();
                    //Stream baseStream = sr.BaseStream;
                    //string line;
                    //while ((line = sr.ReadLine()) != null)
                    //{
                    //    Text += line;
                    //    ((BackgroundWorker)sender).ReportProgress(Convert.ToInt32(baseStream.Position));
                    //    Thread.Sleep(30);
                    //}
                }
                for (int i = 0; i <= size; i++)
                {
                    ((BackgroundWorker)sender).ReportProgress(i);
                      Thread.Sleep(30);
                }
                Dispatcher.Invoke(() =>
                {
                    Textbox.Text = Text;
                    progressbar.Visibility = Visibility.Collapsed;
                    labelProgressbar.Visibility = Visibility.Collapsed;
                });
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки текста!");
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    progressbar.Visibility = Visibility.Collapsed;
                    labelProgressbar.Visibility = Visibility.Collapsed;
                });
            }
        }
        private void bw_RunWorkerCompleted_LoadTextFromTxtFile(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Collapsed;
                labelProgressbar.Visibility = Visibility.Collapsed;
            });
        }
        private void bw_ProgressChanged_LoadTextFromTxtFile(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            progressbar.Maximum = size;
        }
        #endregion

        #region BackgroundWorker for LoadTextFromWord
        private void bw_RunWorkerCompleted_LoadTextFromWord(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Collapsed;
                labelProgressbar.Visibility = Visibility.Collapsed;
            });
        }
        private void bw_ProgressChanged_LoadTextFromWord(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            progressbar.Maximum = size;
        }
        private void bw_DoWork_LoadTextFromWord(object sender, DoWorkEventArgs e)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var wordDoc = wordApp.Documents.Open(PathFile);
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Visible;
            });
            Thread.Sleep(100);
            try
            {
                Text = wordDoc.Content.Text;
                for (int i = 0; i <= size; i++)
                {
                    //Text += " \r\n " + wordDoc.Paragraphs[i + 1].Range.Text;
                    ((BackgroundWorker)sender).ReportProgress(i);
                    Thread.Sleep(30);
                }
                Dispatcher.Invoke(() =>
                {
                    Textbox.Text = Text;
                });
            }
            catch
            {
                MessageBox.Show("Ошибка загрузки текста!");
            }
            finally
            {
                wordDoc.Close();
                wordApp.Quit();
                Dispatcher.Invoke(() =>
                {
                    progressbar.Visibility = Visibility.Collapsed;
                    labelProgressbar.Visibility = Visibility.Collapsed;
                });
            }
        }
        #endregion

        #region BackgroundWorker for EncryptingText
        private void bw_RunWorkerCompleted_EncryptingText(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Collapsed;
                labelProgressbar.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Создан текстовый файл и ключ с названием {Path}");
            });
        }
        private void bw_ProgressChanged_EncryptingText(object sender, ProgressChangedEventArgs e)
        {
            //progressbar.Value = e.ProgressPercentage;
            //progressbar.Maximum = size;
        }
        private void bw_DoWork_EncryptingText(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                labelProgressbar.Content = "Please, wait";
                //progressbar.Visibility = Visibility.Visible;
                labelProgressbar.Visibility = Visibility.Visible;
                enc = new Encrypter(Textbox.Text, $"{Path}.bin", $"{Path}.txt");
            }); 
            try
            {
                var result = enc.Encrypting();
                //size = enc.masOfText.Length;
                Dispatcher.Invoke(() =>
                {
                    Textbox.Text = enc.GetEncodedText();
                });
            }
            catch
            {
                MessageBox.Show("Ошибка шифрования!");
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    //progressbar.Visibility = Visibility.Collapsed;
                    labelProgressbar.Visibility = Visibility.Collapsed;
                });
            }
        }
        #endregion

        #region BackgroundWorker for DecryptingText
        private void bw_RunWorkerCompleted_DecryptingText(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressbar.Visibility = Visibility.Collapsed;
                labelProgressbar.Visibility = Visibility.Collapsed;
            });
        }
        private void bw_ProgressChanged_DecryptingText(object sender, ProgressChangedEventArgs e)
        {
            //progressbar.Value = e.ProgressPercentage;
            //progressbar.Maximum = size;
        }
        private void bw_DoWork_DecryptingText(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                labelProgressbar.Content = "Please, wait";
                //progressbar.Visibility = Visibility.Visible;
                labelProgressbar.Visibility = Visibility.Visible;
                dec = new Decrypter(Textbox.Text, ArrayOfSymb, PathKey);
            });
            try
            {
                var result = dec.Decrypting();
                //size = enc.masOfText.Length;
                Dispatcher.Invoke(() =>
                {
                    Textbox.Text = dec.GetDecryptedText();
                });
            }
            catch
            {
                MessageBox.Show("Ключ не подходит");
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    //progressbar.Visibility = Visibility.Collapsed;
                    labelProgressbar.Visibility = Visibility.Collapsed;
                });
            }
        }
        #endregion

        #endregion

        private void buttonSaveTextInFile_Click(object sender, RoutedEventArgs e)
        {
            if (Textbox.Text == null || Textbox.Text == " " || Textbox.Text == "")
            {
                MessageBox.Show("Нет текста!");
            }
            else
            {
                var SaveFile = new Microsoft.Win32.SaveFileDialog();
                SaveFile.Filter = "Обычный текст (*.txt)|*.txt;|Текст в формате RTF (*.rtf)|*.rtf"; // Документ Word (*.docx)|*.docx;|Документ Word 97-2003 (*.doc)|*.doc
                if (SaveFile.ShowDialog() == true)
                {
                    File.WriteAllText(SaveFile.FileName, Textbox.Text);
                }
            }
        }
    }
}