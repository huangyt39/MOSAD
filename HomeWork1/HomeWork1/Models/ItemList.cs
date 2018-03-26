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
    public class ItemList
        { 

        private string id;

        private string title;
        public string pub_title
        {
            set { title = value;  }
            get { return title; }
        }

        private ImageSource img;
        public ImageSource pub_img
        {
            set { img = value; }
            get { return img; }
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

    }
}
