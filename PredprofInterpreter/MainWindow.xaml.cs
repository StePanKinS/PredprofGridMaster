using GridMasterPredprof;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PredprofInterpreter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            setUpMap();

            Grid.SetColumn(playerBorder, 2);
            Grid.SetRow(playerBorder, 40);
            mapGrid.Children.Add(playerBorder);
            playerBorder.Background = new SolidColorBrush(Colors.Green);
            playerBorder.BorderThickness = new Thickness(0);
            defaultBrush = textEditor.SelectionBrush;
        }

        internal string TextEditorText 
        {
            get => textEditor.Text;
            set => textEditor.Text = value;
        }

        private string? _openedFile = null;
        internal string? OpenedFile
        {
            get => _openedFile;
            set
            {
                if(value == null)
                {
                    Title = "Interpreter";
                    menuSave.IsEnabled = false;
                }
                else
                {
                    Title = "Intterpreter - " + value;
                    menuSave.IsEnabled = true;
                }

                _openedFile = value;
            }
        }

        private PlayerBorder playerBorder = new PlayerBorder();
        private double speed = 15;
        private (int x, int y)[] positions = [(0, 0)];
        private bool animating = false;
        private Brush defaultBrush;

        private void Menu_FileOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "txt files (*.txt)|*.txt";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader fs = new(ofd.FileName))
                    {
                        textEditor.Text = fs.ReadToEnd();
                    }
                    OpenedFile = ofd.FileName;
                }
                catch (Exception)
                {
                    textEditor.Text = "";
                    OpenedFile = null;
                }
            }
        }

        private void Menu_FileSaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new();
            sfd.Filter = "txt files (*.txt)|*.txt";

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using StreamWriter sw = new(sfd.FileName);
                    sw.Write(textEditor.Text);
                    OpenedFile = sfd.FileName;
                }
                catch (Exception)
                {
                    OpenedFile = null;
                }
            }
        }

        private void Menu_FileSave(object sender, RoutedEventArgs e)
        {
            if (OpenedFile != null)
            {
                try
                {
                    using StreamWriter sw = new(OpenedFile);
                    sw.Write(textEditor.Text);
                }
                catch (Exception)
                {
                    OpenedFile = null;  
                }
            }
        }


        private void setUpMap()
        {
            mapGridContainer.SizeChanged += (sender, e) =>
            {
                if (mapGridContainer.ActualWidth > mapGridContainer.ActualHeight)
                {
                    mapGrid.Width = mapGridContainer.ActualHeight;
                    mapGrid.Height = mapGridContainer.ActualHeight;
                }
                else
                {
                    mapGrid.Width = mapGridContainer.ActualWidth;
                    mapGrid.Height = mapGridContainer.ActualWidth;
                }
            };

            mapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            mapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            for (int i = 0; i < 21; i++)
            {
                mapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                mapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                mapGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
                mapGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            }

            SolidColorBrush brush = new SolidColorBrush(Colors.DarkGray);

            for (int i = 0; i < 21; i++)
            {
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = (20 - i).ToString();
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetColumn(textBlock, 0);
                    Grid.SetRow(textBlock, i * 2);

                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = brush;
                    Grid.SetRow(rectangle, i * 2 + 1);
                    Grid.SetColumnSpan(rectangle, 43);

                    mapGrid.Children.Add(rectangle);
                    mapGrid.Children.Add(textBlock);
                }
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = i.ToString();
                    textBlock.VerticalAlignment = VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(textBlock, 43);
                    Grid.SetColumn(textBlock, i * 2 + 2);

                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = brush;
                    Grid.SetRowSpan(rectangle, 43);
                    Grid.SetColumn(rectangle, i * 2 + 1);

                    mapGrid.Children.Add(rectangle);
                    mapGrid.Children.Add(textBlock);
                }
            }
        }

        private void Button_Run(object sender, RoutedEventArgs e)
        {
            playerBorder.Background = new SolidColorBrush(Colors.Green);

            animating = true;

            try
            {
                Interpreter interpreter = new(textEditor.Text, false);
                interpreter.Start();
                positions = interpreter.GetTrajectory();

                Animate(0);
            }
            catch(CodeException exc)
            {
                playerBorder.Background = new SolidColorBrush(Colors.Red);

                int n = 1;
                int start = 0;
                for(int i = 0; i < textEditor.Text.Length; i++)
                {
                    if (n == exc.line)
                    {
                        start = i;
                        break;
                    }

                    if (textEditor.Text[i] == '\n')
                        n++;
                }

                n = 1;
                if (exc.line != 1 && start == 0)
                {
                    for (int i = 0; i < textEditor.Text.Length; i++)
                    {
                        if (n == exc.line - 1)
                        {
                            start = i;
                            break;
                        }

                        if (textEditor.Text[i] == '\n')
                            n++;
                    }
                }

                int length = 0;
                for(int i = start; i < textEditor.Text.Length; i++)
                {
                    if (textEditor.Text[i] == '\n')
                        break;

                    length++;
                }

                

                errorTextBlock.Text = exc.Message;

                textEditor.SelectionBrush = new SolidColorBrush(Colors.Red);
                textEditor.Select(start, length);
                textEditor.SelectionChanged += ResetSelection;
                textEditor.Focus();
            }
        }

        private void ResetSelection(object sender, RoutedEventArgs e)
        {
            textEditor.SelectionBrush = defaultBrush;
            textEditor.SelectionChanged -= ResetSelection;

            errorTextBlock.Text = "";
        }

        private void Animate(int i)
        {
            if (!animating)
                return;

            if (i >= positions.Length || i < 0)
                return;

            Int32Animation animationX = new();
            Int32Animation animationY = new();

            int byX, byY;
            double timeX, timeY;

            if (i == 0)
            {
                byX = positions[0].x - playerBorder.XPosition;
                timeX = 0;

                byY = positions[0].y - playerBorder.YPosition;
                timeY = 0;

                animationY.Completed += (a, b) => Animate(1);
            }
            else
            {
                byX = positions[i].x - positions[i - 1].x;
                timeX = Math.Abs(byX / speed);

                byY = positions[i].y - positions[i - 1].y;
                timeY = Math.Abs(byY / speed);

                (timeX > timeY ? animationX : animationY).Completed += (a, b) => Animate(i + 1);
            }

            animationX.From = playerBorder.XPosition;
            animationX.By = byX;
            animationX.Duration = TimeSpan.FromSeconds(timeX);

            animationY.From = playerBorder.YPosition;
            animationY.By = byY;
            animationY.Duration = TimeSpan.FromSeconds(timeY);

            playerBorder.BeginAnimation(PlayerBorder.XPositionProperty, animationX);
            playerBorder.BeginAnimation(PlayerBorder.YPositionProperty, animationY);
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            animating = false;

            playerBorder.BeginAnimation(PlayerBorder.XPositionProperty, null);
            playerBorder.BeginAnimation(PlayerBorder.YPositionProperty, null);
        }

        private void ManageDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DBWindow dBWindow = new()
                {
                    Owner = this
                };

                dBWindow.ShowDialog();
            }
            catch (MySqlException)
            {
                MessageBox.Show("Cant connect to DataBase");
            }
        }

        private void Result_Save(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new()
            {
                Filter = "Program result (*.result)|*.result"
            };

            if(sfd.ShowDialog() == true)
            {
                using StreamWriter sw = new(sfd.FileName);

                (int, int)[] poses;
                try
                {
                    Interpreter interpreter = new(textEditor.Text, false);
                    interpreter.Start();
                    poses = interpreter.GetTrajectory();
                }
                catch (CodeException)
                {
                    playerBorder.Background = new SolidColorBrush(Colors.Red);

                    return;
                }

                string str = serialize(poses);
                sw.Write(str);
            }
        }

        private void Result_Load(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Program result (*.result)|*.result"
            };

            if(ofd.ShowDialog() == true)
            {
                using StreamReader sr = new(ofd.FileName);

                (int, int)[]? poses = deserialize(sr.ReadToEnd());

                if (poses == null)
                {
                    playerBorder.Background = new SolidColorBrush(Colors.Red);

                    return;
                }


                playerBorder.Background = new SolidColorBrush(Colors.Green);
                positions = poses;
                animating = true;
                Animate(0);
            }
        }

        private string serialize((int, int)[] poses)
        {
            string str = "";

            foreach ((int x, int y) pos in poses)
            {
                str += $"{pos.x},{pos.y};";
            }

            return str;
        }

        private (int, int)[]? deserialize(string str)
        {
            if(str.Length == 0)
                return null;

            List<(int x, int y)> poses = [];

            int i = 0;
            while (i < str.Length)
            {
                int? x = getInt(str, ',', ref i);

                if (x == null)
                    return null;

                int? y = getInt(str, ';', ref i);

                if (y == null)
                    return null;


                poses.Add(((int)x, (int)y));
            }

            if (poses.Count == 0)
                return null;

            (int, int)[] res = new (int, int)[poses.Count];
            for(int j = 0; j < poses.Count; j++)
            {
                res[j] = poses[j];
            }

            return res;
        }

        private int? getInt(string str, char target, ref int pos)
        {
            char c;
            int start = pos;
            do
            {
                c = str[pos++];

            } while (c != target && pos < str.Length);

            if(c != target)
                return null;

            if (!int.TryParse(str[start..(pos - 1)], out int num) ||
                num < 0 || num > 20) return null;

            return num;
        }

        private void Button_Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}