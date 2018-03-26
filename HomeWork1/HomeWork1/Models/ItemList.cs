using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HomeWork1.Model
{
    public class ItemList : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string id;

        private string title_;
        public string title
        {
            get { return title_; }
            set
            {
                title_ = value;
                NotifyPropertyChanged("title");
            }
        }

        private ImageSource img_;
        public ImageSource img
        {
            get { return img_; }
            set
            {
                img_ = value;
                NotifyPropertyChanged("img");
            }
        }

        public string detail { get; set; }
        public bool completed { get; set; }
        public DateTime date { get; set; }

        public ItemList(string title_in, string detail_in, DateTime date_in, ImageSource img_in)
        {
            this.id = Guid.NewGuid().ToString();
            this.title = title_in;
            this.detail = detail_in;
            this.img = img_in;
            this.date = date_in;
            this.completed = false;
        }

        private void NotifyPropertyChanged(string name)
        {
            if (this.PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
