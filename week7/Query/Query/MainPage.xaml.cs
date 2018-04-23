using Query.Model;
using Query.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Query
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public GameList gamelist = new GameList();

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (e.Parameter.GetType() == typeof(GameList))
            {
                this.gamelist = (GameList)(e.Parameter);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textbox1.Text == "")
            {
                await new MessageDialog("城市名不能为空！").ShowAsync();
                textblock1.Text = "";
                weatherimg.Source = null;
                return;
            }

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;

            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri("https://api.seniverse.com/v3/weather/now.json?location=" + textbox1.Text + "&key=eapo0y2fnurmrafq&language=zh-Hans&unit=c");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            JsonObject jsonobject = JsonObject.Parse(httpResponseBody);

            try
            {
                string result = jsonobject.GetNamedValue("results").ToString();
                result = result.Replace("[{", "{").Replace("}]", "}");

                JsonObject resobj = JsonObject.Parse(result);

                JsonObject locationobj = resobj.GetNamedObject("location");
                JsonObject nowobj = resobj.GetNamedObject("now");

                string name = "城市: " + locationobj.GetNamedString("name");
                string text = "天气现象: " + nowobj.GetNamedString("text");
                string temperature = "温度: " + nowobj.GetNamedString("temperature") + "度";
                string weacode = nowobj.GetNamedString("code");

                textblock1.Text = name + "\n" + text + "\n" + temperature + "\n";
                Uri imguri = new Uri("ms-appx:///Assets/Weatherpic/" + weacode.ToString() + ".png");
                BitmapImage bmi = new BitmapImage(imguri);
                weatherimg.Source = bmi;
            }
            catch (Exception ex)
            {
                weatherimg.Source = null;
                await new MessageDialog("找不到该城市！").ShowAsync();
                textblock1.Text = "";
            }
        }

        private async void Button_Click2(object sender, RoutedEventArgs e)
        {
            if (textbox2.Text == "")
            {
                gamelist.Allgame.Clear();
                await new MessageDialog("球队名不能为空！").ShowAsync();
                Frame.Navigate(typeof(MainPage), gamelist);
                weatherimg.Source = null;
                return;
            }

            if(gamelist.Allgame.Count > 0) gamelist.Allgame.Clear();

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;

            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri("http://op.juhe.cn/onebox/basketball/team?dtype=xml&team=" + textbox2.Text + "&key=9600b31f1a1946ac65f80a8a7153b7de");

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";

            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                //httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            XmlNodeList nodelist = null;

            try
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(httpResponseBody);
                XmlNode rootnode = xdoc.LastChild;
                XmlNodeList rootchild = rootnode.ChildNodes;
                XmlNode resultnode = rootchild[1];
                XmlNodeList resultchild = resultnode.ChildNodes;
                XmlNode list = resultchild[1];
                nodelist = list.ChildNodes;
            }
            catch(Exception ex)
            {
                gamelist.Allgame.Clear();
                await new MessageDialog("找不到该球队！").ShowAsync();
                Frame.Navigate(typeof(MainPage), gamelist);
                return;
            }

            foreach (XmlNode xe in nodelist)
            {
                string t1 = xe.ChildNodes[2].InnerText;
                string t2 = xe.ChildNodes[3].InnerText;
                string t = xe.ChildNodes[1].InnerText;
                string s = xe.ChildNodes[10].InnerText;
                gamelist.Allgame.Add(new Model.Game(t1, t2, t, s));
            }

            Frame.Navigate(typeof(MainPage), gamelist);
        }
    }
}
