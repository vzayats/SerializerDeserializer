using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SerializerDeserializer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _inputFolder;
        private string _inputFile;
        private string _outputFolder;
        private string _outputFileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        //Select input folder for serialization
        private void buttonSelectInputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowDialog();

                if (dialog.SelectedPath != null)
                {
                    _inputFolder = dialog.SelectedPath;
                    textBlockSelectedFolder.Text = dialog.SelectedPath;
                }
            }
        }

        //Save serealized .dat file
        private void buttonSaveFilePath_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = "data";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Filter = @"Data files (*.dat)|*.dat";

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _outputFileName = saveFileDialog.FileName;
                    textBlockFileName.Text = saveFileDialog.FileName;
                }
            }
        }

        //Open .dat file
        private void buttonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"Data files (*.dat)|*.dat|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.ShowDialog();

                if (openFileDialog.FileName != null)
                {
                    try
                    {
                        _inputFile = openFileDialog.FileName;
                        textBlockInputFile.Text = openFileDialog.SafeFileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
        }

        //Select output folder for deserialize
        private void SelectOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowDialog();

                if (dialog.SelectedPath != null)
                {
                    _outputFolder = dialog.SelectedPath;
                    textBlockOutputFolder.Text = dialog.SelectedPath;
                }
            }
        }

        //Serialize
        private void buttonSerialize_Click(object sender, RoutedEventArgs e)
        {
            if (_inputFolder != null)
            {
                var folders = Directory.GetDirectories(_inputFolder, "*", SearchOption.AllDirectories).Select(Path.GetFullPath).ToArray();
                var files = Directory.GetFiles(_inputFolder, "*", SearchOption.AllDirectories).Select(Path.GetFullPath).ToArray();

                var fold = new Folder(folders, files);

                if (_outputFileName != null)
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(_outputFileName, FileMode.Create))
                    {
                        try
                        {
                            binaryFormatter.Serialize(fs, fold);
                            MessageBox.Show("Folder was successfully serialized!", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    //If file for saving not selected
                    var result = MessageBox.Show(@"You didn't select a file for saving! Please select a file.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.OK)
                    {
                        //Open new .dat file from disk
                        buttonSaveFilePath_Click(sender, e);
                    }
                }
            }
        }

        //Deserialize file
        private void buttonDeserialize_Click(object sender, RoutedEventArgs e)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            if (_inputFile != null)
            {
                using (FileStream fs = new FileStream(_inputFile, FileMode.OpenOrCreate))
                {
                    try
                    {
                        Folder folder = (Folder)binaryFormatter.Deserialize(fs);
                        folder.Extract(_outputFolder);

                        MessageBox.Show("Folder was successfully deserialized!", "Info", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                        //Open output folder in explorer
                        Process.Start("explorer.exe", _outputFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        //Close application
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}