using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace HomeWork1.ViewModels
{
    public class ItemListViewModels
    {
        private ObservableCollection<Model.ItemList> allItems = new ObservableCollection<Model.ItemList>();
        public ObservableCollection<Model.ItemList> AllItems { get { return this.allItems; } }

        private Model.ItemList selectedItem = null;
        public Model.ItemList SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public ItemListViewModels()
        {
            this.selectedItem = null;
            this.allItems.Add(new Model.ItemList("TestTitle1", "TestDetai1", DateTime.Today, null));
            this.allItems.Add(new Model.ItemList("TestTitle2", "TestDetai2", DateTime.Today, null));
        }

        public void AddItemList(string title, string detail, DateTime date, ImageSource imageSource)
        {
            this.allItems.Add(new Model.ItemList(title, detail, date, imageSource));
        }

        public void RemoveItemList(string id)
        {
            this.allItems.Remove(selectedItem);
            this.selectedItem = null;
        }

        public void UpdateItemList(string title, string detail, DateTime date, ImageSource imageSource)
        {
            this.selectedItem.pub_title = title;
            this.selectedItem.detail = detail;
            this.selectedItem.pub_img = imageSource;
            this.selectedItem.date = date;

            this.selectedItem = null;
        }
    }
}
