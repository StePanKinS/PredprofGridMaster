using GridMasterPredprof;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PredprofInterpreter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class DBWindow : Window
    {

        DB dataBase;
        TextBlock? selectedName;

        public DBWindow()
        {
            InitializeComponent();

            dataBase = new DB();

            Closed += DBWindow_Closed;

            FillNamesList();
        }

        private void DBWindow_Closed(object? sender, EventArgs e)
        {
            dataBase.SDServer();
        }

        private void FillNamesList()
        {
            string[] strings = dataBase.GetNames();

            namesList.Children.Clear();
            for (int i = 0; i < strings.Length; i++)
            {
                TextBlock textBlock = new()
                {
                    Text = strings[i],
                    Padding = new Thickness(5),
                };

                int index = i;
                textBlock.MouseLeftButtonDown += (s, e) =>
                {
                    FillCodeBlock(strings[index]);

                    if(selectedName != null)
                    {
                        selectedName.Background = new SolidColorBrush(Colors.White);
                    }

                    textBlock.Background = new SolidColorBrush(Colors.LightGray);
                    selectedName = textBlock;
                };

                namesList.Children.Add(textBlock);
            }
        }

        private void FillCodeBlock(string name)
        {
            codeBlock.Text = dataBase.GetCode(name);
        }

        private void LoadToEditor(object sender, RoutedEventArgs e)
        {
            MainWindow mw = (MainWindow)Owner;
            mw.textEditor.Text = codeBlock.Text;
            mw.OpenedFile = null;

            Close();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new();
            sfd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (sfd.ShowDialog() == true)
            {
                using StreamWriter sw = new(sfd.FileName);
                sw.Write(codeBlock.Text);
            }
        }

        private void SetProgram(string name, string code)
        {
            if(name == "")
            {
                FillNamesList();
                return;
            }

            Interpreter interpreter = new Interpreter(code, isSteppingMode: false);
            interpreter.Start();

            string? result = "";
            try
            {
                var points = interpreter.GetTrajectory();
                foreach (var point in points)
                {
                    result += $"{point.x},{point.y};";
                }
            }
            catch (CodeException)
            {
                result = null;
            }

            if (!dataBase.SetProgram(name, code, result))
                dataBase.UpdateProgram(name, code, result);

            FillNamesList();
        }

        private void LoadFromFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if(ofd.ShowDialog() == true)
            {
                using StreamReader sr = new(ofd.FileName);
                string code = sr.ReadToEnd();
                
                SetProgram(ofd.SafeFileName, code);
            }
        }

        private void LoadFromEditor(object sender, RoutedEventArgs e)
        {
            TextBox textBox = new();
            namesList.Children.Add(textBox);
            textBox.LostFocus += (a, b) => SetProgram(textBox.Text, ((MainWindow)Owner).textEditor.Text);
            textBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                    SetProgram(textBox.Text, ((MainWindow)Owner).textEditor.Text);
            };

            textBox.Focus();
        }

        private void DeleteSelected(object sender, RoutedEventArgs e)
        {
            if (selectedName == null) 
                return;

            dataBase.RemoveProgram(selectedName.Text);
            FillNamesList();
        }
    }
}
