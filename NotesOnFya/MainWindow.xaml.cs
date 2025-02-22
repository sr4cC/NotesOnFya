using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Threading;

namespace NotesOnFya
{
    public partial class MainWindow : Window
    {
        private string currentFilePath = null;
        private DispatcherTimer hideMessageTimer;
        private int lastIndex = 0;
        public MainWindow()
        {
            InitializeComponent();

            //Timer
            hideMessageTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5) //5 seconds
            };
            hideMessageTimer.Tick += HideSaveMessage;

            InputBindings.Add(new KeyBinding(new RelayCommand(OpenFontSizeModal), Key.Q, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new RelayCommand(OpenThemesModal), Key.T, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new RelayCommand(OpenHelpModal), Key.H, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new RelayCommand(SaveFile), Key.S, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new RelayCommand(SaveFileAs), Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            InputBindings.Add(new KeyBinding(new RelayCommand(OpenFile), Key.O, ModifierKeys.Control));
            InputBindings.Add(new KeyBinding(new RelayCommand(OpenFindModal), Key.F, ModifierKeys.Control));
        }

        //Save file
        private void SaveFile(object sender)
        {
            if (currentFilePath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName; 
                }
                else
                {
                    return; 
                }
            }

            File.WriteAllText(currentFilePath, txt_block.Text);
            file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);

            ShowSaveMessage();
        }
        private void SaveFileAs(object sender)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, txt_block.Text);
                file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            }
        }
        private void ShowSaveMessage()
        {
            saveMessage.Visibility = Visibility.Visible;
            hideMessageTimer.Start();
        }
        private void HideSaveMessage(object sender, EventArgs e)
        {
            saveMessage.Visibility = Visibility.Collapsed;
            hideMessageTimer.Stop();
        }

        //Open file
        private void OpenFile(object sender)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json|Markdown Files (*.md)|*.md|XML Files (*.xml)|*.xml|CSV Files (*.csv)|*.csv|INI Files (*.ini)|*.ini|C# Files (*.cs)|*.cs|JavaScript Files (*.js)|*.js|Python Files (*.py)|*.py|All Files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == true)
            {
                txt_block.Text = File.ReadAllText(openFileDialog.FileName);
                currentFilePath = openFileDialog.FileName; 
                file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            }
        }

        //Themes
        private void OpenThemesModal(object obj)
        {
            if (ThemesModalOverlay.Visibility == Visibility.Visible)
            {
                ThemesModalOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                ThemesModalOverlay.Visibility = Visibility.Visible;
            }
        }
        private void CloseThemesModal(object sender, RoutedEventArgs e)
        {
            ThemesModalOverlay.Visibility = Visibility.Collapsed;
        }
        private void SwitchTheme_Dark(object sender, RoutedEventArgs e)
        {
            ResourceDictionary darkTheme = new ResourceDictionary
            {
                Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(darkTheme);
        }
        private void SwitchTheme_Light(object sender, RoutedEventArgs e)
        {
            ResourceDictionary lightTheme = new ResourceDictionary
            {
                Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(lightTheme);
        }
        private void SwitchTheme_NotesOnFya(object sender, RoutedEventArgs e)
        {
            ResourceDictionary notesOnFyaTheme = new ResourceDictionary
            {
                Source = new Uri("Themes/NotesOnFyaTheme.xaml", UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(notesOnFyaTheme);
        }

        //Font size
        private void OpenFontSizeModal(object obj)
        {
            if (FontSizeModalOverlay.Visibility == Visibility.Collapsed)
            {
                FontSizeModalOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                FontSizeModalOverlay.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseFontSizeModal(object sender, RoutedEventArgs e)
        {
            FontSizeModalOverlay.Visibility = Visibility.Collapsed;
        }
        private void ConfirmFontSize(object sender, RoutedEventArgs e)
        {
            // Check if the input in the TextBox is a valid number
            if (double.TryParse(FontSizeInput.Text, out double fontSize))
            {
                txt_block.FontSize = fontSize;
            }
            else
            {
                // Handle the case where the input is not a valid number
                MessageBox.Show("Please enter a valid number for the font size.");
            }

            FontSizeModalOverlay.Visibility = Visibility.Collapsed;
        }

        //Find text
        private void OpenFindModal(object obj)
        {
            if (FindModalOverlay.Visibility == Visibility.Visible)
            {
                FindModalOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                FindModalOverlay.Visibility = Visibility.Visible;

                double windowWidth = this.ActualWidth;
                double windowHeight = this.ActualHeight;

                double modalWidth = 250; 
                double modalHeight = 80; 

                Canvas.SetRight(FindModalOverlay, 10); 
                Canvas.SetTop(FindModalOverlay, 10);
            }
        }
        private void ConfirmFind(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextInput.Text;
            string fullText = txt_block.Text;

            if (!string.IsNullOrEmpty(searchText))
            {
                int index = fullText.IndexOf(searchText, lastIndex, StringComparison.OrdinalIgnoreCase);

                if (index >= 0)
                {
                    txt_block.Focus();
                    txt_block.Select(index, searchText.Length);
                    lastIndex = index + searchText.Length;
                }
                else
                {
                    lastIndex = 0;
                }
            }
        }

        //Help 
        private void OpenHelpModal(object obj)
        {
            if (HelpModalOverlay.Visibility == Visibility.Visible)
            {
                HelpModalOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                HelpModalOverlay.Visibility = Visibility.Visible;
            }
        }
        private void CloseHelpModal(object sender, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
        }

        //Open close stuff
        private void OpenThemesModalNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            ThemesModalOverlay.Visibility = Visibility.Visible;
        }
        private void OpenTextSizeModalNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            FontSizeModalOverlay.Visibility = Visibility.Visible;
        }
        private void SaveFileNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            if (currentFilePath == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    currentFilePath = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            File.WriteAllText(currentFilePath, txt_block.Text);
            file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);

            ShowSaveMessage();
        }
        private void OpenFileNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                txt_block.Text = File.ReadAllText(openFileDialog.FileName);
                currentFilePath = openFileDialog.FileName;
                file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            }
        }
        private void SaveFileAsNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                currentFilePath = saveFileDialog.FileName;
                File.WriteAllText(currentFilePath, txt_block.Text);
                file_name.Text = System.IO.Path.GetFileNameWithoutExtension(currentFilePath);
            }
        }
        private void OpenFindModalNCloseHelp(object obj, RoutedEventArgs e)
        {
            HelpModalOverlay.Visibility = Visibility.Collapsed;
            FindModalOverlay.Visibility = Visibility.Visible;

            double windowWidth = this.ActualWidth;
            double windowHeight = this.ActualHeight;

            double modalWidth = 250;
            double modalHeight = 80;

            Canvas.SetRight(FindModalOverlay, 10);
            Canvas.SetTop(FindModalOverlay, 10);
        }
    }
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute) => _execute = execute;

        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
    }
}