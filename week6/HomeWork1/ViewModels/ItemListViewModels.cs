using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HomeWork1.ViewModels
{
    public class ItemListViewModels
    {
        public bool[] usingId = new bool[10];

        private ObservableCollection<Model.ItemList> allItems = new ObservableCollection<Model.ItemList>();
        public ObservableCollection<Model.ItemList> AllItems { get { return this.allItems; } }

        private Model.ItemList selectedItem = null;
        public Model.ItemList SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public ItemListViewModels()
        {
            this.selectedItem = null;
            var conn = HomeWork1.App.conn;
            var sql = "SELECT * FROM ItemList";
            try
            {
                using(var statement = conn.Prepare(sql))
                {
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        string datestr = (string)statement[3];
                        datestr = datestr.Substring(0, datestr.IndexOf(' '));
                        DateTime date = new DateTime(int.Parse(datestr.Split('/')[0]), int.Parse(datestr.Split('/')[1]), int.Parse(datestr.Split('/')[2]));
                        string filename = (string)statement[5];

                        if (filename != null) { 
                            Uri uri = new Uri(filename, UriKind.RelativeOrAbsolute);
                            BitmapImage Btm = new BitmapImage(uri);

                            this.AddItemList((long)statement[0], (string)statement[1], (string)statement[2], date, Btm, (long)statement[4] == 1 ? true : false);
                         }
                        else this.AddItemList((long)statement[0], (string)statement[1], (string)statement[2], date, null, (long)statement[4] == 1 ? true : false);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            foreach (var item in this.AllItems)
            {
                usingId[item.id] = true;
            }
        }

        public void AddItemList(long id, string title, string detail, DateTime date, ImageSource imageSource, bool? ischeck)
        {
            this.allItems.Add(new Model.ItemList(id, title, detail, date, imageSource, ischeck));
        }

        public void RemoveItemList(Model.ItemList it)
        {
            this.allItems.Remove(it);
            this.selectedItem = null;
        }

        public void UpdateItemList(long id, string title, string detail, DateTime date, ImageSource imageSource, bool? ischeck)
        {
            this.selectedItem.title = title;
            this.selectedItem.detail = detail;
            this.selectedItem.img = imageSource;
            this.selectedItem.date = date;

            this.selectedItem = null;
        }
    }
}
