using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WPF2
{
    /// <summary>
    /// Interaction logic for SlideshowWindow.xaml
    /// </summary>
    public partial class SlideshowWindow : Window
    {
        private DispatcherTimer SlideTimer;
        private bool isPaused = false;
        public string SelectedEffect { get; set; }
        public List<Image> Images { get; set; }
        public int IndexCurr { get; set; }

        public SlideshowWindow(string effect)
        {
            InitializeComponent();
            SelectedEffect = effect;
            Images = new List<Image>();
            IndexCurr = 0;
        }

        public void InitializeSlideshow(string folderPath)
        {
            string[] imageFiles = Directory.GetFiles(folderPath, "*.jpg");

            foreach (string imagePath in imageFiles)
            {
                BitmapImage imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.UriSource = new Uri(imagePath, UriKind.Absolute);
                imageSource.EndInit();

                Image image = new Image();
                image.Source = imageSource;
                image.Tag = Images.Count;

                Images.Add(image);
                canvas.Children.Add(image); 
            }

            if (Images.Count > 0)
            {
                slideshowImage.Source = Images[0].Source;
            }

            SlideTimer = new DispatcherTimer();
            SlideTimer.Interval = TimeSpan.FromSeconds(2);

            SlideTimer.Tick += (sender, e) =>
            {
                IndexCurr = (IndexCurr + 1) % Images.Count;
                slideshowImage.Source = Images[IndexCurr].Source;
                switch (SelectedEffect)
                {
                    case "Horizontal Effect":
                        ApplyHorizontalEffect(IndexCurr);
                        break;
                    case "Vertical Effect":
                        ApplyVerticalEffect(IndexCurr);
                        break;
                    case "Opacity Effect":
                        ApplyOpacityEffect(IndexCurr);
                        break;
                    default:
                        break;
                }
            };
            SlideTimer.Start();
        }


        public void ApplyHorizontalEffect(int currentIndex)
        {
            double Width = canvas.ActualWidth;
            double initialX = Width;
            double finalX = 0;
            try
            {
                foreach (Image image in canvas.Children.OfType<Image>())
                {
                    if (image.Tag != null && int.TryParse(image.Tag.ToString(), out int index))
                    {
                        if (index == currentIndex)
                        {
                            DoubleAnimation horizontalAnimation = new DoubleAnimation();
                            horizontalAnimation.From = initialX;
                            horizontalAnimation.To = finalX;
                            horizontalAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));

                            image.BeginAnimation(Canvas.LeftProperty, horizontalAnimation);
                        }
                        else if (index > currentIndex)
                        {
                            Canvas.SetLeft(image, initialX);
                        }
                        else
                        {
                            Canvas.SetLeft(image, finalX);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ApplyVerticalEffect(int currentIndex)
        {
            double canvasHeight = canvas.ActualHeight;
            double initialY = canvasHeight;
            double finalY = 0;

            foreach (Image image in canvas.Children.OfType<Image>())
            {
                if (image.Tag != null && int.TryParse(image.Tag.ToString(), out int index))
                {
                    if (index == currentIndex)
                    {
                        DoubleAnimation verticalAnimation = new DoubleAnimation();
                        verticalAnimation.From = initialY;
                        verticalAnimation.To = finalY;
                        verticalAnimation.Duration = new Duration(TimeSpan.FromSeconds(3));

                        image.BeginAnimation(Canvas.TopProperty, verticalAnimation);
                    }
                    else if (index > currentIndex)
                    {
                        Canvas.SetTop(image, initialY);
                    }
                    else
                    {
                        Canvas.SetTop(image, finalY);
                    }
                }
            }
        }

        public void ApplyOpacityEffect(int currentIndex)
        {
            foreach (Image image in canvas.Children.OfType<Image>())
            {
                if (image.Tag != null && int.TryParse(image.Tag.ToString(), out int index))
                {
                    if (index == currentIndex)
                    {
                        DoubleAnimation opacityAnimation = new DoubleAnimation();
                        opacityAnimation.From = 0;
                        opacityAnimation.To = 1;
                        opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                        image.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
                    }
                    else
                    {
                        image.Opacity = 0;
                    }
                }
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                SlideTimer.Start();
                isPaused = false;
            }
            else
            {
                SlideTimer.Stop();
                isPaused = true;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
