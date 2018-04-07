using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWork1.Models
{
    public class PrimaryTile
    {
        public string time { get; set; } = "2018-04-06";
        public string message { get; set; } = "Title";
        public string message2 { get; set; } = "Detail!!!";
        public string branding { get; set; } = "name";
        public string appName { get; set; } = "ToDoItem";

        public PrimaryTile(string _title, string _detail, DateTime _date)
        {
            time = _date.ToString();
            message = _title;
            message2 = _detail;
        }
    }
}
