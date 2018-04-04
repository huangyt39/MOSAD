using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Windows.UI.Xaml.Controls.Page;
using static App4.MainPage;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace App4.Assets
{

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {

        private HomeWork1.ViewModels.ItemListViewModels ViewModel;



        public NewPage()
        {
            this.InitializeComponent();
            ElementSoundPlayer.Volume = 0.5;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox tt = (TextBox)TitleTextBox;
            TextBox dt = (TextBox)DetailTextBox;
            tt.Text = "";
            dt.Text = "";
            DatePicker dp = (DatePicker)Datepicker;
            dp.Date = DateTime.Today.Date;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            ViewModel = ((HomeWork1.ViewModels.ItemListViewModels)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                CreateButton.Content = "Create";
            }
            else
            {
                CreateButton.Content = "Update";
                TitleTextBox.Text = ViewModel.SelectedItem.title;
                Image.Source = ViewModel.SelectedItem.img;
                DetailTextBox.Text = ViewModel.SelectedItem.detail;
                Datepicker.Date = ViewModel.SelectedItem.date;
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox tt = (TextBox)TitleTextBox;
            TextBox dt = (TextBox)DetailTextBox;
            string ErrorMessage = "";
            if (tt.Text == "") ErrorMessage += "Title不能为空！\n";
            if (dt.Text == "") ErrorMessage += "Detail不能为空！\n";
            DatePicker dp = (DatePicker)Datepicker;
            if (dp.Date < DateTime.Today.Date) ErrorMessage += "DueDate不正确！\n";
            if (ErrorMessage != "") await new MessageDialog(ErrorMessage).ShowAsync();
            else
            {
                if (this.CreateButton.Content.ToString() == "Create")
                {
                    this.ViewModel.AddItemList(TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime, Image.Source);
                    await new MessageDialog("Create successfully!").ShowAsync();
                    this.ViewModel.SelectedItem = null;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
                else
                {
                    ViewModel.UpdateItemList(TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime, Image.Source);
                    await new MessageDialog("Update successfully!").ShowAsync();
                    ViewModel.SelectedItem = null;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), ViewModel);
        }

        private async void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.ViewModel.SelectedItem != null)
            {
                ViewModel.AllItems.Remove(ViewModel.SelectedItem);
                ViewModel.SelectedItem = null;
                await new MessageDialog("Delete successfully!").ShowAsync();
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private async void SelectImgButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            StorageFile file = await openPicker.PickSingleFileAsync();
            BitmapImage srcImage = new BitmapImage();

            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    this.Image.Source = srcImage;
                }
            }

        }
    }
}
