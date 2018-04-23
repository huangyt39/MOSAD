using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Query.Model
{
    public class Game
    {
        public string team1 { get; set; }
        public string team2 { get; set; }
        public string time { get; set; }
        public string socre { get; set; }

        public Game(string t1, string t2, string t, string s)
        {
            this.team1 = t1;
            this.team2 = t2;
            this.time = t;
            this.socre = s;
        }
    }
}
