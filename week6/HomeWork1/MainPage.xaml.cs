using App4.Assets;
using HomeWork1.Model;
using HomeWork1.Services;
using HomeWork1.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using HomeWork1.Models;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Navigation;
using static App4.Assets.NewPage;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using System.Text;
using SQLitePCL;
using Windows.Storage.AccessCache;
using HomeWork1;

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

        public string imgPath;
        private string shareTitle = "", shareDescription = "", shareImgName = "";
        private StorageFile shareImg;
        //DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
        private string shareDate;

        int ItemIndex = 0;

        public MainPage()
        {
            this.InitializeComponent();

            this.ViewModel = new ItemListViewModels();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(4);
            timer.Tick += (x, y) =>
            {
                if (ViewModel.AllItems.Count > ItemIndex)
                {
                    var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                    TileNotification notification;

                    var item = ViewModel.AllItems.ElementAt(ItemIndex);

                    var xmlDoc = TileService.CreateTiles(new PrimaryTile(item.title, item.detail, item.date));

                    notification = new TileNotification(xmlDoc);
                    updater.Update(notification);

                    ItemIndex = (ItemIndex + 1) % ViewModel.AllItems.Count;
                }
            };
            timer.Start();
        }

        //public void DisPlayTile()
        //{
        //    var updater = TileUpdateManager.CreateTileUpdaterForApplication();
        //    TileNotification notification;

        //    var item = ViewModel.AllItems.ElementAt(ItemIndex);

        //    var xmlDoc = TileService.CreateTiles(new PrimaryTile(item.title, item.detail, item.date));

        //    notification = new TileNotification(xmlDoc);
        //    updater.Update(notification);

        //    ItemIndex = (ItemIndex + 1) % ViewModel.AllItems.Count;

        //}

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (e.Parameter.GetType() == typeof(ItemListViewModels))
            {
                this.ViewModel = (ItemListViewModels)(e.Parameter);
            }

            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgres" +
                    "s"))
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

            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
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
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;

            request.Data.Properties.Title = shareTitle;
            request.Data.Properties.Description = shareDescription;
            request.Data.SetText(shareDescription + shareDate);

            try
            {
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(shareImg));
            }
            finally
            {
                request.GetDeferral().Complete();
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

        private void ItemList_ItemClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SelectedItem = (ItemList)((MenuFlyoutItem)sender).DataContext;

            if (this.AddItemStackPanel.Visibility.ToString() == "Collapsed")
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
                    long newId = 1;
                    for(long i = 1;i < 10;i++)
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
                imgPath = file.Path;
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    await srcImage.SetSourceAsync(stream);
                    this.Image.Source = srcImage;
                }
            }

        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SelectedItem = (ItemList)((MenuFlyoutItem)sender).DataContext;
            if(this.ViewModel.SelectedItem != null)
            {
                DeleteItem(this.ViewModel.SelectedItem.id);
                ViewModel.usingId[this.ViewModel.SelectedItem.id] = false;
                this.ViewModel.RemoveItemList(this.ViewModel.SelectedItem);
                await new MessageDialog("Delete successfully!").ShowAsync();
                this.ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            this.ViewModel.SelectedItem = null;
        }

        async private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.SelectedItem = (ItemList)((MenuFlyoutItem)sender).DataContext;

            shareTitle = this.ViewModel.SelectedItem.title;
            shareDescription = this.ViewModel.SelectedItem.detail;
            //shareImgName = item.img;
            var date = this.ViewModel.SelectedItem.date;
            shareDate = "\nDue date: " + date.Year + '-' + date.Month + '-' + date.Day;
            if (shareImgName == "")
            {
                shareImg = await Package.Current.InstalledLocation.GetFileAsync("Assets\\icon1.jpg");
            }
            else
            {
                shareImg = await ApplicationData.Current.LocalFolder.GetFileAsync(shareImgName);
            }
            DataTransferManager.ShowShareUI();

            this.ViewModel.SelectedItem = null;
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
            catch(Exception e)
            {
                throw e;
            }
        }

        private void UpdateItem(long id, string title, string detail, string date, bool? ischeck, string imgpath)
        {
            var db = HomeWork1.App.conn;
            try
            {
                string sql = @"UPDATE ItemList SET Title = ?, Detail = ?, Date = ?, Ischeck = ?, Img = ? WHERE Id = ?";
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
            catch(Exception e)
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

        async private void GetItem(object sender, RoutedEventArgs e)
        {
            var db = HomeWork1.App.conn;
            string searchstr = search.Text;
            if (searchstr == "") return;
            StringBuilder Messagestr = new StringBuilder();
            try
            {
                string sql = @"SELECT Title, Detail, Date FROM ItemList WHERE Title LIKE ? OR Detail LIKE ? OR Date LIKE ?";
                using (var item = db.Prepare(sql))
                {
                    item.Bind(1, "%%" + searchstr + "%%");
                    item.Bind(2, "%%" + searchstr + "%%");
                    item.Bind(3, "%%" + searchstr + "%%");
                    while(SQLiteResult.ROW == item.Step())
                    {
                        Messagestr.Append("Title: ");
                        Messagestr.Append((string)item[0]);
                        Messagestr.Append("Detail: ");
                        Messagestr.Append((string)item[1]);
                        Messagestr.Append("Date: ");
                        Messagestr.Append((string)item[2]);
                        Messagestr.Append("\n");
                    }

                    if(Messagestr.Equals(new StringBuilder()))
                    {
                        Messagestr.Append("No result!");
                    }
                    await new MessageDialog(Messagestr.ToString()).ShowAsync();
                    search.Text = "";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

