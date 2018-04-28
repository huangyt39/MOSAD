using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MusicPlay
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        double fullWidth, currentWidth, fullHeight, currentHeight;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void OnMouseDownPlayMedia(object sender, RoutedEventArgs e)
        {
            myMediaElement.Play();
            InitializePropertyValues();
        }

        private void OnMouseDownPauseMedia(object sender, RoutedEventArgs e)
        {
            myMediaElement.Pause();
        }

        private void OnMouseDownStopMedia(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }

        private async void OpenFileClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            //选择视图模式  
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            //openPicker.ViewMode = PickerViewMode.List;  
            //初始位置  
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            //添加文件类型  
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".avi");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".asf");
            openPicker.FileTypeFilter.Add(".wma");

            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                myMediaElement.SetSource(stream, file.ContentType);
                currentWidth = myMediaElement.Width;
                currentHeight = myMediaElement.Height;
            }
        }

        private void fullScreenClick(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();

            bool isInFullScreenMode = view.IsFullScreenMode;

            if (isInFullScreenMode)
            {
                view.ExitFullScreenMode();
                fullWidth = myMediaElement.Width;
                fullHeight = myMediaElement.Height;
                myMediaElement.Width = currentWidth;
                myMediaElement.Height = currentHeight;
            }
            else
            {
                view.TryEnterFullScreenMode();
                fullWidth = 1280;
                fullHeight = 720;


                currentWidth = myMediaElement.Width;
                currentHeight = myMediaElement.Height;
                myMediaElement.Width = fullWidth;
                myMediaElement.Height = fullHeight;
            }
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            myMediaElement.Stop();
        }

        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            var ts = myMediaElement.NaturalDuration.TimeSpan;
            total.Text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timelineSlider.Maximum = ts.TotalMilliseconds;
        }

        private void ChangeMediaVolume(object sender, RangeBaseValueChangedEventArgs e)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        private void ChangeMediaSpeedRatio(object sender, RangeBaseValueChangedEventArgs e)
        {
            
        }

        private void SeekMediaPosition(object sender, RangeBaseValueChangedEventArgs e)
        {
            int SliderValue = (int)timelineSlider.Value;

            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            myMediaElement.Position = ts;
        }

        void InitializePropertyValues()
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        private void myMediaElementLoaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += (ss, ee) =>
            {
                //显示当前视频进度
                var ts = myMediaElement.Position;
                current.Text = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
                timelineSlider.Value = ts.TotalMilliseconds;
            };
            timer.Start();
        }
    }
}
