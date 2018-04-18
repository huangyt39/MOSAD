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
using Windows.Storage.AccessCache;
using HomeWork1;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace App4.Assets
{

    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public string imgPath;
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
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

            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;
                    TitleTextBox.Text = (string)composite["Title"];
                    DetailTextBox.Text = (string)composite["Detail"];
                    Datepicker.Date = Convert.ToDateTime((string)composite["Date"]);

                    StorageFile theFile = await StorageApplicationPermissions.FutureAccessList.GetFileAsync((string)ApplicationData.Current.LocalSettings.Values["MyToken"]);
                    BitmapImage srcImage = new BitmapImage();
                    if (theFile != null)
                    {
                        ApplicationData.Current.LocalSettings.Values["MyToken"] = StorageApplicationPermissions.FutureAccessList.Add(theFile);
                        using (IRandomAccessStream stream = await theFile.OpenAsync(FileAccessMode.Read))
                        {
                            await srcImage.SetSourceAsync(stream);
                            this.Image.Source = srcImage;
                        }
                    }

                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool issuspending = ((App)App.Current).isSuspending;
            if (issuspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                composite["Title"] = TitleTextBox.Text;
                composite["Detail"] = DetailTextBox.Text;
                composite["Date"] = Datepicker.Date.ToString();

                ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;
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
                    long newId = 1;
                    for (long i = 1; i < 10; i++)
                    {
                        if (!ViewModel.usingId[i])
                        {
                            ViewModel.usingId[i] = true;
                            newId = i;
                            break;
                        }
                    }
                    InsertItem(newId, TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime.ToString(), false, imgPath);
                    this.ViewModel.AddItemList(newId, TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime, Image.Source, false);
                    await new MessageDialog("Create successfully!").ShowAsync();
                    this.ViewModel.SelectedItem = null;
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
                else
                {
                    UpdateItem(ViewModel.SelectedItem.id, TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime.ToString(), ViewModel.SelectedItem.ischeck, imgPath);
                    ViewModel.UpdateItemList(ViewModel.SelectedItem.id, TitleTextBox.Text, DetailTextBox.Text, Datepicker.Date.DateTime, Image.Source, ViewModel.SelectedItem.ischeck);
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
                DeleteItem(this.ViewModel.SelectedItem.id);
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

        private void InsertItem(long id, string title, string detail, string date, bool? ischeck, string imgpath)
        {
            var db = HomeWork1.App.conn;
            try
            {
                using (var newItem = db.Prepare("INSERT INTO ItemList (Id, Title, Detail, Date, Ischeck, Img) VALUES(?, ?, ?, ?, ?, ?)"))
                {
                    newItem.Bind(1, id);
                    newItem.Bind(2, title);
                    newItem.Bind(3, detail);
                    newItem.Bind(4, date);
                    newItem.Bind(5, ischeck == true ? 1 : 0);
                    newItem.Bind(6, imgpath);
                    newItem.Step();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void UpdateItem(long id, string title, string detail, string date, bool? ischeck, string imgpath)
        {
            var db = HomeWork1.App.conn;
            try
            {
                string sql = @"UPDATE ItemList SET Title = ?, Detail = ?, Date = ?, Ischeck = ? WHERE Id = ?";
                using (var statement = db.Prepare(sql))
                {
                    statement.Bind(1, title);
                    statement.Bind(2, detail);
                    statement.Bind(3, date);
                    statement.Bind(4, ischeck == true ? 1 : 0);
                    statement.Bind(5, imgpath);
                    statement.Bind(6, id);
                    statement.Step();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void LineCheckBox_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModel.AllItems)
            {
                var db = HomeWork1.App.conn;
                try
                {
                    string sql = @"UPDATE ItemList SET Ischeck = ? WHERE Id = ?";
                    using (var statement = db.Prepare(sql))
                    {
                        statement.Bind(1, item.ischeck == true ? 1 : 0);
                        statement.Bind(2, item.id);
                        statement.Step();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void DeleteItem(long id)
        {
            var db = HomeWork1.App.conn;
            using (var statement = db.Prepare("DELETE FROM ItemList WHERE Id = ?"))
            {
                statement.Bind(1, id);
                statement.Step();
            }
        }
    }
}
