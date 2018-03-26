using App4.Assets;
using HomeWork1.Model;
using HomeWork1.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static App4.Assets.NewPage;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace App4
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    ///  

    public sealed partial class MainPage : Page
    {

        public HomeWork1.ViewModels.ItemListViewModels ViewModel { get;set; }

        public MainPage()
        {
            this.InitializeComponent();

            this.ViewModel = new ItemListViewModels();
           
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (e.Parameter.GetType() == typeof(ItemListViewModels))
            {
                this.ViewModel = (ItemListViewModels)(e.Parameter);
            }
        }

        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.AddItemStackPanel.Visibility.ToString() == "Collapsed")
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
        }

        private void ItemList_ItemClick(object sender, ItemClickEventArgs i)
        {
            ViewModel.SelectedItem = (ItemList)(i.ClickedItem);

            if(this.AddItemStackPanel.Visibility.ToString() == "Collapsed")
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
            else
            {
                this.CreateButtonOrUpdate.Content = "Update";
                this.TitleTextBox.Text = ViewModel.SelectedItem.title;
                this.DetailTextBox.Text = ViewModel.SelectedItem.detail;
                this.Image.Source = ViewModel.SelectedItem.img;
                this.Datepicker.Date = ViewModel.SelectedItem.date;
            }
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
                if(this.CreateButtonOrUpdate.Content.ToString() == "Create")
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

            if(file != null)
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

