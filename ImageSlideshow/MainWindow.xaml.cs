using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> AvailableSlideshowEffects { get; set; }
        public string SelectedSlideshowEffect { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // Set the DataContext to the MainWindow instance

            // Populate the AvailableSlideshowEffects collection
            AvailableSlideshowEffects = new ObservableCollection<string>
            {
                "Horizontal Effect",
                "Opacity Effect",
                "Vertical Effect"
            };
        }
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadingDrivers();
        }

        private string selectedFolderPath;
        private void OpenFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                selectedFolderPath = dialog.SelectedPath;
                string[] imageFiles = Directory.GetFiles(selectedFolderPath, "*.jpg");
                ImageListView.ItemsSource = imageFiles;
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Image Explorer Application\nVersion 1.0\n\n LAB WPF2 GRADED", "About");
        }

        //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-find-a-treeviewitem-in-a-treeview?view=netframeworkdesktop-4.8

        private void LoadingDrivers()
        {
            ExplorerTreeView.Items.Clear();

            string[] partitions = Directory.GetLogicalDrives();

            foreach (string partition in partitions)
            {
                TreeViewItem partitionItem = new TreeViewItem();
                partitionItem.Header = partition;
                partitionItem.Tag = partition;
                partitionItem.Expanded += Expanded_Drivers;
                partitionItem.Items.Add(null);
                ExplorerTreeView.Items.Add(partitionItem);

            }
        }

        private void Expanded_Drivers(object sender, RoutedEventArgs e)
        {
            TreeViewItem partitionItem = (TreeViewItem)sender;

            if (partitionItem.Items.Count == 1 && partitionItem.Items[0] == null)
            {
                partitionItem.Items.Clear();

                string partitionPath = (string)partitionItem.Tag;

                string[] directories = Directory.GetDirectories(partitionPath);

                foreach (string directory in directories)
                {
                    TreeViewItem directoryItem = new TreeViewItem();
                    directoryItem.Header = new DirectoryInfo(directory).Name;
                    directoryItem.Tag = directory;
                    directoryItem.Expanded += Expanded_Directories;
                    directoryItem.Items.Add(null);
                    partitionItem.Items.Add(directoryItem);
                }
            }
        }

        private void Expanded_Directories(object sender, RoutedEventArgs e)
        {
            TreeViewItem directoryItem = (TreeViewItem)sender;

            if (directoryItem.Items.Count == 1 && directoryItem.Items[0] == null)
            {
                directoryItem.Items.Clear();
                string directoryPath = (string)directoryItem.Tag;
                string[] directories = Directory.GetDirectories(directoryPath);

                foreach (string directory in directories)
                {
                    TreeViewItem subDirectoryItem = new TreeViewItem();
                    subDirectoryItem.Header = new DirectoryInfo(directory).Name;
                    subDirectoryItem.Tag = directory;
                    subDirectoryItem.Expanded += Expanded_Directories;
                    subDirectoryItem.Items.Add(null);
                    directoryItem.Items.Add(subDirectoryItem);
                }

                string[] imageFiles = Directory.GetFiles(directoryPath, "*.jpg");
                ImageListView.ItemsSource = imageFiles;
            }
        }
        //https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.selectionchangedeventargs?view=windowsdesktop-7.0
        private void ImageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListView.SelectedItem != null)
            {
                string imagePath = (string)ImageListView.SelectedItem;
                string fileName = System.IO.Path.GetFileName(imagePath);
                FileInfoTextBlock.Text = $"File Name: {fileName}";

                BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

                int width = bitmapImage.PixelWidth;
                int height = bitmapImage.PixelHeight;
                FileInfo fileInfo = new FileInfo(imagePath);
                long fileSize = fileInfo.Length / 1024;
                WidthTextBlock.Text = $"Width: {width}px";
                HeightTextBlock.Text = $"Height: {height}px";
                SizeTextBlock.Text = $"Size: {fileSize} KB";

            }
            else
            {
                FileInfoTextBlock.Text = "No file selected";
                WidthTextBlock.Text = string.Empty;
                HeightTextBlock.Text = string.Empty;
                SizeTextBlock.Text = string.Empty;
            }
        }

        private void ApplyTransitionEffects(SlideshowWindow slideshowWindow)
        {
            int currentIndex = slideshowWindow.IndexCurr;

            switch (slideshowWindow.SelectedEffect)
            {
                case "Horizontal Effect":
                    slideshowWindow.ApplyHorizontalEffect(currentIndex);
                    break;
                case "Vertical Effect":
                    slideshowWindow.ApplyVerticalEffect(currentIndex);
                    break;
                case "Opacity Effect":
                    slideshowWindow.ApplyOpacityEffect(currentIndex);
                    break;
                default:
                    break;
            }
        }
        private void SlideshowEffectMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = (System.Windows.Controls.MenuItem)sender;
            string selectedEffect = menuItem.Header.ToString();

            StartSlideshow(selectedEffect);
        }

        private void StartSlideshowButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedEffect = EffectComboBox.SelectedItem as string;
            if (selectedEffect != null)
            {
                StartSlideshow(selectedEffect);
            }
        }

        private void StartSlideshow(string effect)
        {
            SlideshowWindow slideshowWindow = new SlideshowWindow(effect);

            slideshowWindow.InitializeSlideshow(selectedFolderPath);

            slideshowWindow.Loaded += (sender, e) =>
            {
                ApplyTransitionEffects(slideshowWindow);
            };

            slideshowWindow.ShowDialog();

        }
    }
    //Tutorial
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filePath = value as string;
            if (!string.IsNullOrEmpty(filePath))
            {
                return System.IO.Path.GetFileName(filePath);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

