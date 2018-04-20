using Query.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public GameList gamelist;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textbox1.Text == "")
            {
                textblock1.Text = "城市名不能为空";
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
                textblock1.Text = "找不到输入的城市！";
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {

        }
    }
}
